using Energy.Data;
using Energy.Helpers;
using Energy.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;


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

                var today = DateTime.Now.Date;

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
                BlurRadius = 8,
                ShadowDepth = 2,
                Opacity = 0.1
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
            var spotsText = freeSpots > 0 ? $"🎟Свободно мест: {freeSpots}" : "Мест нет";
            var spotsColor = freeSpots > 5 ? Colors.Green : (freeSpots > 0 ? Colors.Orange : Colors.Red);

            stack.Children.Add(new TextBlock
            {
                Text = spotsText,
                FontSize = 13,
                Foreground = new SolidColorBrush(spotsColor),
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

            // Кнопка
            if (_showCurrent)
            {
                // Проверяем, записан ли пользователь
                bool isRegistered = IsUserRegistered(classItem.Id);
                bool isFull = classItem.CurrentParticipants >= classItem.MaxParticipants;

                var registerButton = new Button
                {
                    Content = isRegistered ? "✅ Вы записаны" : (isFull ? "❌ Нет мест" : "📝 Записаться"),
                    Tag = classItem.Id,
                    Margin = new Thickness(0, 5, 0, 0),
                    Padding = new Thickness(10, 8, 10, 8),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    FontSize = 13
                };

                if (isRegistered)
                {
                    registerButton.Background = (SolidColorBrush)FindResource("ButtonSuccessBrush");
                    registerButton.Foreground = (SolidColorBrush)FindResource("TextPrimaryBrush");
                    registerButton.IsEnabled = false;
                }
                else if (isFull)
                {
                    registerButton.Background = (SolidColorBrush)FindResource("BorderBrush");
                    registerButton.Foreground = (SolidColorBrush)FindResource("TextSecondaryBrush");
                    registerButton.IsEnabled = false;
                }
                else
                {
                    registerButton.Background = (SolidColorBrush)FindResource("ButtonPrimaryBrush");
                    registerButton.Foreground = (SolidColorBrush)FindResource("TextPrimaryBrush");
                    registerButton.Click += (s, e) => RegisterForClass(classItem.Id);
                }

                stack.Children.Add(registerButton);
            }
            else
            {
                // Для истории показываем статус посещения
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

        private bool IsUserRegistered(int classId)
        {
            using (var db = new AppDbContext())
            {
                return db.ClassRegistrations.Any(r => r.UserId == Session.CurrentUserId && r.GroupClassId == classId && !r.IsCanceled);
            }
        }

        private ClassRegistration GetUserRegistration(int classId)
        {
            using (var db = new AppDbContext())
            {
                return db.ClassRegistrations.FirstOrDefault(r => r.UserId == Session.CurrentUserId && r.GroupClassId == classId);
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

                if (classItem.CurrentParticipants >= classItem.MaxParticipants)
                {
                    MessageBox.Show("Нет свободных мест!");
                    return;
                }

                // Добавляем регистрацию
                var registration = new ClassRegistration
                {
                    UserId = Session.CurrentUserId,
                    GroupClassId = classId,
                    RegistrationDate = DateTime.Now,
                    IsAttended = false,
                    IsCanceled = false
                };

                db.ClassRegistrations.Add(registration);

                // Увеличиваем количество участников
                classItem.CurrentParticipants++;

                db.SaveChanges();
            }

            LoadClasses();
            MessageBox.Show("Вы записаны на занятие!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnCurrent_Click(object sender, RoutedEventArgs e)
        {
            _showCurrent = true;
            btnCurrent.Background = (SolidColorBrush)FindResource("ButtonSuccessBrush");
            btnHistory.Background = (SolidColorBrush)FindResource("BorderBrush");
            LoadClasses();
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            _showCurrent = false;
            btnCurrent.Background = (SolidColorBrush)FindResource("BorderBrush");
            btnHistory.Background = (SolidColorBrush)FindResource("ButtonSuccessBrush");
            LoadClasses();
        }
    }
}