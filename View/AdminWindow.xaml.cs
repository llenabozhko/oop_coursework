using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using oop_coursework.Models;
using oop_coursework.Services;

namespace oop_coursework.Views
{
    public partial class AdminWindow : BaseWindow
    {
        private readonly Administrator _admin;
        private readonly DataService _dataService;

        public AdminWindow(Administrator admin, DataService dataService)
        {
            InitializeComponent();
            InitializeMenu();

            _admin = admin;
            _dataService = dataService;

            WelcomeText.Text = $"Welcome, {_admin.FullName}!";
            RoleFilter.SelectedIndex = 0;
            LoadUsers();

            UsersGrid.MouseDoubleClick += UsersGrid_MouseDoubleClick;
        }

        private void LoadUsers(string? roleFilter = null)
        {
            var users = _dataService.GetStudents()
                .Cast<User>()
                .Concat(_dataService.GetTeachers())
                .Concat(_dataService.GetAdministrators());

            if (!string.IsNullOrEmpty(roleFilter))
            {
                users = users.Where(u => u.Role == roleFilter);
            }

            UsersGrid.ItemsSource = users.ToList();
        }

        private void RoleFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoleFilter.SelectedItem is ComboBoxItem selectedItem)
            {
                string? filter = selectedItem.Content?.ToString();
                if (filter == "All Users")
                {
                    LoadUsers();
                }
                else if (filter != null)
                {
                    LoadUsers(filter.TrimEnd('s')); // Remove 's' from plural form
                }
            }
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var addUserWindow = new AddUserWindow(_dataService);
            if (addUserWindow.ShowDialog() == true)
            {
                LoadUsers();
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int userId)
            {
                var user = _dataService.GetStudents().Cast<User>()
                    .Concat(_dataService.GetTeachers())
                    .Concat(_dataService.GetAdministrators())
                    .FirstOrDefault(u => u.Id == userId);

                if (user == null) return;

                // Don't allow deleting the last administrator
                if (user is Administrator && _dataService.GetAdministrators().Count <= 1)
                {
                    MessageBox.Show("Cannot delete the last administrator account.",
                        "Delete Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                // Don't allow self-deletion
                if (user.Id == _admin.Id)
                {
                    MessageBox.Show("You cannot delete your own account.",
                        "Delete Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show(
                    $"Are you sure you want to delete {user.FullName}?\n\n" +
                    "This will also delete:\n" +
                    (user is Student ? "- All their grades\n" : "") +
                    (user is Teacher ? "- Their subjects (if not taught by other teachers)\n- All grades for their subjects\n" : ""),
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _dataService.DeleteUser(userId);
                    LoadUsers();
                    MessageBox.Show("User deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void UsersGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (UsersGrid.SelectedItem is User selectedUser)
            {
                var editWindow = new AddUserWindow(_dataService, selectedUser);
                if (editWindow.ShowDialog() == true)
                {
                    LoadUsers();
                }
            }
        }
    }
}
