using Energy.Data;
using Energy.Helpers;
using Energy.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Energy.Pages.AdminPages
{
    public partial class AdminUsersPage : Page
    {
        private int _selectedId;

        public AdminUsersPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                var users = db.Users.ToList();
                dgUsers.ItemsSource = users;
                txtStatus.Text = $"Всего пользователей: {users.Count}";
            }
        }

        private void ClearForm()
        {
            _selectedId = 0;
            txtLogin.Text = "";
            txtEmail.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtPhone.Text = "";
            cbRole.SelectedIndex = 0;
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0)
            {
                MessageBox.Show("Выберите пользователя!");
                return;
            }

            if (txtLogin.Text.Length < 4)
            {
                MessageBox.Show("Логин не менее 4 символов");
                return;
            }

            using (var db = new AppDbContext())
            {
                var existingUser = db.Users.FirstOrDefault(u => u.Login == txtLogin.Text && u.Id != _selectedId);
                if (existingUser != null)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var user = db.Users.Find(_selectedId);
                if (user != null)
                {
                    string newRole = (cbRole.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "User";

                    if (user.Id == Session.CurrentUserId && user.Role == "Admin" && user.Role != newRole)
                    {
                        MessageBox.Show("Нельзя изменить роль своей учётной записи!", "Доступ запрещён",
                                        MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    user.Login = txtLogin.Text;
                    user.Email = txtEmail.Text;
                    user.FirstName = txtFirstName.Text;
                    user.LastName = txtLastName.Text;
                    user.Phone = txtPhone.Text;
                    user.Role = newRole;

                    if (!string.IsNullOrEmpty(txtPassword.Password))
                    {
                        if (txtPassword.Password.Length < 5)
                        {
                            MessageBox.Show("Пароль не менее 5 символов");
                            return;
                        }
                        user.PasswordHash = PasswordHelper.HashPassword(txtPassword.Password);
                    }

                    if (user.Role == "Trainer")
                    {
                        var trainer = db.Trainers.FirstOrDefault(t => t.FirstName == user.FirstName || t.LastName == user.LastName);
                        if (trainer != null)
                        {
                            trainer.FirstName = txtFirstName.Text;
                            trainer.LastName = txtLastName.Text;
                        }
                    }

                    db.SaveChanges();
                }
            }

            LoadData();
            ClearForm();
            MessageBox.Show("Данные пользователя обновлены!");
        }


        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0)
            {
                MessageBox.Show("Выберите пользователя!");
                return;
            }

            using (var db = new AppDbContext())
            {
                var user = db.Users.Find(_selectedId);

                if (user == null)
                {
                    MessageBox.Show("Пользователь не найден!");
                    return;
                }

                if (user.Login == Session.CurrentUserLogin)
                {
                    MessageBox.Show("Нельзя удалить свою учётную запись!");
                    return;
                }

                if (MessageBox.Show($"Удалить пользователя {user.Login}?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (user.Role == "Trainer")
                    {
                        var trainer = db.Trainers.FirstOrDefault(t => t.FirstName == user.FirstName && t.LastName == user.LastName);
                        if (trainer != null)
                        {
                            db.Trainers.Remove(trainer);
                        }
                    }

                    db.Users.Remove(user);
                    db.SaveChanges();
                    LoadData();
                    ClearForm();
                    MessageBox.Show("Пользователь удалён!");
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }


        private void Add_Click(object sender, RoutedEventArgs e)
        {

            if (txtLogin.Text.Length < 4)
            {
                MessageBox.Show("Логин не менее 4 символов");
                return;
            }
            else if (txtPassword.Password.Length < 5)
            {
                MessageBox.Show("Пароль не менее 5 символов");
                return;
            }

            using (var db = new AppDbContext())
            {
                if (string.IsNullOrEmpty(txtLogin.Text) || string.IsNullOrEmpty(txtPassword.Password))
                {
                    MessageBox.Show("Заполните Логин и Пароль!");
                    return;
                }
                if (string.IsNullOrEmpty(txtFirstName.Text) || string.IsNullOrEmpty(txtLastName.Text))
                {
                    MessageBox.Show("Заполните имя и фамилию!");
                    return;
                }
                if (db.Users.Any(u => u.Login == txtLogin.Text))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!");
                    return;
                }

                string role = (cbRole.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "User";

                var newUser = new User
                {
                    Login = txtLogin.Text,
                    PasswordHash = PasswordHelper.HashPassword(txtPassword.Password),
                    Email = txtEmail.Text,
                    FirstName = txtFirstName.Text,
                    LastName = txtLastName.Text,
                    Phone = txtPhone.Text,
                    Role = role
                };
                db.Users.Add(newUser);
                db.SaveChanges();

                if (role == "Trainer")
                {
                    var trainer = new Trainer
                    {
                        FirstName = txtFirstName.Text,
                        LastName = txtLastName.Text,
                        Specialization = "",
                        YearsOfExperience = 0,
                        Description = "",
                        PhotoUrl = ""
                    };
                    db.Trainers.Add(trainer);
                    db.SaveChanges();
                }

                MessageBox.Show("Пользователь успешно добавлен!", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                // Очищаем форму и обновляем таблицу
                ClearForm();
                LoadData();
            }
        }

        private void dgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgUsers.SelectedItem != null && dgUsers.SelectedItem is User user)
            {
                _selectedId = user.Id;
                txtLogin.Text = user.Login;
                txtEmail.Text = user.Email;
                txtFirstName.Text = user.FirstName;
                txtLastName.Text = user.LastName;
                txtPhone.Text = user.Phone;

                // Устанавливаем роль в комбобоксе
                for (int i = 0; i < cbRole.Items.Count; i++)
                {
                    var item = cbRole.Items[i] as ComboBoxItem;
                    if (item != null && item.Content.ToString() == user.Role)
                    {
                        cbRole.SelectedIndex = i;
                        break;
                    }
                }
            }
        }
    }
}