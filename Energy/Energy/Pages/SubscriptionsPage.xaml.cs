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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Energy.Pages
{
    public partial class SubscriptionsPage : Page
    {

        private List<Subscription> _allSubscriptions;
        private Button _activeFilter;
        public SubscriptionsPage()
        {
            InitializeComponent();
            LoadSubscription();
        }

        private void LoadSubscription()
        {
            using (var db = new AppDbContext())
            {
                _allSubscriptions = db.Subscriptions.ToList();
                DisplaySubscriptions(_allSubscriptions);
            }
        }

        private void DisplaySubscriptions(List<Subscription> subscriptions)
        {
            SubscriptionsPanel.Children.Clear();

            foreach (var subscription in subscriptions)
            {
                var card = CreateCard(
                    subscription.Name,
                    subscription.Condition,
                    subscription.DurationDays,
                    subscription.Price,
                    subscription.Id
                );
                SubscriptionsPanel.Children.Add(card);
            }
        }


        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string filter = button.Tag.ToString();
            
            // Обновляем стиль кнопок
            if (_activeFilter != null)
                _activeFilter.Style = (Style)FindResource("FilterButton");
            
            button.Style = (Style)FindResource("FilterButtonActive");
            _activeFilter = button;
            
            // Фильтрация
            var filtered = _allSubscriptions.Where(s => 
            {
                switch (filter)
                {
                    case "student":
                        return s.Condition == "Студенческий";
                    case "daytime":
                        return s.Condition == "До 16:00";
                    case "unlimited":
                        return s.Name.Contains("Безлимит");
                    case "monthly":
                        return s.DurationDays == 30;
                    default:
                        return true;
                }
            }).ToList();
            
            DisplaySubscriptions(filtered);
        }

        private Border CreateCard(string name, string condition, int durationDays, int price, int subscriptionId)
        {
            var border = new Border
            {
                Width = 320,
                MinHeight = 180,
                Margin = new Thickness(15),
                CornerRadius = new CornerRadius(12),
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = new ScaleTransform(1, 1)
            };

            // Применяем стиль из ресурсов (он подхватит тему)
            border.SetResourceReference(Border.BackgroundProperty, "CardBackgroundBrush");
            border.SetResourceReference(Border.BorderBrushProperty, "BorderBrush");
            border.BorderThickness = new Thickness(1);

            // Тень
            border.Effect = new DropShadowEffect
            {
                BlurRadius = 8,
                ShadowDepth = 2,
                Opacity = 0.08
            };

            // Главный контейнер
            var mainStack = new StackPanel();
            mainStack.Margin = new Thickness(15, 18, 15, 18);

            // Верхняя строка: название + условие
            var headerPanel = new StackPanel();
            headerPanel.Orientation = Orientation.Horizontal;

            // Название
            var nameBlock = new TextBlock
            {
                Text = name,
                FontSize = 22,
                FontWeight = FontWeights.SemiBold
            };
            nameBlock.SetResourceReference(TextBlock.ForegroundProperty, "TextPrimaryBrush");
            headerPanel.Children.Add(nameBlock);

            // Условие (если есть)
            if (!string.IsNullOrEmpty(condition))
            {
                var conditionBlock = new TextBlock
                {
                    Text = condition,
                    FontSize = 13,
                    Margin = new Thickness(15, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                conditionBlock.SetResourceReference(TextBlock.ForegroundProperty, "TextSecondaryBrush");
                headerPanel.Children.Add(conditionBlock);
            }

            mainStack.Children.Add(headerPanel);

            // Срок действия
            var durationBlock = new TextBlock
            {
                Text = $"Срок действия: {durationDays} дней",
                FontSize = 13,
                Margin = new Thickness(0, 5, 0, 0)
            };
            durationBlock.SetResourceReference(TextBlock.ForegroundProperty, "TextSecondaryBrush");
            mainStack.Children.Add(durationBlock);

            // Нижняя строка: цена + кнопка
            var bottomGrid = new Grid();
            bottomGrid.Margin = new Thickness(0, 12, 0, 0);
            bottomGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            bottomGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Цена
            var priceBlock = new TextBlock
            {
                Text = $"{price} BYN",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center
            };
            priceBlock.SetResourceReference(TextBlock.ForegroundProperty, "ButtonSuccessBrush");
            Grid.SetColumn(priceBlock, 0);
            bottomGrid.Children.Add(priceBlock);

            // Кнопка "Купить"
            var buyButton = new Button
            {
                Content = "Купить",
                FontSize = 14,
                Padding = new Thickness(20, 8, 20, 8),
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Tag = subscriptionId

            };
            buyButton.SetResourceReference(Button.BackgroundProperty, "ButtonPrimaryBrush");
            buyButton.SetResourceReference(Button.ForegroundProperty, "TextPrimaryBrush");
            buyButton.Click += (s, e) => BuySubscription(subscriptionId, price, name);

            Grid.SetColumn(buyButton, 1);
            bottomGrid.Children.Add(buyButton);

            mainStack.Children.Add(bottomGrid);
            border.Child = mainStack;

            // Анимация при наведении (через EventTrigger в стиле, но добавим вручную для каждой карточки)
            border.MouseEnter += (s, e) =>
            {
                border.RenderTransform = new ScaleTransform(1.05, 1.05);
                (border.Effect as DropShadowEffect).Opacity = 0.15;
                (border.Effect as DropShadowEffect).ShadowDepth = 5;
            };
            border.MouseLeave += (s, e) =>
            {
                border.RenderTransform = new ScaleTransform(1, 1);
                (border.Effect as DropShadowEffect).Opacity = 0.08;
                (border.Effect as DropShadowEffect).ShadowDepth = 2;
            };

            return border;
        }

        private void BuySubscription(int subscriptionId, int price, string name)
        {
            using (var db = new AppDbContext())
            {
                // Проверяем, есть ли активный абонемент
                var activePurchase = db.Purchases
                    .FirstOrDefault(p => p.UserId == Session.CurrentUserId && p.IsActive && p.EndDate > DateTime.Now);

                if (activePurchase != null)
                {
                    // Спрашиваем пользователя
                    var result = MessageBox.Show(
                        "У вас уже есть активный абонемент!\n\n" +
                        "Хотите заменить его новым?\n" +
                        "(старый станет неактивным)",
                        "Замена абонемента",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.No)
                    {
                        return; // Не покупаем
                    }

                    // Делаем старый абонемент неактивным
                    activePurchase.IsActive = false;
                    db.SaveChanges();
                }

                // Создаём новую покупку
                var subscription = db.Subscriptions.Find(subscriptionId);

                var purchase = new Purchase
                {
                    UserId = Session.CurrentUserId,
                    Subscriptionid = subscriptionId,
                    PurchaseDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(subscription.DurationDays),
                    IsActive = true
                };

                db.Purchases.Add(purchase);
                db.SaveChanges();

                MessageBox.Show($"Абонемент '{name}' успешно куплен!", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
