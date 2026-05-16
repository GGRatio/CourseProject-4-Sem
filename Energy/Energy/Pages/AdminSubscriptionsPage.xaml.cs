using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Energy.Data;
using Energy.Models;
using Microsoft.EntityFrameworkCore; 

namespace Energy.Pages
{
    public partial class AdminSubscriptionsPage : Page
    {
        private int _selectedId;

        public AdminSubscriptionsPage()
        {
            InitializeComponent();
            LoadData();
        }

        // ==================== ЗАГРУЗКА ДАННЫХ ====================
        private async void LoadData()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            using (var db = new AppDbContext())
            {
                var subscriptions = await db.Subscriptions.ToListAsync();
                dgSubscriptions.ItemsSource = subscriptions;
            }
        }

        // ==================== CRUD АСИНХРОННО ====================

        // CREATE (Добавление)
        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtPrice.Text))
            {
                MessageBox.Show("Заполните название и цену!");
                return;
            }

            if (!int.TryParse(txtDurationDays.Text, out int days) || days <= 0)
            {
                MessageBox.Show("Введите корректный срок действия!");
                return;
            }

            if (!int.TryParse(txtPrice.Text, out int price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену!");
                return;
            }

            var sub = new Subscription
            {
                Name = txtName.Text,
                Price = price,
                DurationDays = days,
                Condition = txtCondition.Text
            };

            await AddSubscriptionAsync(sub);
            await LoadDataAsync();
            ClearForm();
            MessageBox.Show("Абонемент добавлен асинхронно!");
        }

        private async Task AddSubscriptionAsync(Subscription subscription)
        {
            using (var db = new AppDbContext())
            {
                db.Subscriptions.Add(subscription);
                await db.SaveChangesAsync();
            }
        }

        // UPDATE (Обновление)
        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0)
            {
                MessageBox.Show("Выберите запись для обновления!");
                return;
            }

            if (!int.TryParse(txtDurationDays.Text, out int days) || days <= 0)
            {
                MessageBox.Show("Введите корректный срок действия!");
                return;
            }

            if (!int.TryParse(txtPrice.Text, out int price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену!");
                return;
            }

            var sub = new Subscription
            {
                Id = _selectedId,
                Name = txtName.Text,
                Price = price,
                DurationDays = days,
                Condition = txtCondition.Text
            };

            await UpdateSubscriptionAsync(sub);
            await LoadDataAsync();
            ClearForm();
            MessageBox.Show("Абонемент обновлён асинхронно!");
        }

        private async Task UpdateSubscriptionAsync(Subscription subscription)
        {
            using (var db = new AppDbContext())
            {
                var existing = await db.Subscriptions.FindAsync(subscription.Id);
                if (existing != null)
                {
                    existing.Name = subscription.Name;
                    existing.Price = subscription.Price;
                    existing.DurationDays = subscription.DurationDays;
                    existing.Condition = subscription.Condition;
                    await db.SaveChangesAsync();
                }
            }
        }

        // DELETE (Удаление)
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0)
            {
                MessageBox.Show("Выберите запись для удаления!");
                return;
            }

            if (MessageBox.Show("Удалить выбранный абонемент?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                await DeleteSubscriptionAsync(_selectedId);
                await LoadDataAsync();
                ClearForm();
                MessageBox.Show("Абонемент удалён асинхронно!");
            }
        }

        private async Task DeleteSubscriptionAsync(int id)
        {
            using (var db = new AppDbContext())
            {
                var sub = await db.Subscriptions.FindAsync(id);
                if (sub != null)
                {
                    db.Subscriptions.Remove(sub);
                    await db.SaveChangesAsync();
                }
            }
        }

        // ==================== ФИЛЬТРАЦИЯ И СОРТИРОВКА ====================
        private void SortByPriceAsc()
        {
            using (var db = new AppDbContext())
            {
                var sorted = db.Subscriptions.OrderBy(s => s.Price).ToList();
                dgSubscriptions.ItemsSource = sorted;
            }
        }

        private void SortByPriceDesc()
        {
            using (var db = new AppDbContext())
            {
                var sorted = db.Subscriptions.OrderByDescending(s => s.Price).ToList();
                dgSubscriptions.ItemsSource = sorted;
            }
        }

        private void SearchByName(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                LoadData();
                return;
            }

            using (var db = new AppDbContext())
            {
                var filtered = db.Subscriptions
                    .Where(s => s.Name.ToLower().Contains(search.ToLower()))
                    .ToList();
                dgSubscriptions.ItemsSource = filtered;

                if (filtered.Count == 0)
                    MessageBox.Show("Ничего не найдено!");
            }
        }

        private void FilterByPrice(string minText, string maxText)
        {
            if (string.IsNullOrWhiteSpace(minText) || string.IsNullOrWhiteSpace(maxText))
            {
                MessageBox.Show("Введите минимальную и максимальную цену!");
                return;
            }

            if (!int.TryParse(minText, out int minPrice))
            {
                MessageBox.Show("Введите корректную минимальную цену!");
                return;
            }

            if (!int.TryParse(maxText, out int maxPrice))
            {
                MessageBox.Show("Введите корректную максимальную цену!");
                return;
            }

            using (var db = new AppDbContext())
            {
                var filtered = db.Subscriptions
                    .Where(s => s.Price >= minPrice && s.Price <= maxPrice)
                    .ToList();
                dgSubscriptions.ItemsSource = filtered;

                if (filtered.Count == 0)
                    MessageBox.Show("Ничего не найдено!");
            }
        }

        private void ResetFilters()
        {
            txtSearchName.Text = "";
            txtMinPriceFilter.Text = "";
            txtMaxPriceFilter.Text = "";
            LoadData();
        }

        // ==================== ОБРАБОТЧИКИ КНОПОК ====================
        private void SortByPriceAsc_Click(object sender, RoutedEventArgs e) => SortByPriceAsc();
        private void SortByPriceDesc_Click(object sender, RoutedEventArgs e) => SortByPriceDesc();
        private void SearchByName_Click(object sender, RoutedEventArgs e) => SearchByName(txtSearchName.Text);
        private void FilterByPrice_Click(object sender, RoutedEventArgs e) => FilterByPrice(txtMinPriceFilter.Text, txtMaxPriceFilter.Text);
        private void ResetFilter_Click(object sender, RoutedEventArgs e) => ResetFilters();

        private void btnClear_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void dgSubscriptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgSubscriptions.SelectedItem is Subscription sub)
            {
                _selectedId = sub.Id;
                txtName.Text = sub.Name;
                txtPrice.Text = sub.Price.ToString();
                txtDurationDays.Text = sub.DurationDays.ToString();
                txtCondition.Text = sub.Condition;
            }
        }

        private void ClearForm()
        {
            _selectedId = 0;
            txtName.Text = "";
            txtPrice.Text = "";
            txtDurationDays.Text = "";
            txtCondition.Text = "";
        }
    }
}