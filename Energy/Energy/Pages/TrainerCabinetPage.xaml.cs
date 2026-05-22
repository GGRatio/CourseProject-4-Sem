using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using Energy.Data;
using Energy.Helpers;
using Energy.Models;

namespace Energy.Pages
{
    public partial class TrainerCabinetPage : Page
    {
        private int _trainerId;
        private string _trainerFullName;

        public TrainerCabinetPage()
        {
            InitializeComponent();
            this.Loaded += (s, e) => LoadTrainerData();
        }

        private void LoadTrainerData()
        {
            using (var db = new AppDbContext())
            {
                var trainer = db.Trainers
                    .FirstOrDefault(t => t.FirstName == Session.CurrentUserFirstName &&
                                         t.LastName == Session.CurrentUserLastName);

                if (trainer != null)
                {
                    _trainerId = trainer.Id;
                    _trainerFullName = $"{trainer.FirstName} {trainer.LastName}";
                    txtTitle.Text = $"Кабинет тренера: {_trainerFullName}";

                    LoadFutureClasses(db);
                    LoadPastClasses(db);
                    LoadClients(db);
                }
                else
                {
                    txtTitle.Text = "Кабинет тренера";
                }
            }
        }

        // Будущие занятия (просто просмотр)
        private void LoadFutureClasses(AppDbContext db)
        {
            var today = DateTime.Now;

            var classes = db.GroupClasses
                .Where(gc => gc.Instructor == _trainerFullName && gc.ClassDate > today)
                .OrderBy(gc => gc.ClassDate)
                .ToList();

            futureClassesList.ItemsSource = classes;
        }

        // Прошедшие занятия (за последние 14 дней для отметки)
        private void LoadPastClasses(AppDbContext db)
        {
            var twoWeeksAgo = DateTime.Now.AddDays(-14);
            var today = DateTime.Now;

            var classes = db.GroupClasses
                .Where(gc => gc.Instructor == _trainerFullName &&
                             gc.ClassDate >= twoWeeksAgo &&
                             gc.ClassDate <= today)
                .OrderByDescending(gc => gc.ClassDate)
                .ToList();

            pastClassesList.ItemsSource = classes;
        }

        private void LoadClients(AppDbContext db)
        {
            var clientIds = db.ClassRegistrations
                .Where(r => r.GroupClass.Instructor == _trainerFullName && !r.IsCanceled)
                .Select(r => r.UserId)
                .Distinct()
                .ToList();

            var clients = db.Users
                .Where(u => clientIds.Contains(u.Id))
                .ToList();

            clientsList.ItemsSource = clients;
        }

        private void MarkAttendance_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int classId = (int)button.Tag;

            using (var db = new AppDbContext())
            {
                var groupClass = db.GroupClasses.Find(classId);
                if (groupClass == null) return;

                var registrations = db.ClassRegistrations
                    .Where(r => r.GroupClassId == classId && !r.IsCanceled)
                    .ToList();

                if (!registrations.Any())
                {
                    MessageBox.Show("Нет записанных клиентов на это занятие!");
                    return;
                }

                var dialog = new Window
                {
                    Title = $"Отметка посещаемости - {groupClass.Name}",
                    Width = 350,
                    Height = 400,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                var stack = new StackPanel { Margin = new Thickness(10) };

                stack.Children.Add(new TextBlock
                {
                    Text = "Отметьте посетивших клиентов:",
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 10)
                });

                var checkBoxes = new List<CheckBox>();

                foreach (var reg in registrations)
                {
                    var user = db.Users.Find(reg.UserId);
                    string userName = user != null ? $"{user.FirstName} {user.LastName}" : reg.UserId.ToString();

                    var cb = new CheckBox
                    {
                        Content = userName,
                        Tag = reg.Id,
                        IsChecked = reg.IsAttended,
                        Margin = new Thickness(0, 5, 0, 5),
                        FontSize = 13
                    };
                    checkBoxes.Add(cb);
                    stack.Children.Add(cb);
                }

                var saveButton = new Button
                {
                    Content = "Сохранить",
                    Background = new SolidColorBrush(Colors.Green),
                    Foreground = new SolidColorBrush(Colors.White),
                    Padding = new Thickness(10, 5, 10, 5),
                    Margin = new Thickness(0, 15, 0, 0),
                    Cursor = System.Windows.Input.Cursors.Hand
                };

                saveButton.Click += (s, args) =>
                {
                    using (var context = new AppDbContext())
                    {
                        foreach (var cb in checkBoxes)
                        {
                            int registrationId = (int)cb.Tag;
                            var reg = context.ClassRegistrations.Find(registrationId);
                            if (reg != null)
                            {
                                reg.IsAttended = cb.IsChecked == true;
                            }
                        }
                        context.SaveChanges();
                    }
                    MessageBox.Show("Посещаемость отмечена!");
                    (saveButton.Parent as Window)?.Close();

                    // Обновляем списки
                    LoadPastClasses(db);
                    LoadClients(db);
                };

                stack.Children.Add(saveButton);
                dialog.Content = stack;
                dialog.ShowDialog();
            }
        }
    }
}