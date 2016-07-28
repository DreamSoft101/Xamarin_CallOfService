using System.Threading.Tasks;
using CallOfService.Mobile.Proxies.Abstratcs;
using CallOfService.Mobile.Services.Abstracts;
using Plugin.Geolocator;

namespace CallOfService.Mobile.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationProxy _locationProxy;

        public LocationService(ILocationProxy locationProxy)
        {
            _locationProxy = locationProxy;
        }

        public async Task StartSendLocation()
        {
            var locator = CrossGeolocator.Current;
            if (locator.IsGeolocationAvailable)
            {
                locator.AllowsBackgroundUpdates = true;
                locator.DesiredAccuracy = 1;

                var currentLocation = await locator.GetPositionAsync();
                await _locationProxy.SendLocation(currentLocation.Latitude, currentLocation.Longitude);

                locator.PositionChanged += async (sender, e) => {
                    var position = e.Position;
                    await _locationProxy.SendLocation(position.Latitude, position.Longitude);
                };
            }
            //return _locationProxy.SendLocation(double latitude, double longitude);
        }
    }
}