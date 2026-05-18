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

namespace Energy.Pages.AdminPages
{
    public partial class AdminSubscriptionsPage : Page
    {
        private int _selectedId;

        public AdminSubscriptionsPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                var list = db.Subscriptions.ToList();
                dgSubscriptions.ItemsSource = list;
                txtStatus.Text = $"Всего: {list.Count}";
            }
        }

        private void ClearForm()
        {
            _selectedId = 0;
            txtName.Text = "";
            txtCondition.Text = "";
            txtDurationDays.Text = "";
            txtPrice.Text = "";
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtPrice.Text))
            {
                MessageBox.Show("Заполните название и цену!");
                return;
            }

            if (!int.TryParse(txtDurationDays.Text, out int days) || days <= 0)
            {
                MessageBox.Show("Введите корректный срок!");
                return;
            }

            if (!int.TryParse(txtPrice.Text, out int price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену!");
                return;
            }

            using (var db = new AppDbContext())
            {
                var item = new Subscription
                {
                    Name = txtName.Text,
                    Condition = txtCondition.Text,
                    DurationDays = days,
                    Price = price
                };
                db.Subscriptions.Add(item);
                db.SaveChanges();
            }

            LoadData();
            ClearForm();
            MessageBox.Show("Добавлено!");
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0)
            {
                MessageBox.Show("Выберите запись!");
                return;
            }

            using (var db = new AppDbContext())
            {
                var item = db.Subscriptions.Find(_selectedId);
                if (item != null)
                {
                    item.Name = txtName.Text;
                    item.Condition = txtCondition.Text;
                    item.DurationDays = int.Parse(txtDurationDays.Text);
                    item.Price = int.Parse(txtPrice.Text);
                    db.SaveChanges();
                }
            }

            LoadData();
            ClearForm();
            MessageBox.Show("Обновлено!");
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0)
            {
                MessageBox.Show("Выберите запись!");
                return;
            }

            if (MessageBox.Show("Удалить?", "Подтверждение",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    var item = db.Subscriptions.Find(_selectedId);
                    if (item != null)
                    {
                        db.Subscriptions.Remove(item);
                        db.SaveChanges();
                    }
                }
                LoadData();
                ClearForm();
                MessageBox.Show("Удалено!");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void dgSubscriptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgSubscriptions.SelectedItem is Subscription item)
            {
                _selectedId = item.Id;
                txtName.Text = item.Name;
                txtCondition.Text = item.Condition;
                txtDurationDays.Text = item.DurationDays.ToString();
                txtPrice.Text = item.Price.ToString();
            }
        }
    }
}