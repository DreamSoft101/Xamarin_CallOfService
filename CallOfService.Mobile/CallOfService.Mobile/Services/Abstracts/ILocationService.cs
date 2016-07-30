using System;
using System.Threading.Tasks;
using Plugin.Geolocator.Abstractions;

namespace CallOfService.Mobile.Services.Abstracts
{
    public interface ILocationService
    {
        Task<bool> SendCurrentLocationUpdate(Action<Position> resultingPositionAction = null, Position lastReportedPosition = null, double accuracy = 50, bool disableWorkingHoursCheck = false, int timeoutInMillisecondsSeconds = 10000);
        Task<bool> StartListening();
        EventHandler<PositionEventArgs> LocationUpdated { get; set; }
    }
}
