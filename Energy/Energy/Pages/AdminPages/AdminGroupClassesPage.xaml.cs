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
    public partial class AdminGroupClassesPage : Page
    {
        private int _selectedId;

        public AdminGroupClassesPage()
        {
            InitializeComponent();
            LoadData();
            // Устанавливаем время по умолчанию
            cbTime.SelectedItem = cbTime.Items.OfType<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == "18:00");
        }

        private DateTime GetSelectedDateTime()
        {
            DateTime date = dpDate.SelectedDate ?? DateTime.Now;
            string time = (cbTime.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "18:00";

            string[] parts = time.Split(':');
            int hours = int.Parse(parts[0]);
            int minutes = parts.Length > 1 ? int.Parse(parts[1]) : 0;

            return new DateTime(date.Year, date.Month, date.Day, hours, minutes, 0);
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                var list = db.GroupClasses.ToList();
                dgClasses.ItemsSource = list;
                txtStatus.Text = $"Всего занятий: {list.Count}";
            }
        }

        private void ClearForm()
        {
            _selectedId = 0;
            txtName.Text = "";
            txtInstructor.Text = "";
            dpDate.SelectedDate = DateTime.Now;
            cbTime.SelectedIndex = 0;
            txtDuration.Text = "60";
            txtMaxParticipants.Text = "10";
            txtDescription.Text = "";
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtInstructor.Text))
            {
                MessageBox.Show("Заполните название и инструктора!");
                return;
            }

            if (!int.TryParse(txtDuration.Text, out int duration) || duration <= 0)
            {
                MessageBox.Show("Введите корректную длительность!");
                return;
            }

            if (!int.TryParse(txtMaxParticipants.Text, out int maxParticipants) || maxParticipants <= 0)
            {
                MessageBox.Show("Введите корректное количество мест!");
                return;
            }

            DateTime classDate = GetSelectedDateTime();

            using (var db = new AppDbContext())
            {
                var item = new GroupClass
                {
                    Name = txtName.Text,
                    Instructor = txtInstructor.Text,
                    ClassDate = classDate,
                    DurationMinutes = duration,
                    MaxParticipants = maxParticipants,
                    CurrentParticipants = 0,
                    Description = txtDescription.Text
                };
                db.GroupClasses.Add(item);
                db.SaveChanges();
            }

            LoadData();
            ClearForm();
            MessageBox.Show("Занятие добавлено!");
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0)
            {
                MessageBox.Show("Выберите запись!");
                return;
            }

            DateTime classDate = GetSelectedDateTime();

            using (var db = new AppDbContext())
            {
                var item = db.GroupClasses.Find(_selectedId);
                if (item != null)
                {
                    item.Name = txtName.Text;
                    item.Instructor = txtInstructor.Text;
                    item.ClassDate = classDate;
                    item.DurationMinutes = int.Parse(txtDuration.Text);
                    item.MaxParticipants = int.Parse(txtMaxParticipants.Text);
                    item.Description = txtDescription.Text;
                    db.SaveChanges();
                }
            }

            LoadData();
            ClearForm();
            MessageBox.Show("Занятие обновлено!");
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0)
            {
                MessageBox.Show("Выберите запись!");
                return;
            }

            if (MessageBox.Show("Удалить занятие?", "Подтверждение",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    var item = db.GroupClasses.Find(_selectedId);
                    if (item != null)
                    {
                        db.GroupClasses.Remove(item);
                        db.SaveChanges();
                    }
                }
                LoadData();
                ClearForm();
                MessageBox.Show("Занятие удалено!");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void dgClasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgClasses.SelectedItem != null && dgClasses.SelectedItem is GroupClass item)
            {
                _selectedId = item.Id;
                txtName.Text = item.Name;
                txtInstructor.Text = item.Instructor;
                dpDate.SelectedDate = item.ClassDate;

                string time = item.ClassDate.ToString("HH:mm");
                var timeItem = cbTime.Items.OfType<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == time);
                if (timeItem != null)
                    cbTime.SelectedItem = timeItem;
                else
                    cbTime.Text = time;

                txtDuration.Text = item.DurationMinutes.ToString();
                txtMaxParticipants.Text = item.MaxParticipants.ToString();
                txtDescription.Text = item.Description;
            }
        }
    }
}

