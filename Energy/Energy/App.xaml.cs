
using System.Windows;
using Energy.Data;
using Energy.Helpers;


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
                Session.CurrentUserId = savedUser.UserId;
                Session.CurrentUserLogin = savedUser.Login;
                Session.CurrentUserRole = savedUser.Role;

                // Загружаем имя и фамилию из БД
                using (var db = new AppDbContext())
                {
                    var user = db.Users.Find(savedUser.UserId);
                    if (user != null)
                    {
                        Session.CurrentUserFirstName = user.FirstName;
                        Session.CurrentUserLastName = user.LastName;
                    }
                }

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