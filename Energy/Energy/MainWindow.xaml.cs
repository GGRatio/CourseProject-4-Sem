using Energy.Models;
using Energy.Pages.AdminPages;
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

namespace Energy
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ContentFrame.Navigate(new Pages.SubscriptionsPage());
        }

        public MainWindow(string userRole)
        {
            InitializeComponent();
            if (userRole == "Admin")
            {
                ContentFrame.Navigate(new AdminUsersPage());
            }
            else
            {
                ContentFrame.Navigate(new Pages.SubscriptionsPage());
            }

        }


        public void NavigateTo(Page page)
        {
            ContentFrame.Navigate(page);
        }
    }
}
