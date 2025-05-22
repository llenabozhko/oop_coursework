using System.Windows;
using oop_coursework.Models;
using oop_coursework.Services;

namespace oop_coursework.Views
{
    public partial class LoginWindow : Window
    {
        private readonly DataService _dataService;

        public LoginWindow()
        {
            InitializeComponent();
            _dataService = new DataService();

            // Create default admin if no users exist
            if (_dataService.GetAdministrators().Count == 0)
            {
                var admin = new Administrator
                {
                    Username = "admin",
                    Password = "admin",
                    FirstName = "Admin",
                    LastName = "User",
                    Role = "Administrator"
                };
                _dataService.AddUser(admin);
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password;

            var user = _dataService.GetUserByCredentials(username, password);

            if (user == null)
            {
                MessageBox.Show("Invalid username or password!", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Window mainWindow;
            switch (user)
            {
                case Student student:
                    mainWindow = new StudentWindow(student, _dataService);
                    break;
                case Teacher teacher:
                    mainWindow = new TeacherWindow(teacher, _dataService);
                    break;
                case Administrator admin:
                    mainWindow = new AdminWindow(admin, _dataService);
                    break;
                default:
                    MessageBox.Show("Invalid user type!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }

            mainWindow.Show();
            this.Close();
        }
    }
}
