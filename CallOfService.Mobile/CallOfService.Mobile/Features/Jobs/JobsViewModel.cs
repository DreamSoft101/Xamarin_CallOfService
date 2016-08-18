using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CallOfService.Mobile.Services.Abstracts;
using CallOfService.Mobile.UI;
using Xamarin.Forms;
using PubSub;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Core;
using CallOfService.Mobile.Messages;

namespace CallOfService.Mobile.Features.Jobs
{
    public class JobsViewModel : ViewModelBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IAnalyticsService _analyticsService;
       
        public JobsViewModel(IAppointmentService appointmentService, IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
            _appointmentService = appointmentService;

            Appointments = new ObservableCollection<AppointmentModel>();
            this.Subscribe<JobSelected>(async m =>
            {
                await NavigationService.NavigateToJobDetails();
                this.Publish(new ViewJobDetails(m.JobId));
            });
            this.Subscribe<NewDateSelected>(m =>
            {
                _analyticsService.Track("Date Selected");

                Date = m.DateTime;
                OnAppearing();
            });
            Date = DateTime.Today;
        }

        public ObservableCollection<AppointmentModel> Appointments { get; set; }

        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set { SetPropertyValue(ref _date, value); }
        }

        private bool _hasAppointments;
        public bool HasAppointments
        {
            get { return _hasAppointments; }
            set { SetPropertyValue(ref _hasAppointments, value); }
        }

        private bool _hasNoAppointments;
        public bool HasNoAppointments
        {
            get { return _hasNoAppointments; }
            set { SetPropertyValue(ref _hasNoAppointments, value); }
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set { SetPropertyValue(ref _isRefreshing, value); RaisePropertyChanged("CanNavigate");}
        }

        public bool CanNavigate => !IsRefreshing;

        public ICommand GoToNextDay
        {
            get
            {
                return new Command(() =>
                {
                    _analyticsService.Track("Navigate to next day");
                    Date = Date.AddDays(1);
                    OnAppearing();
                });
            }
        }

        public ICommand GoToPrevDay
        {
            get
            {
                return new Command(() =>
                {
                    _analyticsService.Track("Navigate to prev day");
                    Date = Date.AddDays(-1);
                    OnAppearing();
                });
            }
        }

        public ICommand ShowCalendarView
        {
            get
            {
                return new Command(() =>
                {
                    this.Publish(new ShowCalendarView());
                });
            }
        }

        public ICommand RefreshListOfJobsCommand
        {
            get
            {
                return new Command(() =>
                {
                    _analyticsService.Track("Refreshing Jobs");
                    OnAppearing();
                });
            }
        }

        public override void Dispose()
        {
            Appointments.Clear();
        }

        public override async void OnAppearing()
        {
            if (IsRefreshing)
                return;

            IsRefreshing = true;
            var appointments = await _appointmentService.AppointmentsByDay(Date);
            Appointments.Clear();
            foreach (var appointment in appointments.Where(a => !a.IsCancelled).OrderBy(a => a.Start))
            {
                Appointments.Add(new AppointmentModel
                {
                    Title = appointment.Title,
                    Location = appointment.Location,
                    IsFinished = appointment.IsFinished,
                    IsInProgress = appointment.IsInProgress,
                    IsCancelled = appointment.IsCancelled,
                    StartTimeEndTimeFormated = $"{appointment.Start.ToUniversalTime().ToString("hh:mm tt")} - {appointment.End.ToUniversalTime().ToString("hh:mm tt")}",
                    JobId = appointment.JobId
                });
            }
            HasAppointments = Appointments.Any();
            HasNoAppointments = !HasAppointments;
            IsRefreshing = false;
        }
    }

    //Check if you need notify property changed
    public class AppointmentModel
    {
        public string Title { get; set; }

        public string StartTimeEndTimeFormated { get; set; }

        public string Location { get; set; }

        public int JobId { get; set; }

        public bool IsFinished { get; set; }
        public bool IsInProgress { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsScheduled => !IsFinished && !IsInProgress && !IsCancelled;

        public ICommand ViewDetails { get { return new Command(() => this.Publish(new JobSelected(JobId))); } }
    }
}