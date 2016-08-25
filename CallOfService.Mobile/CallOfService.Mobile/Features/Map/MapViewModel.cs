using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CallOfService.Mobile.Domain;
using CallOfService.Mobile.Features.Jobs;
using CallOfService.Mobile.Proxies.Abstratcs;
using CallOfService.Mobile.Services.Abstracts;
using CallOfService.Mobile.UI;
using TK.CustomMap;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CallOfService.Mobile.Features.Map
{
    public class MapViewModel : ViewModelBase
    {
        private readonly IAppointmentProxy _appointmentProxy;
        private readonly ILocationService _locationService;
        private readonly IUserDialogs _userDialogs;
        private readonly double _rotation = 360 / Math.Pow(2, 15);

        public MapViewModel(IAppointmentProxy appointmentProxy, ILocationService locationService, IUserDialogs userDialogs)
        {
            _appointmentProxy = appointmentProxy;
            _locationService = locationService;
            _userDialogs = userDialogs;

            Pins = new ObservableCollection<TKCustomMapPin>();
            Region = new MapSpan(new Position(34.5133, -94.1629), _rotation, _rotation);
        }

        public ObservableCollection<TKCustomMapPin> Pins { get; set; }

        private JobsViewModelData _model;
        public JobsViewModelData Model
        {
            get { return _model; }
            set
            {
                SetPropertyValue(ref _model, value);
                Model.StartingLoadingData += StartingLoadingData;
                Model.FinishedLoadingData += FinishedLoadingData;
            }
        }

        private MapSpan _region;
        public MapSpan Region
        {
            get { return _region; }
            set { SetPropertyValue(ref _region, value); }
        }

        public override void Dispose()
        {
            Pins.Clear();
        }

        private Task StartingLoadingData()
        {
            var tcs = new TaskCompletionSource<bool>();
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    _userDialogs.ShowLoading("Loading...");
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return tcs.Task;
        }

        private Task FinishedLoadingData()
        {
            var tcs = new TaskCompletionSource<bool>();
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    Pins.Clear();

                    foreach (var appointment in Model.Appointments)
                    {
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

                    _userDialogs.HideLoading();

                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return tcs.Task;
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
