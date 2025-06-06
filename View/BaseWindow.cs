using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace oop_coursework.Views
{
    public class BaseWindow : Window
    {
        protected void InitializeMenu()
        {
            var menu = new Menu();
            var fileMenuItem = new MenuItem { Header = "Меню" };
            var logoutMenuItem = new MenuItem
            {
                Header = "Вийти",
                Command = new RelayCommand(_ => Logout())
            };

            fileMenuItem.Items.Add(logoutMenuItem);
            menu.Items.Add(fileMenuItem);

            if (Content is Grid grid)
            {
                grid.RowDefinitions.Insert(0, new RowDefinition { Height = GridLength.Auto });
                Grid.SetRow(grid.Children[0], 1);
                grid.Children.Insert(0, menu);
            }
        }

        protected virtual void Logout()
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }

        public class RelayCommand : ICommand
        {
            private readonly Action<object?> _execute;
            private readonly Func<object?, bool>? _canExecute;

            public event EventHandler? CanExecuteChanged;

            public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public bool CanExecute(object? parameter)
            {
                return _canExecute?.Invoke(parameter) ?? true;
            }

            public void Execute(object? parameter)
            {
                _execute(parameter);
            }

            public void RaiseCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        protected void CloseWindow()
        {
            Close();
        }
    }
}
