using Energy.Data;
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
    }
}
