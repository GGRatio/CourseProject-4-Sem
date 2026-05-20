using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Energy.Data;
using Energy.Helpers;
using Energy.Models;

namespace Energy
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void txtBoxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (txtBoxPassword.Password.Length > 0)
                txtPasswordHint.Visibility = Visibility.Collapsed;
            else
                txtPasswordHint.Visibility = Visibility.Visible;

            // Сброс подсветки
            txtBoxPassword.Background = Brushes.White;
            txtBoxPassword.ToolTip = null;
        }

        private void SwitchToRegistration_Click(object sender, RoutedEventArgs e)
        {
            var registrationWindow = new Registration();
            registrationWindow.Show();
            this.Close();
        }

        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            string login = txtBoxLogin.Text.Trim();
            string pass = txtBoxPassword.Password;

            using (var db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Login == login);

                if (user != null && PasswordHelper.HashPassword(pass) == user.PasswordHash)
                {
                    Session.CurrentUserId = user.Id;
                    Session.CurrentUserLogin = user.Login;
                    Session.CurrentUserRole = user.Role;
                    Session.CurrentUserFirstName = user.FirstName;
                    Session.CurrentUserLastName = user.LastName;

                    if (RememberMeCheckBox.IsChecked == true)
                    {
                        SessionManager.SaveUser(user.Id, user.Login, user.Role);  // ← передаём роль
                    }

                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль!");
                }
            }
        }

    }
}
