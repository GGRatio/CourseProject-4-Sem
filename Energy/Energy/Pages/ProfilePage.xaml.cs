using Energy.Data;
using Energy.Helpers;
using Microsoft.EntityFrameworkCore;
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
    /// <summary>
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        private bool _isEditing = false;

        public ProfilePage()
        {
            InitializeComponent();
            LoadUserData();
            LoadCurrentSubscription();
            LoadMyTrainer();
            LoadMyClasses();
            LoadVisitsCount();

            LoadPurchaseHistory();
            SetEditMode(false);
        }

        private void LoadUserData()
        {
            using (var db = new AppDbContext())
            {
                var user = db.Users.Find(Session.CurrentUserId);
                if (user != null)
                {
                    txtLogin.Text = user.Login;
                    txtFirstName.Text = user.FirstName ?? "";
                    txtLastName.Text = user.LastName ?? "";
                    txtPhone.Text = user.Phone ?? "";
                    txtEmail.Text = user.Email ?? "";
                }
            }
        }

        

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var user = db.Users.Find(Session.CurrentUserId);
                if (user != null)
                {
                    user.FirstName = txtFirstName.Text.Trim();
                    user.LastName = txtLastName.Text.Trim();
                    user.Phone = txtPhone.Text.Trim();
                    user.Email = txtEmail.Text.Trim();

                    db.SaveChanges();
                }
            }

            SetEditMode(false);
            MessageBox.Show("Данные сохранены!", "Успех",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            SetEditMode(true);
        }

        private void SetEditMode(bool isEditing)
        {
            _isEditing = isEditing;

            txtFirstName.IsEnabled = isEditing;
            txtLastName.IsEnabled = isEditing;
            txtPhone.IsEnabled = isEditing;
            txtEmail.IsEnabled = isEditing;

            btnSave.Visibility = isEditing ? Visibility.Visible : Visibility.Collapsed;
            btnEdit.Visibility = isEditing ? Visibility.Collapsed : Visibility.Visible;
        }

        private void LightTheme_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(ThemeManager.ThemeType.Light);
        }

        private void DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(ThemeManager.ThemeType.Dark);
        }

        private void LoadMyTrainer()
        {
            using (var db = new AppDbContext())
            {
                var userTrainer = db.UserTrainers
                    .FirstOrDefault(ut => ut.UserId == Session.CurrentUserId);

                if (userTrainer != null)
                {
                    var trainer = db.Trainers.Find(userTrainer.TrainerId);
                    if (trainer != null)
                    {
                        txtMyTrainer.Text = $"{trainer.FirstName} {trainer.LastName}";
                        txtTrainerSpecialization.Text = trainer.Specialization;

                        // Загружаем фото тренера
                        LoadTrainerPhoto(trainer.PhotoUrl);
                    }
                    else
                    {
                        txtMyTrainer.Text = "Тренер не найден";
                        TrainerPhotoBorder.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    txtMyTrainer.Text = "Тренер не выбран. Перейдите в раздел 'Тренеры' чтобы выбрать.";
                    txtTrainerSpecialization.Text = "";
                    TrainerPhotoBorder.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void LoadTrainerPhoto(string photoPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(photoPath))
                {
                    string fullPath = $"pack://application:,,,/{photoPath}";
                    TrainerPhoto.Source = new BitmapImage(new Uri(fullPath));
                    TrainerPhotoBorder.Visibility = Visibility.Visible;
                }
                else
                {
                    // Если фото нет - показываем иконку
                    TrainerPhotoBorder.Background = (SolidColorBrush)FindResource("ButtonPrimaryBrush");
                    var icon = new TextBlock
                    {
                        Text = "🏋️",
                        FontSize = 28,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    TrainerPhotoBorder.Child = icon;
                    TrainerPhoto.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                // Если ошибка загрузки
                TrainerPhotoBorder.Background = (SolidColorBrush)FindResource("ButtonPrimaryBrush");
                var icon = new TextBlock
                {
                    Text = "🏋️",
                    FontSize = 28,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                TrainerPhotoBorder.Child = icon;
                TrainerPhoto.Visibility = Visibility.Collapsed;
            }
        }



        public class UserClassInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Instructor { get; set; }
            public DateTime ClassDate { get; set; }
            public int RegistrationId { get; set; }
            public string ClassDateText => ClassDate.ToString("dd.MM.yyyy HH:mm");
        }

        private void LoadMyClasses()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var registrations = db.ClassRegistrations
                        .Include(r => r.GroupClass)
                        .Where(r => r.UserId == Session.CurrentUserId && !r.IsCanceled && r.GroupClass.ClassDate > DateTime.Now)
                        .ToList();

                    var items = registrations.Select(r => new UserClassInfo
                    {
                        Id = r.GroupClass.Id,
                        Name = r.GroupClass.Name,
                        Instructor = r.GroupClass.Instructor,
                        ClassDate = r.GroupClass.ClassDate,
                        RegistrationId = r.Id
                    }).OrderBy(r => r.ClassDate).ToList();

                    if (items.Count > 0)
                    {
                        lstMyClasses.Visibility = Visibility.Visible;
                        txtNoClasses.Visibility = Visibility.Collapsed;
                        lstMyClasses.ItemsSource = items;
                    }
                    else
                    {
                        lstMyClasses.Visibility = Visibility.Collapsed;
                        txtNoClasses.Visibility = Visibility.Visible;
                        lstMyClasses.ItemsSource = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                lstMyClasses.Visibility = Visibility.Collapsed;
                txtNoClasses.Visibility = Visibility.Visible;
                txtNoClasses.Text = "❌ Ошибка загрузки записей";
            }
        }

        //Отмена записи на гупповое занятие 
        private void CancelClass_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что нажата именно кнопка с Tag
            if (sender is Button button && button.Tag is int classId)
            {
                if (MessageBox.Show("Отменить запись на занятие?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (var db = new AppDbContext())
                    {
                        var registration = db.ClassRegistrations
                            .FirstOrDefault(r => r.UserId == Session.CurrentUserId && r.GroupClassId == classId && !r.IsCanceled);

                        if (registration != null)
                        {
                            registration.IsCanceled = true;

                            var classItem = db.GroupClasses.Find(classId);
                            if (classItem != null && classItem.CurrentParticipants > 0)
                            {
                                classItem.CurrentParticipants--;
                            }

                            db.SaveChanges();
                        }
                    }

                    LoadMyClasses();
                    MessageBox.Show("Запись отменена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }


        private void LoadVisitsCount()
        {
            using (var db = new AppDbContext())
            {
                // Считаем количество отмеченных посещений (IsAttended = true)
                int visitsCount = db.ClassRegistrations
                    .Count(r => r.UserId == Session.CurrentUserId && r.IsAttended);

                txtVisitsCount.Text = visitsCount.ToString();
            }
        }


        public class PurchaseHistoryItem
        {
            public string SubscriptionName { get; set; }
            public DateTime PurchaseDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Status { get; set; }
            public string StatusColor { get; set; }
        }

        private void LoadPurchaseHistory()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var purchases = db.Purchases
                        .Include(p => p.Subscription)
                        .Where(p => p.UserId == Session.CurrentUserId)
                        .OrderByDescending(p => p.PurchaseDate)
                        .ToList();

                    var items = purchases.Select(p => new PurchaseHistoryItem
                    {
                        SubscriptionName = p.Subscription?.Name ?? "Неизвестно",
                        PurchaseDate = p.PurchaseDate,
                        EndDate = p.EndDate,
                        Status = p.IsActive && p.EndDate > DateTime.Now ? "✅ Активен" : "⏰ Завершён",
                        StatusColor = p.IsActive && p.EndDate > DateTime.Now ? "Green" : "Gray"
                    }).ToList();

                    lstPurchaseHistory.ItemsSource = items;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void LoadCurrentSubscription()
        {
            using (var db = new AppDbContext())
            {
                var activePurchase = db.Purchases
                    .Include(p => p.Subscription)
                    .FirstOrDefault(p => p.UserId == Session.CurrentUserId && p.IsActive && p.EndDate > DateTime.Now);

                if (activePurchase != null)
                {
                    txtSubscriptionName.Text = activePurchase.Subscription.Name;
                    txtSubscriptionEndDate.Text = $"Действует до: {activePurchase.EndDate:dd.MM.yyyy}";

                    int daysLeft = (activePurchase.EndDate - DateTime.Now).Days;
                    txtSubscriptionStatus.Text = $"Осталось дней: {daysLeft}";

                    // Показываем кнопку продления
                    btnExtend.Visibility = Visibility.Visible;
                }
                else
                {
                    txtSubscriptionName.Text = "Нет активного абонемента";
                    txtSubscriptionEndDate.Text = "";
                    txtSubscriptionStatus.Text = "Купите абонемент в разделе 'Абонементы'";
                    btnExtend.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ExtendSubscription_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var activePurchase = db.Purchases
                    .Include(p => p.Subscription)
                    .FirstOrDefault(p => p.UserId == Session.CurrentUserId && p.IsActive && p.EndDate > DateTime.Now);

                if (activePurchase != null)
                {
                    // Продлеваем на 30 дней
                    activePurchase.EndDate = activePurchase.EndDate.AddDays(30);
                    db.SaveChanges();

                    MessageBox.Show($"Абонемент продлён до {activePurchase.EndDate:dd.MM.yyyy}!",
                                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadCurrentSubscription();
                }
            }
        }

    }
}
