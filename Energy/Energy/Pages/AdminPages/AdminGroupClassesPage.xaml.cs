using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Energy.Data;
using Energy.Models;

namespace Energy.Pages.AdminPages
{
    public partial class AdminGroupClassesPage : Page
    {
        private int _selectedId;

        public AdminGroupClassesPage()
        {
            InitializeComponent();
            LoadTrainers();
            LoadData();
            cbTime.SelectedIndex = 0;
        }

        private void LoadTrainers()
        {
            using (var db = new AppDbContext())
            {
                var trainers = db.Trainers.ToList();

                if (trainers.Count == 0)
                {
                    MessageBox.Show("Нет тренеров! Сначала добавьте тренеров.");
                    return;
                }

                cbTrainer.ItemsSource = trainers;
                cbTrainer.DisplayMemberPath = "FirstName";
                cbTrainer.SelectedValuePath = "Id";

            }
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
            cbTrainer.SelectedIndex = -1;
            dpDate.SelectedDate = DateTime.Now;
            cbTime.SelectedIndex = 0;
            txtDuration.Text = "60";
            txtMaxParticipants.Text = "10";
            txtDescription.Text = "";
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("Введите название занятия!");
                return;
            }

            if (cbTrainer.SelectedItem == null)
            {
                MessageBox.Show("Выберите тренера!");
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

            var selectedTrainer = cbTrainer.SelectedItem as Trainer;
            DateTime classDate = GetSelectedDateTime();

            using (var db = new AppDbContext())
            {
                var item = new GroupClass
                {
                    Name = txtName.Text,
                    Instructor = selectedTrainer.FirstName + " " + selectedTrainer.LastName,
                    InstructorId = selectedTrainer.Id,
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
                MessageBox.Show("Выберите занятие!");
                return;
            }

            if (cbTrainer.SelectedItem == null)
            {
                MessageBox.Show("Выберите тренера!");
                return;
            }

            DateTime classDate = GetSelectedDateTime();
            var selectedTrainer = cbTrainer.SelectedItem as Trainer;

            using (var db = new AppDbContext())
            {
                var item = db.GroupClasses.Find(_selectedId);
                if (item != null)
                {
                    item.Name = txtName.Text;
                    item.Instructor = selectedTrainer.FirstName + " " + selectedTrainer.LastName;
                    item.InstructorId = selectedTrainer.Id;
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
                MessageBox.Show("Выберите занятие!");
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

                // Выбираем тренера в ComboBox
                if (item.InstructorId.HasValue)
                {
                    cbTrainer.SelectedValue = item.InstructorId.Value;
                }
            }
        }
    }
}