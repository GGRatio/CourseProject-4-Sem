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
            string pass = txtBoxPassword.Password.Trim();
            string pass2 = txtBoxPassword2.Password.Trim();
            string email = txtBoxEmail.Text.Trim();

            ResetFieldsBackground();

            // Проверки
            if (login.Length < 5)
            {
                ShowError(txtBoxLogin, "Логин не менее 5 символов!");
                txtBoxLogin.Focus();
            }
            else if (pass.Length < 8)
            {
                ShowError(txtBoxPassword, "Пароль не менее 8 символов!");
                txtBoxPassword.Focus();
            }
            else if (pass != pass2)
            {
                ShowError(txtBoxPassword2, "Пароли не совпадают!");
                txtBoxPassword2.Focus();
            }
            else if (!email.Contains("@") || !email.Contains("."))
            {
                ShowError(txtBoxEmail, "Неверный формат email!");
                txtBoxEmail.Focus();
            }
            else
            {
                MessageBox.Show("Регистрация успешна");
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


    }
}
