using System;
using System.Threading.Tasks;
using CallOfService.Mobile.Core;
using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Messages;
using CallOfService.Mobile.Services.Abstracts;
using CallOfService.Mobile.UI;
using PubSub;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CallOfService.Mobile.Features.JobDetails
{
    public partial class JobDetailsPage : BasePage, IDisposable
    {
        public JobDetailsPage()
        {
            InitializeComponent();
            this.Subscribe<ShowPinOnMap>(m =>
            {
                Map.Pins.Add(new Pin
                {
                    Type = PinType.Place,
                    Position = m.GpsPoint.Position,
                    Label = m.Contact ?? string.Empty,
                    Address = m.Location
                });

                var latlongdegrees = 360 / Math.Pow(2, 15);

                Map.MoveToRegion(new MapSpan(m.GpsPoint.Position, latlongdegrees, latlongdegrees));

                Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        var locationService = DependencyResolver.Resolve<ILocationService>();
                        var currentLocation = await locationService.GetCurrentLocation(timeoutInMilliseconds: 2000);

                        if (currentLocation != null)
                        {
                            var distance = GetDistance(m.GpsPoint.Position.Latitude, m.GpsPoint.Position.Longitude, currentLocation.Latitude, currentLocation.Longitude, 'K');
                            if (distance < 50)
                            {
                                var midPoint = new Position((m.GpsPoint.Position.Latitude + currentLocation.Latitude)/2,
                                    (m.GpsPoint.Position.Longitude + currentLocation.Longitude)/2);
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    //Map.Pins.Add(new Pin
                                    //{
                                    //    Type = PinType.Generic,
                                    //    Position = new Position(currentLocation.Latitude, currentLocation.Longitude),
                                    //    Label = "Me"
                                    //});
                                    Map.MoveToRegion(MapSpan.FromCenterAndRadius(midPoint, Distance.FromKilometers(distance*1.2)));
                                });
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                });
            });
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

        public void Dispose()
        {
            this.Unsubscribe<ShowPinOnMap>();
            Content = null;
        }
    }
}