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
        private readonly AdminService _adminService;

        public AdminWindow(Administrator admin, DataService dataService)
        {
            InitializeComponent();
            InitializeMenu();

            _admin = admin;
            _dataService = dataService;
            _adminService = new AdminService(dataService);

            WelcomeText.Text = $"Вітаємо, {_admin.FullName}!";
            RoleFilter.SelectedIndex = 0;
            LoadUsers();

            UsersGrid.MouseDoubleClick += UsersGrid_MouseDoubleClick;
        }

        private void LoadUsers(string? roleFilter = null)
        {
            UsersGrid.ItemsSource = _adminService.GetAllUsers(roleFilter);
        }

        private void RoleFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoleFilter.SelectedItem is ComboBoxItem selectedItem)
            {
                string? filter = selectedItem.Content?.ToString();
                if (filter == "Всі користувачі")
                {
                    LoadUsers();
                }
                else if (filter != null)
                {
                    LoadUsers(filter switch
                    {
                        "Студенти" => "Student",
                        "Викладачі" => "Teacher",
                        "Адміністратори" => "Administrator",
                        _ => filter
                    });
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

                if (!_adminService.CanDeleteUser(user, _admin))
                {
                    MessageBox.Show("Неможливо видалити цього користувача.",
                        "Помилка видалення",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show(
                    $"Ви впевнені, що хочете видалити користувача {user.FullName}?\n\n" +
                    "Це також видалить:\n" +
                    (user is Student ? "- Всі їхні оцінки\n" : "") +
                    (user is Teacher ? "- Їхні предмети (якщо їх не викладають інші викладачі)\n- Всі оцінки з їхніх предметів\n" : ""),
                    "Підтвердження видалення",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _adminService.DeleteUser(userId);
                    LoadUsers();
                    MessageBox.Show("Користувача успішно видалено.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
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
