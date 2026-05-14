    
using Energy.Helpers;
using Energy.Models;
using System.Configuration;
using System.Data;
using System.Windows;


namespace Energy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //
        public static UndoRedoManager<PurchaseAction> PurchaseManager { get; set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            //
            PurchaseManager = new UndoRedoManager<PurchaseAction>();

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

