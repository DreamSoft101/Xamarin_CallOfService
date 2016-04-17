using System;
using CallOfService.Technician.Mobile.Messages;
using CallOfService.Technician.Mobile.UI;
using PubSub;
using Xamarin.Forms.Maps;

namespace CallOfService.Technician.Mobile.Features.JobDetails
{
    public partial class JobDetailsPage : BasePage , IDisposable
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
                    Label = m.Contact,
                    Address = m.Location
                });

                var latlongdegrees = 360/(Math.Pow(2, 15));
                Map.MoveToRegion(new MapSpan(m.GpsPoint.Position, latlongdegrees, latlongdegrees));
            });
        }

        public void Dispose()
        {
            this.Unsubscribe<ShowPinOnMap>();
            Content = null;
        }
    }
}