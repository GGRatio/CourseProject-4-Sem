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
using System.Windows.Shapes;

using Energy.Data;
using Energy.Models;
using Energy.Helpers;
using System.Windows.Media.Animation;

namespace Energy
{
    /// <summary>
    /// Логика взаимодействия для SubscriptionsWindow.xaml
    /// </summary>
    public partial class SubscriptionsWindow : Window
    {
        public SubscriptionsWindow()
        {
            InitializeComponent();
            LoadSubscription();
        }

        private void LoadSubscription()
        {
            using (var db = new AppDbContext())
            {
                var subscriptions = db.Subscriptions.ToList();

                foreach (var subscription in subscriptions)
                {
                    var card = CreateCard(
                        subscription.Name,
                        subscription.Condition,
                        subscription.DurationDays,
                        subscription.Price
                    );
                    SubscriptionsPanel.Children.Add(card);
                }
            }
        }

        private Border CreateCard(string name, string condition, int durationDays, int price)
        {
            var border = new Border
            {
                Width = 400,
                Height = Double.NaN,
                MinHeight = 100,
                Margin = new Thickness(10),
                Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#D9D9D9"),
                BorderBrush = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(1),
            };

            // Главный контейнер
            var mainStack = new StackPanel();
            mainStack.Margin = new Thickness(15, 25, 15, 20);
            mainStack.HorizontalAlignment = HorizontalAlignment.Stretch;  // растянуть на всю ширину

            // Верхняя строка: название + условие (справа)
            var headerPanel = new StackPanel();
            headerPanel.Orientation = Orientation.Horizontal;
            headerPanel.Margin = new Thickness(0, 0, 0, 0);
            headerPanel.HorizontalAlignment = HorizontalAlignment.Stretch;  // растянуть на всю ширину

            // Название
            var nameBlock = new TextBlock
            {
                Text = name,
                FontSize = 30,
                FontWeight = FontWeights.SemiBold
            };
            headerPanel.Children.Add(nameBlock);

            // Условие (если есть) — справа
            if (!string.IsNullOrEmpty(condition))
            {
                var conditionBlock = new TextBlock
                {
                    Background = new SolidColorBrush(Colors.White),
                    Text = condition,
                    FontSize = 16,
                    Margin = new Thickness(20, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                headerPanel.Children.Add(conditionBlock);
            }

            mainStack.Children.Add(headerPanel);

            // Срок действия
            mainStack.Children.Add(new TextBlock
            {
                Text = $"Срок действия - {durationDays} дней",
                FontSize = 24,
                Foreground = new SolidColorBrush(Colors.Black),
                Margin = new Thickness(0, 0, 0, 10)
            });

            // Нижняя строка: цена слева, кнопка справа
            var bottomGrid = new Grid();
            bottomGrid.Margin = new Thickness(0, 10, 0, 0);

            // Создаём колонки
            bottomGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) }); // цена 
            bottomGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // кнопка * 

            // Цена
            var priceBlock = new TextBlock
            {
                Text = $"{price} BYN",
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#4CAF50"),
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(priceBlock, 0);
            bottomGrid.Children.Add(priceBlock);

            // Кнопка "Купить"
            var buyButton = new Button
            {
                Content = "Купить",
                FontSize = 24,
                Padding = new Thickness(25, 8, 25, 8),
                Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#BDBDBD"),
                Foreground = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            Grid.SetColumn(buyButton, 1);
            bottomGrid.Children.Add(buyButton);

            mainStack.Children.Add(bottomGrid);

            border.Child = mainStack;
            return border;
        }
    }
}
