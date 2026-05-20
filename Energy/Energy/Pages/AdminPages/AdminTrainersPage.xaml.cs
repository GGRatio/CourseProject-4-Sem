using Energy.Data;
using Energy.Helpers;
using Energy.Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtSpecialization.Text = "";
            txtExperience.Text = "";
            txtPhotoPath.Text = "";
            txtDescription.Text = "";
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // Проверки
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
                // Создаём тренера (без User, только данные тренера)
                var trainer = new Trainer
                {
                    FirstName = txtFirstName.Text,
                    LastName = txtLastName.Text,
                    Specialization = txtSpecialization.Text,
                    YearsOfExperience = experience,
                    PhotoUrl = txtPhotoPath.Text,
                    Description = txtDescription.Text
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
                    trainer.Specialization = txtSpecialization.Text;
                    trainer.YearsOfExperience = experience;
                    trainer.PhotoUrl = txtPhotoPath.Text;
                    trainer.Description = txtDescription.Text;

                    // СИНХРОНИЗАЦИЯ: обновляем имя/фамилию в Users
                    var user = db.Users.FirstOrDefault(u => u.Role == "Trainer" &&
                                                            (u.FirstName == trainer.FirstName || u.LastName == trainer.LastName));
                    if (user != null)
                    {
                        user.FirstName = txtFirstName.Text;
                        user.LastName = txtLastName.Text;
                    }

                    // Также обновляем поля FirstName/LastName в самом тренере
                    trainer.FirstName = txtFirstName.Text;
                    trainer.LastName = txtLastName.Text;

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
            if (dgTrainers.SelectedItem != null && dgTrainers.SelectedItem is Trainer trainer)
            {
                _selectedId = trainer.Id;

                // Показываем актуальные данные из тренера
                txtFirstName.Text = trainer.FirstName;
                txtLastName.Text = trainer.LastName;
                txtSpecialization.Text = trainer.Specialization;
                txtExperience.Text = trainer.YearsOfExperience.ToString();
                txtPhotoPath.Text = trainer.PhotoUrl;
                txtDescription.Text = trainer.Description;
            }
        }

        private void SelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*";
            dialog.Title = "Выберите фото тренера";

            if (dialog.ShowDialog() == true)
            {
                string appFolder = AppDomain.CurrentDomain.BaseDirectory;
                string imagesFolder = System.IO.Path.Combine(appFolder, "Images");

                if (!System.IO.Directory.Exists(imagesFolder))
                    System.IO.Directory.CreateDirectory(imagesFolder);

                string fileName = $"trainer_{DateTime.Now.Ticks}{System.IO.Path.GetExtension(dialog.FileName)}";
                string destPath = System.IO.Path.Combine(imagesFolder, fileName);

                System.IO.File.Copy(dialog.FileName, destPath, true);

                txtPhotoPath.Text = $"Images/{fileName}";
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