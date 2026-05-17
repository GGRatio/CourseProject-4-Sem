using Energy.Data;
using Energy.Helpers;
using Energy.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                MinHeight = 220,
                Margin = new Thickness(15),
                Background = (SolidColorBrush)FindResource("CardBackgroundBrush"),
                BorderBrush = (SolidColorBrush)FindResource("BorderBrush"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(12)
            };

            var stack = new StackPanel();
            stack.Margin = new Thickness(15);

            // Имя и фамилия
            stack.Children.Add(new TextBlock
            {
                Text = $"{trainer.FirstName} {trainer.LastName}",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = (SolidColorBrush)FindResource("TextPrimaryBrush")
            });

            // Специализация
            stack.Children.Add(new TextBlock
            {
                Text = trainer.Specialization,
                FontSize = 13,
                Foreground = (SolidColorBrush)FindResource("TextSecondaryBrush"),
                Margin = new Thickness(0, 5, 0, 0)
            });

            // Опыт
            stack.Children.Add(new TextBlock
            {
                Text = $"Опыт: {trainer.YearsOfExperience} лет",
                FontSize = 12,
                Foreground = (SolidColorBrush)FindResource("TextSecondaryBrush"),
                Margin = new Thickness(0, 5, 0, 0)
            });

            // Описание
            stack.Children.Add(new TextBlock
            {
                Text = trainer.Description,
                FontSize = 12,
                Foreground = (SolidColorBrush)FindResource("TextSecondaryBrush"),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 10, 0, 0),
                MaxHeight = 60
            });

            // Кнопка выбора тренера
            bool isSelected = _selectedTrainerId == trainer.Id;
            var selectButton = new Button
            {
                Content = isSelected ? "✅ Мой тренер" : "📝 Выбрать тренера",
                Tag = trainer.Id,
                Margin = new Thickness(0, 15, 0, 0),
                Padding = new Thickness(10, 8, 10, 8),
                Cursor = System.Windows.Input.Cursors.Hand,
                FontSize = 13
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

            stack.Children.Add(selectButton);

            border.Child = stack;
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
    }
}
