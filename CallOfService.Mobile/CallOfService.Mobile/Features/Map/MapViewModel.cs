using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using CallOfService.Mobile.Core;
using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Domain;
using CallOfService.Mobile.Features.Calendar;
using CallOfService.Mobile.Features.Jobs;
using CallOfService.Mobile.Messages;
using CallOfService.Mobile.Proxies.Abstratcs;
using CallOfService.Mobile.Services.Abstracts;
using CallOfService.Mobile.UI;
using PubSub;
using TK.CustomMap;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CallOfService.Mobile.Features.Map
{
    public class MapViewModel : ViewModelBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IAnalyticsService _analyticsService;
        private readonly IAppointmentProxy _appointmentProxy;
        private readonly ILocationService _locationService;
        private readonly IUserDialogs _userDialogs;
        private readonly double _rotation = 360 / Math.Pow(2, 15);

        public MapViewModel(IAnalyticsService analyticsService, IAppointmentService appointmentService, IAppointmentProxy appointmentProxy, ILocationService locationService, IUserDialogs userDialogs)
        {
            _analyticsService = analyticsService;
            _appointmentService = appointmentService;
            _appointmentProxy = appointmentProxy;
            _locationService = locationService;
            _userDialogs = userDialogs;

            Appointments = new ObservableCollection<AppointmentModel>();
            Pins = new ObservableCollection<TKCustomMapPin>();

            Region = new MapSpan(new Position(34.5133, -94.1629), _rotation, _rotation);

            this.Subscribe<JobSelected>(async m =>
            {
                await NavigationService.NavigateToJobDetailsAsync();
                this.Publish(new ViewJobDetails(m.JobId));
            });

            this.Subscribe<NewDateSelected>(async m =>
            {
                if (m.Source == Source.Map)
                    await LoadDate(m.DateTime);
                else
                {
                    Date = m.DateTime;
                }
            });

            Date = DateTime.Today;
        }

        public ObservableCollection<AppointmentModel> Appointments { get; set; }
        public ObservableCollection<TKCustomMapPin> Pins { get; set; }

        private MapSpan _region;
        public MapSpan Region
        {
            get { return _region; }
            set { SetPropertyValue(ref _region, value); }
        }

        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (value != Date)
                {
                    SetPropertyValue(ref _date, value);
                    //this.Publish(new NewDateSelected(Date, Source.Map));
                }
            }
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
                    this.Publish(new NewDateSelected(Date.AddDays(1), Source.Map));
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
                    this.Publish(new NewDateSelected(Date.AddDays(-1), Source.Map));
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
                    vm.Source = Source.Map;
                    await NavigationService.ShowModalAsync<CalendarPage, CalendarViewModel>(vm);
                });
            }
        }

        public override void Dispose()
        {
            Pins.Clear();
            Appointments.Clear();
        }

        public async Task LoadDate(DateTime date, bool forceRefresh = false)
        {
            Date = date;
            if (!IsRefreshing)
            {
                IsRefreshing = true;
                _userDialogs.ShowLoading("Loading...");

                var appointments = await _appointmentService.AppointmentsByDay(Date, forceRefresh);

                Pins.Clear();
                Appointments.Clear();

                foreach (var appointment in appointments.Where(a => !a.IsCancelled).OrderBy(a => a.Start))
                {
                    Appointments.Add(new AppointmentModel
                    {
                        Title = appointment.Title,
                        Location = appointment.Location,
                        LocationLatitude = appointment.LocationLatitude,
                        LocationLongitude = appointment.LocationLongitude,
                        IsFinished = appointment.IsFinished,
                        IsInProgress = appointment.IsInProgress,
                        IsCancelled = appointment.IsCancelled,
                        StartTimeEndTimeFormated = $"{appointment.Start.ToUniversalTime().ToString("hh:mm tt")} - {appointment.End.ToUniversalTime().ToString("hh:mm tt")}",
                        JobId = appointment.JobId
                    });

                    var position = appointment.LocationLatitude.HasValue && appointment.LocationLongitude.HasValue
                        ? new Position(appointment.LocationLatitude.Value, appointment.LocationLongitude.Value)
                        : await GetPosition(appointment.Location);
                    Pins.Add(new TKCustomMapPin
                    {
                        Title = appointment.Title,
                        Subtitle = appointment.CustomerName,
                        Position = position,
                        ShowCallout = true,
                        BindingContext = appointment.JobId
                    });
                    Region = new MapSpan(new Position(position.Latitude, position.Longitude), _rotation, _rotation);
                }

                var currentLocation = await _locationService.GetCurrentLocation();
                if (currentLocation != null)
                {
                    Pins.Add(new TKCustomMapPin
                    {
                        Title = "Me",
                        Position = new Position(currentLocation.Latitude, currentLocation.Longitude),
                        Image = ImageSource.FromFile("Marker.png")
                    });
                }

                var maxLat = Pins.Max(p => p.Position.Latitude);
                var maxLng = Pins.Max(p => p.Position.Longitude);
                var minLat = Pins.Min(p => p.Position.Latitude);
                var minLng = Pins.Min(p => p.Position.Longitude);
                var center = new Position((maxLat + minLat) / 2, (maxLng + minLng) / 2);
                var distance = GetDistance(minLat, minLng, maxLat, maxLng, 'K');

                Region = MapSpan.FromCenterAndRadius(center, Distance.FromKilometers(distance * 1.2));

                IsRefreshing = false;
                _userDialogs.HideLoading();
            }
        }

        public override async void OnAppearing()
        {
            base.OnAppearing();

            _analyticsService.Screen("Map");

            IsRefreshing = false;

            await LoadDate(Date);
        }

        private async Task<Position> GetPosition(string address)
        {
            var loc = await _appointmentProxy.GetJobLocation(new AddressInfo { FormattedAddress = address });
            return loc.Position;
        }

        private double GetDistance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(Deg2rad(lat1)) * Math.Sin(Deg2rad(lat2)) + Math.Cos(Deg2rad(lat1)) * Math.Cos(Deg2rad(lat2)) * Math.Cos(Deg2rad(theta));
            dist = Math.Acos(dist);
            dist = Rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            else if (unit == 'N')
            {
                dist = dist * 0.8684;
            }
            return (dist);
        }

        private double Deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private double Rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }
    }
}
