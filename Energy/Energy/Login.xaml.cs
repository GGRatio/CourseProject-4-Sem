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

            ResetFieldsBackground();

            if (string.IsNullOrEmpty(login))
            {
                ShowError(txtBoxLogin, "Введите логин");
                return;
            }

            if (string.IsNullOrEmpty(pass))
            {
                ShowError(txtBoxPassword, "Введите пароль");
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    //Пытаемся найти пользователя с таким логином
                    var user = db.Users.FirstOrDefault(u => u.Login == login);

                    if (user == null)
                    {
                        ShowError(txtBoxLogin, "Неверный логин");   
                        return;
                    }

                    if (PasswordHelper.HashPassword(pass) == user.PasswordHash)
                    {
                        if(RememberMeCheckBox.IsChecked == true)
                        {
                            SessionManager.SaveUser(user.Id, user.Login);
                        }

                        Session.CurrentUserLogin = user.Login;
                        Session.CurrentUserId = user.Id;



                        var mainWindow = new MainWindow();
                        mainWindow.Show();
                        Application.Current.MainWindow = mainWindow;
                        this.Close();
                    }
                    else
                    {
                        ShowError(txtBoxLogin, "Неверный пароль");
                        return;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка БД: {ex.Message}");
            }
        }

        private void ShowError(Control control, string message)
        {
            control.ToolTip = message;
            control.Background = new SolidColorBrush(Colors.LightPink);

            // Сброс через 3 секунды
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, args) =>
            {
                control.Background = Brushes.White;
                control.ToolTip = null;
                timer.Stop();
            };
            timer.Start();
        }

        private void ResetFieldsBackground()
        {
            txtBoxLogin.Background = Brushes.White;
            txtBoxPassword.Background = Brushes.White;
        }
    }
}
