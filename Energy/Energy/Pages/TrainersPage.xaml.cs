using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using Energy.Data;
using Energy.Models;
using Energy.Helpers;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

using System.Windows.Navigation;
using System.Windows.Shapes;

using Path = System.IO.Path;
using File = System.IO.File;
using Directory = System.IO.Directory;

namespace Energy.Pages
{
    public partial class TrainersPage : Page
    {
        private int? _selectedTrainerId;

        public TrainersPage()
        {
            InitializeComponent();
            LoadTrainers();
            LoadSelectedTrainer();
        }

        private void LoadTrainers()
        {
            using (var db = new AppDbContext())
            {
                var trainers = db.Trainers.ToList();
                TrainersPanel.Children.Clear();

                foreach (var trainer in trainers)
                {
                    var card = CreateTrainerCard(trainer);
                    TrainersPanel.Children.Add(card);
                }
            }
        }

        private Border CreateTrainerCard(Trainer trainer)
        {
            var border = new Border
            {
                Width = 280,
                MinHeight = 350,
                Margin = new Thickness(15),
                Background = (SolidColorBrush)FindResource("CardBackgroundBrush"),
                BorderBrush = (SolidColorBrush)FindResource("BorderBrush"),
                BorderThickness = new Thickness(0),
                CornerRadius = new CornerRadius(15)
            };

            border.Effect = new DropShadowEffect
            {
                BlurRadius = 12,
                ShadowDepth = 3,
                Opacity = 0.15,
                Color = Colors.Black
            };

            var mainStack = new StackPanel();

            // Фото (верхняя часть)
            var photoBorder = new Border
            {
                Height = 220,
                CornerRadius = new CornerRadius(15, 15, 0, 0),
                Background = (SolidColorBrush)FindResource("BorderBrush"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top
            };

            try
            {
                if (!string.IsNullOrEmpty(trainer.PhotoUrl))
                {
                    var photo = new Image
                    {
                        Stretch = Stretch.UniformToFill,
                        VerticalAlignment = VerticalAlignment.Top
                    };

                    string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, trainer.PhotoUrl);

                    if (File.Exists(fullPath))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(fullPath);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        photo.Source = bitmap;
                        photoBorder.Child = photo;
                    }
                    else
                    {
                        throw new Exception("Фото не найдено");
                    }
                }
                else
                {
                    throw new Exception("Нет пути к фото");
                }
            }
            catch
            {
                photoBorder.Background = new SolidColorBrush(Colors.LightGray);
                var noPhoto = new TextBlock
                {
                    Text = "🏋️",
                    FontSize = 48,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                photoBorder.Child = noPhoto;
            }

            mainStack.Children.Add(photoBorder);

            // Текстовая часть (нижняя)
            var textStack = new StackPanel
            {
                Margin = new Thickness(15),
                VerticalAlignment = VerticalAlignment.Bottom
            };

            // Имя
            textStack.Children.Add(new TextBlock
            {
                Text = $"{trainer.FirstName} {trainer.LastName}",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = (SolidColorBrush)FindResource("TextPrimaryBrush"),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 5)
            });

            // Специализация
            textStack.Children.Add(new TextBlock
            {
                Text = trainer.Specialization,
                FontSize = 13,
                Foreground = (SolidColorBrush)FindResource("TextSecondaryBrush"),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 5)
            });

            // Опыт
            textStack.Children.Add(new TextBlock
            {
                Text = $"Опыт: {trainer.YearsOfExperience} лет",
                FontSize = 12,
                Foreground = (SolidColorBrush)FindResource("TextSecondaryBrush"),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 8)
            });

            // Описание
            var descText = new TextBlock
            {
                Text = trainer.Description,
                FontSize = 11,
                Foreground = (SolidColorBrush)FindResource("TextSecondaryBrush"),
                TextWrapping = TextWrapping.Wrap,
                MaxHeight = 40,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 12)
            };
            textStack.Children.Add(descText);

            // Кнопка выбора тренера
            bool isSelected = _selectedTrainerId == trainer.Id;
            var selectButton = new Button
            {
                Content = isSelected ? "✅ Мой тренер" : "📝 Выбрать тренера",
                Tag = trainer.Id,
                Height = 36,
                Cursor = System.Windows.Input.Cursors.Hand,
                FontSize = 13,
            };

            if (isSelected)
            {
                selectButton.Background = (SolidColorBrush)FindResource("ButtonSuccessBrush");
                selectButton.Foreground = (SolidColorBrush)FindResource("TextPrimaryBrush");
                selectButton.IsEnabled = false;
            }
            else
            {
                selectButton.Background = (SolidColorBrush)FindResource("ButtonPrimaryBrush");
                selectButton.Foreground = (SolidColorBrush)FindResource("TextPrimaryBrush");
                selectButton.Click += (s, e) => SelectTrainer(trainer.Id, $"{trainer.FirstName} {trainer.LastName}");
            }

            textStack.Children.Add(selectButton);
            mainStack.Children.Add(textStack);


            border.Child = mainStack;
            border.Cursor = System.Windows.Input.Cursors.Hand;
            border.MouseLeftButtonUp += (s, e) => GoToTrainerProfile(trainer.Id);
            return border;
        }

        private void LoadSelectedTrainer()
        {
            using (var db = new AppDbContext())
            {
                var userTrainer = db.UserTrainers
                    .FirstOrDefault(ut => ut.UserId == Session.CurrentUserId);

                if (userTrainer != null)
                {
                    _selectedTrainerId = userTrainer.TrainerId;
                }
            }
        }

        private void SelectTrainer(int trainerId, string trainerName)
        {
            using (var db = new AppDbContext())
            {
                // Удаляем старого тренера если был
                var old = db.UserTrainers.FirstOrDefault(ut => ut.UserId == Session.CurrentUserId);
                if (old != null)
                {
                    db.UserTrainers.Remove(old);
                }

                // Добавляем нового
                var userTrainer = new UserTrainer
                {
                    UserId = Session.CurrentUserId,
                    TrainerId = trainerId,
                    SelectedDate = DateTime.Now
                };

                db.UserTrainers.Add(userTrainer);
                db.SaveChanges();
            }

            _selectedTrainerId = trainerId;
            MessageBox.Show($"Теперь {trainerName} ваш тренер!", "Тренер выбран",
                            MessageBoxButton.OK, MessageBoxImage.Information);

            // Обновляем страницу
            LoadTrainers();
        }


        private void GoToTrainerProfile(int trainerId)
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mainWindow?.NavigateTo(new TrainerProfilePage(trainerId));
        }
    }
}
