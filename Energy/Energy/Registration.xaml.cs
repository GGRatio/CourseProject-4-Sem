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
using Energy.Models;
using Energy.Helpers;

namespace Energy
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }


        private void txtBoxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (txtBoxPassword.Password.Length > 0)
                txtPasswordHint.Visibility = Visibility.Collapsed;
            else
                txtPasswordHint.Visibility = Visibility.Visible;

            txtBoxPassword.Background = Brushes.White;
            txtBoxPassword.ToolTip = null;
        }

        private void txtBoxPassword2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (txtBoxPassword2.Password.Length > 0)
                txtPasswordHint2.Visibility = Visibility.Collapsed;
            else
                txtPasswordHint2.Visibility = Visibility.Visible;

            txtBoxPassword2.Background = Brushes.White;
            txtBoxPassword2.ToolTip = null;
        }


        private void Button_Reg_Click(object sender, RoutedEventArgs e)
        {

            string login = txtBoxLogin.Text.Trim();
            string pass = txtBoxPassword.Password;
            string pass2 = txtBoxPassword2.Password;
            string email = txtBoxEmail.Text.Trim();

            ResetFieldsBackground();

            if (login.Length < 4)
            {
                ShowError(txtBoxLogin, "Логин не менее 4 символов!");
                txtBoxLogin.Focus();
                return;
            }
            else if (pass.Length < 5)
            {
                ShowError(txtBoxPassword, "Пароль не менее 5 символов!");
                txtBoxPassword.Focus();
                return;
            }
            else if (pass != pass2)
            {
                ShowError(txtBoxPassword2, "Пароли не совпадают!");
                txtBoxPassword2.Focus();
                return;
            }
            else if (!IsValidEmail(email))
            {
                ShowError(txtBoxEmail, "Неверный формат email!");
                txtBoxEmail.Focus();
                return;
            }
            else
            {
                try
                {
                    using (var db = new AppDbContext())
                    {
                        if (db.Users.Any(u => u.Login == login))
                        {
                            ShowError(txtBoxLogin, "Пользователь с таким логином уже существует!");
                            return;
                        }
                        if (db.Users.Any(u => u.Email == email))
                        {
                            ShowError(txtBoxEmail, "Пользователь с таким email уже существует!");
                            return;
                        }

                        //Создаем нового пользователя
                        var newUser = new User
                        {
                            Login = login,
                            PasswordHash = PasswordHelper.HashPassword(pass),
                            Email = email
                        };

                        db.Users.Add(newUser);
                        db.SaveChanges();

                        MessageBox.Show("Регистрация успешна!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);


                        var loginWindow = new Login();
                        loginWindow.Show();

                        loginWindow.txtBoxLogin.Text = login;
                        loginWindow.txtBoxPassword.Password = pass;

                        this.Close();
                        

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
            txtBoxPassword2.Background = Brushes.White;
            txtBoxEmail.Background = Brushes.White;
        }

        private void SwitchToLogin_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }


        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
