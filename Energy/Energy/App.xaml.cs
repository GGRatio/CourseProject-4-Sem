using System.Windows;
using Energy.Data;
using Energy.Helpers;
using Energy.Models;

namespace Energy
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var savedUser = SessionManager.LoadUser();

            if (savedUser != null)
            {
                try
                {
                    using (var db = new AppDbContext())
                    {
                        var user = db.Users.Find(savedUser.UserId);
                        if (user != null)
                        {
                            Session.CurrentUserId = user.Id;
                            Session.CurrentUserLogin = user.Login;
                            Session.CurrentUserRole = user.Role;
                            Session.CurrentUserFirstName = user.FirstName;
                            Session.CurrentUserLastName = user.LastName;
                        }
                        else
                        {
                            // Пользователь не найден в БД — очищаем сессию
                            SessionManager.ClearSession();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки: {ex.Message}");
                    SessionManager.ClearSession();
                }
            }

            // Проверяем, есть ли активная сессия
            if (Session.CurrentUserId > 0)
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                var loginWindow = new Login();
                loginWindow.Show();
            }
        }
    }
}