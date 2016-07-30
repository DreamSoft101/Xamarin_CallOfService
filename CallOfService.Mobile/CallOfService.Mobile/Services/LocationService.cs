using System;
using System.Linq;
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

        public async Task<bool> SendCurrentLocationUpdate(Action<Position> resultingPositionAction = null, Position lastReportedPosition = null, double accuracy = 50, bool disableWorkingHoursCheck = false, int timeoutInMillisecondsSeconds = 10000)
        {
            try
            {
                var locator = CrossGeolocator.Current;
                if (locator.IsGeolocationAvailable && locator.IsGeolocationEnabled)
                {
                    if (!disableWorkingHoursCheck && await IsOutsideWorkingHours())
                        return false;

                    locator.DesiredAccuracy = accuracy;
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

        private async Task<bool> IsOutsideWorkingHours()
        {
            var defaultDayStart = TimeSpan.FromHours(6);
            var defaultDayEnd = TimeSpan.FromHours(20);

            var now = DateTime.Now;
            var currentTimeOfDay = now.TimeOfDay;
            try
            {
                var availability = await _locationProxy.GetAvailability();
                if (availability?.Availabilities == null || !availability.Availabilities.Any())
                    return IsWithinRange(currentTimeOfDay, defaultDayStart, defaultDayEnd);

                var todaysAvailability = availability.Availabilities.SingleOrDefault(a => a.DayOfWeek.ToLower() == now.DayOfWeek.ToString().ToLower());
                if (todaysAvailability?.From == null || todaysAvailability?.To == null)
                    return IsWithinRange(currentTimeOfDay, defaultDayStart, defaultDayEnd);

                if (!todaysAvailability.IsAvailable)
                    return false;

                if (currentTimeOfDay >= todaysAvailability.From && currentTimeOfDay <= todaysAvailability.To)
                    return true;

                return false;
            }
            catch (Exception e)
            {
                //If no availability info then default to 6am to 8pm
                return IsWithinRange(currentTimeOfDay, defaultDayStart, defaultDayEnd);
            }
        }

        private bool IsWithinRange(TimeSpan value, TimeSpan from, TimeSpan to)
        {
            return value >= from && value <= to;
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

                return await locator.StartListeningAsync(TimeSpan.FromMinutes(5).Milliseconds, 50);// Update every 5 minutes and 50 meters
            }
            return false;
        }

        private async void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            try
            {
                LocationUpdated?.Invoke(this, e);
                await _locationProxy.SendLocation(e.Position.Latitude, e.Position.Longitude);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
