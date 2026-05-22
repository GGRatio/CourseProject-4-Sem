using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Energy.Data;
using Energy.Helpers;
using Energy.Models;

namespace Energy
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void HighlightError(Control control, bool isError)
        {
            if (isError)
            {
                control.Background = new SolidColorBrush(Colors.LightPink);
                control.BorderBrush = new SolidColorBrush(Colors.Red);
                control.BorderThickness = new Thickness(1.5);

                var timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(3);
                timer.Tick += (s, args) =>
                {
                    control.Background = Brushes.White;
                    control.BorderBrush = (SolidColorBrush)FindResource("BorderBrush");
                    control.BorderThickness = new Thickness(1);
                    timer.Stop();
                };
                timer.Start();
            }
            else
            {
                control.Background = Brushes.White;
                control.BorderBrush = (SolidColorBrush)FindResource("BorderBrush");
                control.BorderThickness = new Thickness(1);
            }
        }

        private void ShowError(Control control, string message)
        {
            control.ToolTip = message;
            HighlightError(control, true);

            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, args) =>
            {
                control.ToolTip = null;
                timer.Stop();
            };
            timer.Start();
        }

        private void txtBoxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (txtBoxPassword.Password.Length > 0)
                txtPasswordHint.Visibility = Visibility.Collapsed;
            else
                txtPasswordHint.Visibility = Visibility.Visible;

            HighlightError(txtBoxPassword, false);
            txtBoxPassword.ToolTip = null;
        }

        private void txtBoxLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            HighlightError(txtBoxLogin, false);
            txtBoxLogin.ToolTip = null;
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

            HighlightError(txtBoxLogin, false);
            HighlightError(txtBoxPassword, false);

            if (string.IsNullOrEmpty(login))
            {
                ShowError(txtBoxLogin, "Введите логин!");
                txtBoxLogin.Focus();
                return;
            }
            if (string.IsNullOrEmpty(pass))
            {
                ShowError(txtBoxPassword, "Введите пароль!");
                txtBoxPassword.Focus();
                return;
            }

            using (var db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Login == login);

                if (user == null)
                {
                    ShowError(txtBoxLogin, "Неверный логин или пароль");
                    ShowError(txtBoxPassword, "Неверный логин или пароль");

                    return;
                }

                if (PasswordHelper.HashPassword(pass) != user.PasswordHash)
                {
                    ShowError(txtBoxPassword, "Неверный пароль!");
                    txtBoxPassword.Focus();
                    return;
                }



                Session.CurrentUserId = user.Id;
                Session.CurrentUserLogin = user.Login;
                Session.CurrentUserRole = user.Role;
                Session.CurrentUserFirstName = user.FirstName;
                Session.CurrentUserLastName = user.LastName;

                if (RememberMeCheckBox.IsChecked == true)
                {
                    SessionManager.SaveUser(user.Id, user.Login, user.Role);
                }

                var mainWindow = new MainWindow(user.Role);
                mainWindow.Show();
                this.Close();
            }
        }
    }
}