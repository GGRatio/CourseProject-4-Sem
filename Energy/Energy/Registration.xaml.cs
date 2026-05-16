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


        // Скрываем подсказку для пароля когда есть текст
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

        private void txtBoxPassword2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (txtBoxPassword2.Password.Length > 0)
                txtPasswordHint2.Visibility = Visibility.Collapsed;
            else
                txtPasswordHint2.Visibility = Visibility.Visible;

            // Сброс подсветки
            txtBoxPassword2.Background = Brushes.White;
            txtBoxPassword2.ToolTip = null;
        }


        private void Button_Reg_Click(object sender, RoutedEventArgs e)
        {
            string login = txtBoxLogin.Text.Trim();
            string pass = txtBoxPassword.Password;
            string pass2 = txtBoxPassword2.Password;
            string email = txtBoxEmail.Text.Trim();

            if (login.Length < 3)
            {
                MessageBox.Show("Логин не менее 3 символов!");
                return;
            }
            if (pass.Length < 4)
            {
                MessageBox.Show("Пароль не менее 4 символов!");
                return;
            }
            if (pass != pass2)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    // Создаём пользователя ТОЛЬКО с обязательными полями
                    var newUser = new User
                    {
                        Login = login,
                        PasswordHash = PasswordHelper.HashPassword(pass),
                        Email = email,
                        FirstName = "",      
                        LastName = "",       
                        Phone = "",          
                        Role = "User"        
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges();

                    MessageBox.Show("Регистрация успешна!");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                if (ex.InnerException != null)
                    MessageBox.Show($"Детали: {ex.InnerException.Message}");
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
    }
}
