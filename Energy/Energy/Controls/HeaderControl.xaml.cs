using Energy.Helpers;
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

namespace Energy.Controls
{
    /// <summary>
    /// Логика взаимодействия для HeaderControl.xaml
    /// </summary>
    public partial class HeaderControl : UserControl
    {
        public HeaderControl()
        {
            InitializeComponent();
            this.Loaded += HeaderControl_Loaded;
        }

        private void HeaderControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Загружаем имя из session.json
            var savedUser = SessionManager.LoadUser();

            if (savedUser != null)
            {
                btn_Profile.Content = $"👤 {savedUser.Login}";
            }
        }

        private void btn_Trainers_Click(object sender, RoutedEventArgs e)
        {
            var trainersWindow = new TrainersWindow();
            trainersWindow.Show();
            CloseCurrentWindow();
        }

        private void btn_Subscriptions_Click(object sender, RoutedEventArgs e)
        {
            var subscriptionsWindow = new SubscriptionsWindow();
            subscriptionsWindow.Show();
            CloseCurrentWindow();
        }

        private void btn_Profile_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new ProfileWindow();
            profileWindow.Show();
            CloseCurrentWindow();
        }

        private void CloseCurrentWindow()
        {
            var window = Window.GetWindow(this);
            window?.Close();
        }

        private void btn_Groupe_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
