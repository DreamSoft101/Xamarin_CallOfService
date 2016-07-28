using System;
using Android.App;
using Android.Gms.Gcm;
using Android.OS;
using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Proxies.Abstratcs;
using CallOfService.Mobile.Services.Abstracts;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;

namespace CallOfService.Mobile.Droid.Services
{
    [Service(Exported = true, Permission = "com.google.android.gms.permission.BIND_NETWORK_TASK_SERVICE")]
    [IntentFilter(new[] { "com.google.android.gms.gcm.ACTION_TASK_READY" })]
    public class LocationService : GcmTaskService
    {
        public static bool IsRunning { get; set; }
        public static Position LastReportedPosition { get; set; }
        public const double Tolerance = 0.0000001;

        public override int OnRunTask(TaskParams @params)
        {
            if(IsRunning)
                return GcmNetworkManager.ResultSuccess;

            IsRunning = true;
            try
            {
                var locationProxy = DependencyResolver.Resolve<ILocationProxy>();
                var userService = DependencyResolver.Resolve<IUserService>();

                System.Threading.Tasks.Task.Run(async () =>
                {
                    try
                    {
                        var userCredentials = userService.GetUserCredentials();

                        if (!string.IsNullOrEmpty(userCredentials?.Token))
                        {
                            var locator = CrossGeolocator.Current;
                            if (locator.IsGeolocationAvailable && locator.IsGeolocationEnabled)
                            {
                                locator.AllowsBackgroundUpdates = true;
                                locator.DesiredAccuracy = 5;

                                var currentLocation = await locator.GetPositionAsync();

                                if (LastReportedPosition == null || (Math.Abs(currentLocation.Latitude - LastReportedPosition.Latitude) > Tolerance || Math.Abs(currentLocation.Longitude - LastReportedPosition.Longitude) > Tolerance))
                                {
                                    await locationProxy.SendLocation(currentLocation.Latitude, currentLocation.Longitude);
                                    LastReportedPosition = currentLocation;

                                    var builder = new Notification.Builder(Application.Context)
                                        .SetContentTitle("Location Update Sent")
                                        .SetContentText($"Location update has been sent: Lat={currentLocation.Latitude}, Lng={currentLocation.Longitude}")
                                        .SetSmallIcon(Resource.Drawable.icon);
                                    var notification = builder.Build();
                                    var notificationManager = (NotificationManager)GetSystemService(NotificationService);

                                    var notificationId = (Math.Abs(currentLocation.Latitude + currentLocation.Longitude)).GetHashCode();
                                    notificationManager.Notify(notificationId, notification);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        IsRunning = false;
                    }
                });
            }
            catch (Exception e)
            {
                IsRunning = false;
            }

            IsRunning = false;
            return GcmNetworkManager.ResultSuccess;
        }
    }

    public class LocationServiceBinder : Binder
    {
        private readonly LocationService service;

        public LocationServiceBinder(LocationService service)
        {
            this.service = service;
        }

        public LocationService GetLocationService()
        {
            return service;
        }
    }
}