using System;
using Android.App;
using Android.Gms.Gcm;
using Android.OS;
using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Services.Abstracts;
using Plugin.Geolocator.Abstractions;

namespace CallOfService.Mobile.Droid.Services
{
    [Service(Exported = true, Permission = "com.google.android.gms.permission.BIND_NETWORK_TASK_SERVICE")]
    [IntentFilter(new[] { "com.google.android.gms.gcm.ACTION_TASK_READY" })]
    public class LocationTaskService : GcmTaskService
    {
        public static bool IsRunning { get; set; }
        public static Position LastReportedPosition { get; set; }
        public const double Tolerance = 0.0000001;

        public override int OnRunTask(TaskParams @params)
        {
            if (IsRunning)
                return GcmNetworkManager.ResultSuccess;

            IsRunning = true;
            try
            {
                var userService = DependencyResolver.Resolve<IUserService>();
                var locationService = DependencyResolver.Resolve<ILocationService>();

                System.Threading.Tasks.Task.Run(async () =>
                {
                    try
                    {
                        var userCredentials = userService.GetUserCredentials();

                        if (!string.IsNullOrEmpty(userCredentials?.Token))
                        {
                            await locationService.SendCurrentLocationUpdate(null, LastReportedPosition);
                        }
                    }
                    catch (Exception e)
                    {
                    }
                    finally
                    {
                        IsRunning = false;
                    }
                });
            }
            catch (Exception e)
            {
                IsRunning = false;
            }

            return GcmNetworkManager.ResultSuccess;
        }
    }

    public class LocationServiceBinder : Binder
    {
        private readonly LocationTaskService taskService;

        public LocationServiceBinder(LocationTaskService taskService)
        {
            this.taskService = taskService;
        }

        public LocationTaskService GetLocationService()
        {
            return taskService;
        }
    }
}