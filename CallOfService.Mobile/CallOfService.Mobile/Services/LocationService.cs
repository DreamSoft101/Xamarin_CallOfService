using System;
using System.Linq;
using System.Threading.Tasks;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Proxies.Abstratcs;
using CallOfService.Mobile.Services.Abstracts;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;

namespace CallOfService.Mobile.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationProxy _locationProxy;
        private readonly ILogger _logger;
        private Position _lastReportedPosition;
        private DateTime _lastCheckedTime = DateTime.MinValue;

        public const double Tolerance = 0.0000001;

        public LocationService(ILocationProxy locationProxy, ILogger logger)
        {
            _locationProxy = locationProxy;
            _logger = logger;
        }

        public async Task<Position> GetCurrentLocation(double accuracy = 50, int timeoutInMillisecondsSeconds = 10000)
        {
            if (PositionIsUpToDate())
                return _lastReportedPosition;

            try
            {
                var locator = CrossGeolocator.Current;
                if (locator.IsGeolocationAvailable && locator.IsGeolocationEnabled)
                {
                    locator.DesiredAccuracy = accuracy;
                    var result = await locator.GetPositionAsync(timeoutInMillisecondsSeconds);
                    UpdateLastPosition(result);
                    return result;
                }
            }
            catch (Exception e)
            {
                _logger.WriteError("Exceptoin while getting current location", exception: e);
            }

            return null;
        }

        public async Task<bool> SendLocatoinIfUpdated(double latitude, double longitude, bool disableWorkingHoursCheck = false)
        {
            if (!disableWorkingHoursCheck && !await IsWithinWorkingHours(_locationProxy, _logger))
                return false;
            if (PositionIsUpToDate())
                return false;

            var currentLocation = new Position { Latitude = latitude, Longitude = longitude };
            if (_lastReportedPosition == null ||
                        (Math.Abs(currentLocation.Latitude - _lastReportedPosition.Latitude) > Tolerance ||
                         Math.Abs(currentLocation.Longitude - _lastReportedPosition.Longitude) > Tolerance))
            {
                await _locationProxy.SendLocation(currentLocation.Latitude, currentLocation.Longitude);
                UpdateLastPosition(currentLocation);
                return true;
            }

            return false;
        }

        public async Task<bool> SendCurrentLocationIfUpdated(double accuracy = 50, bool disableWorkingHoursCheck = false, int timeoutInMillisecondsSeconds = 10000, Action<Position> resultingPositionAction = null)
        {
            try
            {
                var locator = CrossGeolocator.Current;
                if (locator.IsGeolocationAvailable && locator.IsGeolocationEnabled)
                {
                    if (!disableWorkingHoursCheck && !await IsWithinWorkingHours(_locationProxy, _logger))
                        return false;

                    Position currentLocation;
                    if (PositionIsUpToDate())
                    {
                        currentLocation = _lastReportedPosition;
                    }
                    else
                    {
                        locator.DesiredAccuracy = accuracy;
                        currentLocation = await locator.GetPositionAsync(timeoutInMillisecondsSeconds);
                    }

                    if (_lastReportedPosition == null ||
                        (Math.Abs(currentLocation.Latitude - _lastReportedPosition.Latitude) > Tolerance ||
                         Math.Abs(currentLocation.Longitude - _lastReportedPosition.Longitude) > Tolerance))
                    {
                        await _locationProxy.SendLocation(currentLocation.Latitude, currentLocation.Longitude);
                        UpdateLastPosition(currentLocation);
                        resultingPositionAction?.Invoke(currentLocation);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.WriteError("Exceptoin while sending current location", exception: e);
                return false;
            }
            return false;
        }

        public async Task<bool> IsWithinWorkingHours(ILocationProxy locationProxy, ILogger logger)
        {
            var defaultDayStart = TimeSpan.FromHours(6);
            var defaultDayEnd = TimeSpan.FromHours(20);

            var now = DateTime.Now;
            var currentTimeOfDay = now.TimeOfDay;
            try
            {
                var availability = await locationProxy.GetAvailability();
                if (availability?.Availabilities == null || !availability.Availabilities.Any())
                    return IsWithinRange(currentTimeOfDay, defaultDayStart, defaultDayEnd);

                var todaysAvailability = availability.Availabilities.SingleOrDefault(a => a.DayOfWeek.ToLower() == now.DayOfWeek.ToString().ToLower());
                if (todaysAvailability?.From == null || todaysAvailability.To == null)
                    return IsWithinRange(currentTimeOfDay, defaultDayStart, defaultDayEnd);

                if (!todaysAvailability.IsAvailable)
                    return false;

                if (currentTimeOfDay >= todaysAvailability.From && currentTimeOfDay <= todaysAvailability.To)
                    return true;

                return false;
            }
            catch (Exception e)
            {
                _logger.WriteError("Exceptoin while checking working hours", exception: e);
                return IsWithinRange(currentTimeOfDay, defaultDayStart, defaultDayEnd);
            }
        }

        private static bool IsWithinRange(TimeSpan value, TimeSpan from, TimeSpan to)
        {
            return value >= from && value <= to;
        }

        private bool PositionIsUpToDate()
        {
            return _lastReportedPosition != null && (DateTime.Now - _lastCheckedTime) < TimeSpan.FromMinutes(5);
        }

        private void UpdateLastPosition(Position position)
        {
            _lastReportedPosition = position;
            _lastCheckedTime = DateTime.Now;
        }
    }
}
