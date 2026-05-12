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
            buyButton.Click += (s, e) => BuySubscription(subscriptionId, price);
            Grid.SetColumn(buyButton, 1);
            bottomGrid.Children.Add(buyButton);

            mainStack.Children.Add(bottomGrid);
            border.Child = mainStack;

            return border;
        }

        private void BuySubscription(int subscriptionId, int price)
        {
            MessageBox.Show($"Абонемент куплен за {price} BYN", "Покупка",
                          MessageBoxButton.OK, MessageBoxImage.Information);
            // TODO: Добавить запись в таблицу Purchases
        }
    }
}
