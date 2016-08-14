using System;
using System.Threading.Tasks;
using Plugin.Geolocator.Abstractions;

namespace CallOfService.Mobile.Services.Abstracts
{
    public interface ILocationService
    {
        Task<bool> SendCurrentLocationIfUpdated(double accuracy = 50, bool disableWorkingHoursCheck = false, int timeoutInMillisecondsSeconds = 10000, Action<Position> resultingPositionAction = null);
        Task<Position> GetCurrentLocation(double accuracy = 50, int timeoutInMilliseconds = 10000);
        Task<bool> SendLocatoinIfUpdated(double latitude, double longitude, bool disableWorkingHoursCheck = false);
    }
}
