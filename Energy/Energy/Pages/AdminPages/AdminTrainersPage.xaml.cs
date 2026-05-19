using Energy.Data;
using Energy.Helpers;
using Energy.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Path = System.IO.Path;
using File = System.IO.File;
using Directory = System.IO.Directory;

namespace Energy.Pages.AdminPages
{
    public partial class AdminTrainersPage : Page
    {
        private int _selectedId;

        public AdminTrainersPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                var list = db.Trainers.ToList();
                dgTrainers.ItemsSource = list;
                txtStatus.Text = $"Всего тренеров: {list.Count}";
            }
        }

        private void ClearForm()
        {
            _selectedId = 0;
            txtLogin.Text = "";
            txtPassword.Password = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtEmail.Text = "";
            txtSpecialization.Text = "";
            txtExperience.Text = "";
            txtPhotoPath.Text = "";
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // Проверки
            if (string.IsNullOrEmpty(txtLogin.Text))
            {
                MessageBox.Show("Введите логин!");
                return;
            }
            if (string.IsNullOrEmpty(txtPassword.Password))
            {
                MessageBox.Show("Введите пароль!");
                return;
            }
            if (string.IsNullOrEmpty(txtFirstName.Text) || string.IsNullOrEmpty(txtLastName.Text))
            {
                MessageBox.Show("Введите имя и фамилию!");
                return;
            }
            if (!int.TryParse(txtExperience.Text, out int experience))
            {
                MessageBox.Show("Введите корректный опыт!");
                return;
            }

            using (var db = new AppDbContext())
            {
                // Проверяем, нет ли такого логина
                if (db.Users.Any(u => u.Login == txtLogin.Text))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!");
                    return;
                }

                // 1. Создаём аккаунт пользователя с ролью Trainer
                var user = new User
                {
                    Login = txtLogin.Text,
                    PasswordHash = PasswordHelper.HashPassword(txtPassword.Password),
                    Email = txtEmail.Text,
                    FirstName = txtFirstName.Text,
                    LastName = txtLastName.Text,
                    Role = "Trainer"
                };
                db.Users.Add(user);
                db.SaveChanges();

                // 2. Создаём профиль тренера
                var trainer = new Trainer
                {
                    FirstName = txtFirstName.Text,
                    LastName = txtLastName.Text,
                    Specialization = txtSpecialization.Text,
                    YearsOfExperience = experience,
                    PhotoUrl = txtPhotoPath.Text,
                    Description = $"{txtSpecialization.Text}, опыт {experience} лет"
                };
                db.Trainers.Add(trainer);
                db.SaveChanges();
            }

            LoadData();
            ClearForm();
            MessageBox.Show("Тренер создан!");
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0)
            {
                MessageBox.Show("Выберите тренера!");
                return;
            }

            if (!int.TryParse(txtExperience.Text, out int experience))
            {
                MessageBox.Show("Введите корректный опыт!");
                return;
            }

            using (var db = new AppDbContext())
            {
                var trainer = db.Trainers.Find(_selectedId);
                if (trainer != null)
                {
                    trainer.FirstName = txtFirstName.Text;
                    trainer.LastName = txtLastName.Text;
                    trainer.Specialization = txtSpecialization.Text;
                    trainer.YearsOfExperience = experience;
                    trainer.PhotoUrl = txtPhotoPath.Text;
                    trainer.Description = $"{txtSpecialization.Text}, опыт {experience} лет";
                    db.SaveChanges();
                }

                // Обновляем данные пользователя
                var user = db.Users.FirstOrDefault(u => u.FirstName == trainer.FirstName && u.LastName == trainer.LastName);
                if (user != null)
                {
                    user.Email = txtEmail.Text;
                    db.SaveChanges();
                }
            }

            LoadData();
            ClearForm();
            MessageBox.Show("Данные тренера обновлены!");
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0)
            {
                MessageBox.Show("Выберите тренера!");
                return;
            }

            if (MessageBox.Show("Удалить тренера?", "Подтверждение",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    var trainer = db.Trainers.Find(_selectedId);
                    if (trainer != null)
                    {
                        // Удаляем пользователя с ролью Trainer
                        var user = db.Users.FirstOrDefault(u => u.FirstName == trainer.FirstName && u.LastName == trainer.LastName && u.Role == "Trainer");
                        if (user != null)
                            db.Users.Remove(user);

                        db.Trainers.Remove(trainer);
                        db.SaveChanges();
                    }
                }
                LoadData();
                ClearForm();
                MessageBox.Show("Тренер удалён!");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void dgTrainers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTrainers.SelectedItem != null && dgTrainers.SelectedItem is Trainer item)
            {
                _selectedId = item.Id;
                txtFirstName.Text = item.FirstName;
                txtLastName.Text = item.LastName;
                txtSpecialization.Text = item.Specialization;
                txtExperience.Text = item.YearsOfExperience.ToString();
                txtPhotoPath.Text = item.PhotoUrl;

                // Пытаемся найти email пользователя
                using (var db = new AppDbContext())
                {
                    var user = db.Users.FirstOrDefault(u => u.FirstName == item.FirstName && u.LastName == item.LastName && u.Role == "Trainer");
                    if (user != null)
                    {
                        txtLogin.Text = user.Login;
                        txtEmail.Text = user.Email;
                    }
                }
            }
        }



        private void SelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*";
            dialog.Title = "Выберите фото тренера";

            if (dialog.ShowDialog() == true)
            {
                // Создаём папку Images в папке приложения (рядом с EXE)
                string appFolder = AppDomain.CurrentDomain.BaseDirectory;
                string imagesFolder = System.IO.Path.Combine(appFolder, "Images");

                if (!System.IO.Directory.Exists(imagesFolder))
                    System.IO.Directory.CreateDirectory(imagesFolder);

                // Генерируем уникальное имя для фото
                string fileName = $"trainer_{DateTime.Now.Ticks}{System.IO.Path.GetExtension(dialog.FileName)}";
                string destPath = System.IO.Path.Combine(imagesFolder, fileName);

                // Копируем фото
                System.IO.File.Copy(dialog.FileName, destPath, true);

                // Сохраняем относительный путь (относительно папки приложения)
                txtPhotoPath.Text = $"Images/{fileName}";

                // Показываем превью
                ShowPhotoPreview(destPath);

                MessageBox.Show("Фото загружено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ShowPhotoPreview(string photoPath)
        {
            try
            {
                var preview = new Image
                {
                    Width = 80,
                    Height = 80,
                    Stretch = Stretch.UniformToFill,
                    Margin = new Thickness(10, 0, 0, 0)
                };

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(photoPath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                preview.Source = bitmap;

                var parent = (txtPhotoPath.Parent as StackPanel);
                if (parent != null)
                {
                    var oldPreview = parent.Children.OfType<Image>().FirstOrDefault();
                    if (oldPreview != null)
                        parent.Children.Remove(oldPreview);

                    parent.Children.Add(preview);
                }
            }
            catch { }
        }


    }
}