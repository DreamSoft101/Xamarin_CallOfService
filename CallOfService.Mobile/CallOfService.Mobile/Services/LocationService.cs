using System;
using System.Threading.Tasks;
using CallOfService.Mobile.Proxies.Abstratcs;
using CallOfService.Mobile.Services.Abstracts;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;

namespace CallOfService.Mobile.Services
{
    public class LocationService : ILocationService
    {
        public const double Tolerance = 0.0000001;
        private readonly ILocationProxy _locationProxy;

        public LocationService(ILocationProxy locationProxy)
        {
            _locationProxy = locationProxy;
        }

        public async Task<bool> SendCurrentLocationUpdate(Action<Position> resultingPositionAction = null, Position lastReportedPosition = null, int timeoutInMillisecondsSeconds = 10000)
        {
            try
            {
                var locator = CrossGeolocator.Current;
                if (locator.IsGeolocationAvailable && locator.IsGeolocationEnabled)
                {
                    //locator.DesiredAccuracy = 50;

                    var currentLocation = await locator.GetPositionAsync(timeoutInMillisecondsSeconds);

                    if (lastReportedPosition == null ||
                        (Math.Abs(currentLocation.Latitude - lastReportedPosition.Latitude) > Tolerance ||
                         Math.Abs(currentLocation.Longitude - lastReportedPosition.Longitude) > Tolerance))
                    {
                        await _locationProxy.SendLocation(currentLocation.Latitude, currentLocation.Longitude);
                        resultingPositionAction?.Invoke(currentLocation);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return false;
        }

        public EventHandler<PositionEventArgs> LocationUpdated { get; set; }
        public async Task<bool> StartListening()
        {
            var locator = CrossGeolocator.Current;
            locator.AllowsBackgroundUpdates = true;
            if (locator.IsGeolocationAvailable && locator.IsGeolocationEnabled)
            {
                if (locator.IsListening)
                    await locator.StopListeningAsync();

                locator.PositionChanged += Locator_PositionChanged;

                return await locator.StartListeningAsync(60000, 10);
            }
            return false;
        }

        private async void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            try
            {
                LocationUpdated.Invoke(this, e);
                await _locationProxy.SendLocation(e.Position.Latitude, e.Position.Longitude);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
