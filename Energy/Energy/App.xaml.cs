using System.Configuration;
using System.Data;
using System.Windows;
    
using Energy.Helpers;


namespace Energy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Проверяем, есть ли сохранённая сессия
            var savedUser = SessionManager.LoadUser();

            if (savedUser != null)
            {
                // Автоматический вход
                Session.CurrentUserId = savedUser.UserId;
                Session.CurrentUserLogin = savedUser.Login;


                var mainWindow = new MainWindow();
                mainWindow.Show();

            }
            else
            {
                // Показываем окно входа
                var loginWindow = new Login();
                loginWindow.Show();
            }
        }

    }

}

