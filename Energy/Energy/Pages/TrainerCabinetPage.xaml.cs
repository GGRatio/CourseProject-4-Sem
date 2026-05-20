using Energy.Data;
using Energy.Helpers;
using System;
using System.Linq;
using System.Windows.Controls;

namespace Energy.Pages
{
    public partial class TrainerCabinetPage : Page
    {
        private int _trainerId;
        private string _trainerName;

        public TrainerCabinetPage()
        {
            InitializeComponent();
            LoadTrainerData();
        }

        private void LoadTrainerData()
        {
            using (var db = new AppDbContext())
            {
                // Находим тренера по текущему пользователю
                var trainer = db.Trainers
                    .FirstOrDefault(t => t.FirstName == Session.CurrentUserFirstName &&
                                        t.LastName == Session.CurrentUserLastName);

                if (trainer != null)
                {
                    _trainerId = trainer.Id;
                    _trainerName = $"{trainer.FirstName} {trainer.LastName}";
                    txtTitle.Text = $"👨‍🏫 Кабинет тренера: {_trainerName}";

                    LoadClients(db);
                    LoadClasses(db);
                }   
                else
                {
                    txtTitle.Text = "👨‍🏫 Кабинет тренера";
                }
            }
        }

        private void LoadClients(AppDbContext db)
        {
            // Находим всех клиентов, которые выбрали этого тренера
            var clients = db.UserTrainers
                .Where(ut => ut.TrainerId == _trainerId)
                .Select(ut => ut.User)
                .ToList();

            clientsList.ItemsSource = clients;
        }

        private void LoadClasses(AppDbContext db)
        {
            // Находим занятия, которые ведёт этот тренер
            var classes = db.GroupClasses
                .Where(gc => gc.Instructor == _trainerName)
                .Where(gc => gc.ClassDate > DateTime.Now)
                .OrderBy(gc => gc.ClassDate)
                .ToList();

            classesList.ItemsSource = classes;
        }
    }
}