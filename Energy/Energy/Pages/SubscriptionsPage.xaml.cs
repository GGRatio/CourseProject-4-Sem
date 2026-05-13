using Energy.Data;
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

using Energy.Helpers;

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
            // Применяем стиль из ресурсов
            var border = new Border();
            border.SetResourceReference(StyleProperty, "SubscriptionCard");

            // Главный контейнер
            var mainStack = new StackPanel();
            mainStack.Margin = new Thickness(15, 18, 15, 18);

            // Верхняя строка: название + условие
            var headerPanel = new StackPanel();
            headerPanel.Orientation = Orientation.Horizontal;

            // Название
            var nameBlock = new TextBlock();
            nameBlock.SetResourceReference(StyleProperty, "CardTitle");
            nameBlock.Text = name;
            headerPanel.Children.Add(nameBlock);

            // Условие (если есть)
            if (!string.IsNullOrEmpty(condition))
            {
                var conditionBlock = new TextBlock();
                conditionBlock.SetResourceReference(StyleProperty, "CardCondition");
                conditionBlock.Text = condition;
                headerPanel.Children.Add(conditionBlock);
            }

            mainStack.Children.Add(headerPanel);

            // Срок действия
            var durationBlock = new TextBlock();
            durationBlock.SetResourceReference(StyleProperty, "CardDuration");
            durationBlock.Text = $"Срок действия: {durationDays} дней";
            mainStack.Children.Add(durationBlock);

            // Нижняя строка: цена + кнопка
            var bottomGrid = new Grid();
            bottomGrid.Margin = new Thickness(0, 12, 0, 0);
            bottomGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            bottomGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Цена
            var priceBlock = new TextBlock();
            priceBlock.SetResourceReference(StyleProperty, "CardPrice");
            priceBlock.Text = $"{price} BYN";
            Grid.SetColumn(priceBlock, 0);
            bottomGrid.Children.Add(priceBlock);

            // Кнопка
            var buyButton = new Button();
            buyButton.SetResourceReference(StyleProperty, "BuyButton");
            buyButton.Tag = subscriptionId;
            buyButton.Click += (s, e) => BuySubscription(subscriptionId, price, name);
            Grid.SetColumn(buyButton, 1);
            bottomGrid.Children.Add(buyButton);

            mainStack.Children.Add(bottomGrid);
            border.Child = mainStack;

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
