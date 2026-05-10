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
                    string name = subscription.Name;
                    string description = subscription.Description;
                    int price = subscription.Price;

                    var card = CreateCard(name, description, price);
                    SubscriptionsPanel.Children.Add(card);
                }
            }
        }

        private Border CreateCard(string name, string description, int price)
        {
            var border = new Border
            {
                Width = 400,
                Height = 280,
                Background = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(1),
            };

            var stack = new StackPanel();
            stack.Margin = new Thickness(20);

            stack.Children.Add(new TextBlock
            {
                Text = name.ToUpper(),
                FontSize = 32,
                FontWeight = FontWeights.SemiBold
            });

            stack.Children.Add(new TextBlock
            {
                Text = description.ToUpper(),
                FontSize = 24,
            });

            stack.Children.Add(new TextBlock
            {
                Text = price.ToString(),
                FontSize = 24,
            });

            border.Child = stack;
            return border;
        }
        
        

    }
}
