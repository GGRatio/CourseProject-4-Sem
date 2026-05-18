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

using Energy.Pages.AdminPages;
using System.IO;

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

            var savedUser = SessionManager.LoadUser();

            if (savedUser != null)
            {
                btn_Profile.Content = $"👤 {savedUser.Login}";
            }
            if(Session.CurrentUserRole== "Admin")
            {

                btn_Profile.Content = "Пользователи";
                btn_Subscriptions.Content = "Управление абонементаами";
                btn_Trainers.Content = "Управление тренерами";
                btn_Groupe.Content = "управление занятиями";
            }
        }


        private void btn_Subscriptions_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (Session.CurrentUserRole == "Admin" && mainWindow!=null)
            {
                mainWindow.NavigateTo(new AdminSubscriptionsPage());
            }
            else 
            {
                mainWindow.NavigateTo(new Pages.SubscriptionsPage());
            }
        }

        private void btn_Profile_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (Session.CurrentUserRole == "Admin" && mainWindow != null)
            {
                mainWindow.NavigateTo(new AdminPage());
            }
            else
            {
                mainWindow.NavigateTo(new Pages.ProfilePage());
            }
        }

        private void btn_Trainers_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                mainWindow.NavigateTo(new Pages.TrainersPage());
            }
        }

        private void btn_Groupe_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                mainWindow.NavigateTo(new Pages.GroupClassesPage());
            }
        }




    }
}
