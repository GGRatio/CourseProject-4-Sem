using Energy.Data;
using Energy.Helpers;
using Energy.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace Energy.Pages
{
    public partial class GroupClassesPage : Page
    {
        private bool _showCurrent = true;

        public GroupClassesPage()
        {
            InitializeComponent();
            LoadClasses();
        }

        private void LoadClasses()
        {
            using (var db = new AppDbContext())
            {
                ClassesPanel.Children.Clear();

                var today = DateTime.Now;

                var classes = _showCurrent
                    ? db.GroupClasses.Where(c => c.ClassDate >= today).OrderBy(c => c.ClassDate).ToList()
                    : db.GroupClasses.Where(c => c.ClassDate < today).OrderByDescending(c => c.ClassDate).ToList();

                foreach (var classItem in classes)
                {
                    var card = CreateClassCard(classItem);
                    ClassesPanel.Children.Add(card);
                }

                if (classes.Count == 0)
                {
                    var emptyText = new TextBlock
                    {
                        Text = _showCurrent ? "Нет текущих занятий" : "Нет истории занятий",
                        FontSize = 16,
                        Foreground = (SolidColorBrush)FindResource("TextSecondaryBrush"),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 50, 0, 0)
                    };
                    ClassesPanel.Children.Add(emptyText);
                }
            }
        }

        private bool IsUserRegistered(int classId)
        {
            using (var db = new AppDbContext())
            {
                return db.ClassRegistrations
                    .Any(r => r.UserId == Session.CurrentUserId &&
                              r.GroupClassId == classId &&
                              !r.IsCanceled);
            }
        }

        private ClassRegistration GetUserRegistration(int classId)
        {
            using (var db = new AppDbContext())
            {
                return db.ClassRegistrations
                    .FirstOrDefault(r => r.UserId == Session.CurrentUserId && r.GroupClassId == classId);
            }
        }

        private void RegisterForClass(int classId)
        {
            using (var db = new AppDbContext())
            {
                var classItem = db.GroupClasses.Find(classId);

                if (classItem == null)
                {
                    MessageBox.Show("Занятие не найдено!");
                    return;
                }

                // Нельзя записаться на прошедшее
                if (classItem.ClassDate <= DateTime.Now)
                {
                    MessageBox.Show("Нельзя записаться на прошедшее занятие!");
                    return;
                }

                if (classItem.CurrentParticipants >= classItem.MaxParticipants)
                {
                    MessageBox.Show("Нет свободных мест!");
                    return;
                }

                var registration = new ClassRegistration
                {
                    UserId = Session.CurrentUserId,
                    GroupClassId = classId,
                    RegistrationDate = DateTime.Now,
                    IsAttended = false,
                    IsCanceled = false
                };

                db.ClassRegistrations.Add(registration);
                classItem.CurrentParticipants++;
                db.SaveChanges();
            }

            LoadClasses();
            MessageBox.Show("Вы записаны на занятие!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private Border CreateClassCard(GroupClass classItem)
        {
            var border = new Border
            {
                Width = 320,
                Margin = new Thickness(15),
                Background = (SolidColorBrush)FindResource("CardBackgroundBrush"),
                BorderBrush = (SolidColorBrush)FindResource("BorderBrush"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(12)
            };

            border.Effect = new DropShadowEffect
            {
                BlurRadius = 12,
                ShadowDepth = 2,
                Opacity = 0.18,
                Color = ((SolidColorBrush)FindResource("ShadowColorBrush")).Color
            };

            var stack = new StackPanel();
            stack.Margin = new Thickness(15);

            // Название
            stack.Children.Add(new TextBlock
            {
                Text = classItem.Name,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = (SolidColorBrush)FindResource("TextPrimaryBrush"),
                Margin = new Thickness(0, 0, 0, 5)
            });

            // Тренер
            stack.Children.Add(new TextBlock
            {
                Text = $"👨‍🏫 {classItem.Instructor}",
                FontSize = 13,
                Foreground = (SolidColorBrush)FindResource("TextSecondaryBrush"),
                Margin = new Thickness(0, 0, 0, 5)
            });

            // Дата и время
            stack.Children.Add(new TextBlock
            {
                Text = $"📅 {classItem.ClassDate:dd.MM.yyyy} в {classItem.ClassDate:HH:mm}",
                FontSize = 13,
                Foreground = (SolidColorBrush)FindResource("TextSecondaryBrush"),
                Margin = new Thickness(0, 0, 0, 5)
            });

            // Длительность
            stack.Children.Add(new TextBlock
            {
                Text = $"⏱️ {classItem.DurationMinutes} минут",
                FontSize = 13,
                Foreground = (SolidColorBrush)FindResource("TextSecondaryBrush"),
                Margin = new Thickness(0, 0, 0, 5)
            });

            // Места
            int freeSpots = classItem.MaxParticipants - classItem.CurrentParticipants;
            var spotsText = freeSpots > 0 ? $"🎟️ Свободно мест: {freeSpots}" : "❌ Мест нет";
            SolidColorBrush spotsBrush;

            if (freeSpots > 5)
                spotsBrush = (SolidColorBrush)FindResource("ButtonSuccessBrush");
            else if (freeSpots > 0)
                spotsBrush = (SolidColorBrush)FindResource("WarningBrush");
            else
                spotsBrush = (SolidColorBrush)FindResource("DangerBrush");
            stack.Children.Add(new TextBlock
            {
                Text = spotsText,
                FontSize = 13,
                Foreground = spotsBrush,
                Margin = new Thickness(0, 0, 0, 5)
            });

            // Описание
            if (!string.IsNullOrEmpty(classItem.Description))
            {
                stack.Children.Add(new TextBlock
                {
                    Text = classItem.Description,
                    FontSize = 12,
                    Foreground = (SolidColorBrush)FindResource("TextSecondaryBrush"),
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 5, 0, 10)
                });
            }

            // Кнопка или статус
            if (_showCurrent)
            {
                bool isRegistered = IsUserRegistered(classItem.Id);
                bool isFull = classItem.CurrentParticipants >= classItem.MaxParticipants;

                var registerButton = new Button
                {
                    Style = (Style)FindResource("PrimaryButton"),
                    Tag = classItem.Id,
                    Margin = new Thickness(0, 5, 0, 0),
                    Padding = new Thickness(10, 8, 10, 8),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    FontSize = 13
                };

                if (Session.CurrentUserRole == "Trainer")
                {
                    registerButton.IsEnabled = false;
                    registerButton.Content = "❌ Тренерам нельзя";
                    registerButton.Background = (SolidColorBrush)FindResource("BorderBrush");
                    registerButton.Foreground = (SolidColorBrush)FindResource("TextSecondaryBrush");
                }
                else if (isRegistered)
                {
                    registerButton.Content = "✅ Вы записаны";
                    registerButton.Background = (SolidColorBrush)FindResource("ButtonSuccessBrush");
                    registerButton.Foreground = (SolidColorBrush)FindResource("TextPrimaryBrush");
                    registerButton.IsEnabled = false;
                }
                else if (isFull)
                {
                    registerButton.Content = "❌ Нет мест";
                    registerButton.Background = (SolidColorBrush)FindResource("BorderBrush");
                    registerButton.Foreground = (SolidColorBrush)FindResource("TextSecondaryBrush");
                    registerButton.IsEnabled = false;
                }
                else
                {
                    registerButton.Content = "📝 Записаться";
                    registerButton.Background = (SolidColorBrush)FindResource("ButtonPrimaryBrush");
                    registerButton.Foreground = (SolidColorBrush)FindResource("TextPrimaryBrush");
                    registerButton.Click += (s, e) => RegisterForClass(classItem.Id);
                }

                stack.Children.Add(registerButton);
            }
            else
            {
                var registration = GetUserRegistration(classItem.Id);
                var statusText = registration?.IsAttended == true ? "✅ Посещено" : "❌ Не посещено";
                var statusBlock = new TextBlock
                {
                    Text = statusText,
                    FontSize = 13,
                    Foreground = registration?.IsAttended == true ? (SolidColorBrush)FindResource("ButtonSuccessBrush") : (SolidColorBrush)FindResource("BorderBrush"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                stack.Children.Add(statusBlock);
            }

            border.Child = stack;
            return border;
        }

        private void btnCurrent_Click(object sender, RoutedEventArgs e)
        {
            _showCurrent = true;

            btnCurrent.Background =
                (SolidColorBrush)FindResource("ButtonSuccessBrush");

            btnHistory.Background =
                (SolidColorBrush)FindResource("SecondaryBackgroundBrush");

            btnHistory.Foreground =
                (SolidColorBrush)FindResource("TextPrimaryBrush");

            LoadClasses();
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            _showCurrent = false;

            btnHistory.Background =
                (SolidColorBrush)FindResource("ButtonSuccessBrush");

            btnCurrent.Background =
                (SolidColorBrush)FindResource("SecondaryBackgroundBrush");

            btnCurrent.Foreground =
                (SolidColorBrush)FindResource("TextPrimaryBrush");

            LoadClasses();
        }
    }
}