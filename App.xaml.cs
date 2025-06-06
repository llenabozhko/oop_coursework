using System.Configuration;
using System.Data;
using System.Windows;
using oop_coursework.Views;

namespace oop_coursework
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}
