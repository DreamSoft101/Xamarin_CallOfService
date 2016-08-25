using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CallOfService.Mobile.Core;
using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Features.Calendar;
using CallOfService.Mobile.Messages;
using CallOfService.Mobile.Services.Abstracts;
using CallOfService.Mobile.UI;
using PubSub;
using Xamarin.Forms;

namespace CallOfService.Mobile.Features.Jobs
{
    public enum Source
    {
        Jobs,
        Map
    }

    public delegate Task StartingLoadingData();

    public class JobsViewModelData : ViewModelBase
    {
        private static readonly AsyncLock Mutex = new AsyncLock();
        private readonly IAnalyticsService _analyticsService;
        private readonly IAppointmentService _appointmentService;

        public JobsViewModelData(IAppointmentService appointmentService, IAnalyticsService analyticsService)
        {
            _appointmentService = appointmentService;
            _analyticsService = analyticsService;

            Appointments = new ObservableCollection<AppointmentModel>();
            this.Subscribe<JobSelected>(async m =>
            {
                await NavigationService.NavigateToJobDetailsAsync();
                this.Publish(new ViewJobDetails(m.JobId));
            });

            this.Subscribe<NewDateSelected>(async m =>
            {
                await LoadDate(m.DateTime);
            });

            Date = DateTime.Today;
        }

        public event StartingLoadingData StartingLoadingData;
        public event StartingLoadingData FinishedLoadingData;

        private Source _source;
        public Source Source
        {
            get { return _source; }
            set
            {
                if (value != _source)
                {
                    SetPropertyValue(ref _source, value);
                }
            }
        }

        public ObservableCollection<AppointmentModel> Appointments { get; set; }

        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (value != Date)
                {
                    SetPropertyValue(ref _date, value);
                    this.Publish(new NewDateSelected(Date));
                }
            }
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
            set { SetPropertyValue(ref _isRefreshing, value); RaisePropertyChanged("CanNavigate"); }
        }

        public bool CanNavigate => !IsRefreshing;

        public ICommand GoToNextDay
        {
            get
            {
                return new Command(() =>
                {
                    _analyticsService.Track("Navigate to next day");
                    this.Publish(new NewDateSelected(Date.AddDays(1)));
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
                    this.Publish(new NewDateSelected(Date.AddDays(-1)));
                });
            }
        }

        public ICommand ShowCalendarView
        {
            get
            {
                return new Command(async () =>
                {
                    var vm = DependencyResolver.Resolve<CalendarViewModel>();
                    //vm.Source = Source.Jobs;
                    await NavigationService.ShowModalAsync<CalendarPage, CalendarViewModel>(vm);
                });
            }
        }

        public ICommand RefreshListOfJobsCommand
        {
            get
            {
                return new Command(async () =>
                {
                    _analyticsService.Track("Refreshing Jobs");
                    await LoadDate(Date, true);
                    OnAppearing();
                });
            }
        }

        public async Task LoadDate(DateTime date, bool forceRefresh = false)
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                // Date = date;
                SetPropertyValue(ref _date, date, "Date");
                if (!IsRefreshing)
                {
                    if(StartingLoadingData != null)
                        await StartingLoadingData.Invoke();

                    IsRefreshing = true;
                    var appointments = await _appointmentService.AppointmentsByDay(Date, forceRefresh);
                    Appointments.Clear();
                    foreach (var appointment in appointments.Where(a => !a.IsCancelled).OrderBy(a => a.Start))
                    {
                        Appointments.Add(new AppointmentModel
                        {
                            Title = appointment.Title,
                            CustomerName = appointment.CustomerName,
                            Location = appointment.Location,
                            LocationLatitude = appointment.LocationLatitude,
                            LocationLongitude = appointment.LocationLongitude,
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
                    if (FinishedLoadingData != null)
                        await FinishedLoadingData.Invoke();
                }
            }
        }

        public override void Dispose()
        {
            Appointments.Clear();
        }

        public override async void OnAppearing()
        {
            base.OnAppearing();
            
            IsRefreshing = false;

            await LoadDate(Date);
        }
    }
}