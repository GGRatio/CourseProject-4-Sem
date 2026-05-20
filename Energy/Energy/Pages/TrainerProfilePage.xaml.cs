using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Energy.Data;
using Energy.Models;
using Energy.Helpers;

namespace Energy.Pages
{
    public partial class TrainerProfilePage : Page
    {
        private int _trainerId;

        public TrainerProfilePage(int trainerId)
        {
            InitializeComponent();
            _trainerId = trainerId;
            LoadTrainerData();
            LoadReviews();
        }

        private void LoadTrainerData()
        {
            using (var db = new AppDbContext())
            {
                var trainer = db.Trainers.Find(_trainerId);
                if (trainer != null)
                {
                    txtName.Text = $"{trainer.FirstName} {trainer.LastName}";
                    txtSpecialization.Text = trainer.Specialization;
                    txtExperience.Text = $"{trainer.YearsOfExperience} лет";
                    txtDescription.Text = trainer.Description;
                    LoadPhoto(trainer.PhotoUrl);
                }
            }
        }

        private void LoadReviews()
        {
            using (var db = new AppDbContext())
            {
                var reviews = db.Reviews
                    .Where(r => r.TrainerId == _trainerId)
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new
                    {
                        r.Comment,
                        r.CreatedAt,
                        UserName = db.Users.Where(u => u.Id == r.UserId).Select(u => u.Login).FirstOrDefault()
                    })
                    .ToList();

                lstReviews.ItemsSource = reviews;
            }
        }

        private void LoadPhoto(string photoPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(photoPath))
                {
                    string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, photoPath);
                    if (File.Exists(fullPath))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(fullPath);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        imgPhoto.Source = bitmap;
                    }
                }
            }
            catch { }
        }

        private void SendReview_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtComment.Text))
            {
                MessageBox.Show("Напишите отзыв!");
                return;
            }

            using (var db = new AppDbContext())
            {
                var review = new Review
                {
                    UserId = Session.CurrentUserId,
                    TrainerId = _trainerId,
                    Comment = txtComment.Text.Trim(),
                    CreatedAt = DateTime.Now
                };
                db.Reviews.Add(review);
                db.SaveChanges();
            }

            txtComment.Text = "";
            LoadReviews();

            MessageBox.Show("Спасибо за отзыв!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mainWindow?.NavigateTo(new TrainersPage());
        }
    }
}