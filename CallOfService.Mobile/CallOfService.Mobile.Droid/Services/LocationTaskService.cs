using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Util;
using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Proxies.Abstratcs;
using CallOfService.Mobile.Services.Abstracts;

namespace CallOfService.Mobile.Droid.Services
{
    [Service]
    public class LocationTaskService : Service, ILocationListener
    {
        protected LocationManager LocationManager = Application.Context.GetSystemService("location") as LocationManager;

        public event EventHandler<LocationChangedEventArgs> LocationSentToServer = delegate { };
        public event EventHandler<LocationChangedEventArgs> LocationChanged = delegate { };
        public event EventHandler<ProviderDisabledEventArgs> ProviderDisabled = delegate { };
        public event EventHandler<ProviderEnabledEventArgs> ProviderEnabled = delegate { };
        public event EventHandler<StatusChangedEventArgs> StatusChanged = delegate { };

        private IBinder binder;
        public override IBinder OnBind(Intent intent)
        {
            Log.Debug("LocationTaskService", "Client now bound to service");

            binder = new LocationTaskServiceBinder(this);
            return binder;
        }

        public void StartLocationUpdates()
        {
            var locationCriteria = new Criteria
            {
                Accuracy = Accuracy.NoRequirement,
                PowerRequirement = Power.NoRequirement
            };
            var locationProvider = LocationManager.GetBestProvider(locationCriteria, true);

            Log.Debug("LocationTaskService", $"You are about to get location updates via {locationProvider}");
            LocationManager.RequestLocationUpdates(locationProvider, TimeSpan.FromMinutes(15).Milliseconds, 50, this); // ToDo: Replace with TimeSpan.FromMinutes(15).Milliseconds after testing
            Log.Debug("LocationTaskService", "Now sending location updates");
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Log.Debug("LocationTaskService", "OnCreate called in the Location Service");
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Log.Debug("LocationTaskService", "Service has been terminated");

            LocationManager.RemoveUpdates(this);
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("LocationTaskService", "LocationService started");
            return StartCommandResult.Sticky;
        }

        public void OnLocationChanged(Location location)
        {
            LocationChanged(this, new LocationChangedEventArgs(location));

            Task.Run(async () =>
            {
                try
                {
                    var locationProxy = DependencyResolver.Resolve<ILocationProxy>();
                    var userService = DependencyResolver.Resolve<IUserService>();
                    var userCredentials = userService.GetUserCredentials();

                    if (!string.IsNullOrEmpty(userCredentials?.Token) && await IsWithinWorkingHours(locationProxy))
                    {
                        await locationProxy.SendLocation(location.Latitude, location.Longitude);
                        LocationSentToServer(this, new LocationChangedEventArgs(location));
                    }
                }
                catch (Exception e)
                {
                    Log.Debug("LocationTaskService", $"Exception while sending location update: {e}");
                }
            });

            Log.Debug("LocationTaskService", $"Latitude is {location.Latitude}");
            Log.Debug("LocationTaskService", $"Longitude is {location.Longitude}");
        }

        public void OnProviderDisabled(string provider)
        {
            Log.Debug("LocationTaskService", $"Provider {provider} is disabled");
            ProviderDisabled(this, new ProviderDisabledEventArgs(provider));
        }

        public void OnProviderEnabled(string provider)
        {
            Log.Debug("LocationTaskService", $"Provider {provider} is enabled");
            ProviderEnabled(this, new ProviderEnabledEventArgs(provider));
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            Log.Debug("LocationTaskService", $"Provider {provider} has changed its status to {status}");
            StatusChanged(this, new StatusChangedEventArgs(provider, status, extras));
        }

        private static async Task<bool> IsWithinWorkingHours(ILocationProxy locationProxy)
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
                Log.Debug("LocationTaskService", $"Exception while checking working hours: {e}");
                return IsWithinRange(currentTimeOfDay, defaultDayStart, defaultDayEnd);
            }
        }

        private static bool IsWithinRange(TimeSpan value, TimeSpan from, TimeSpan to)
        {
            return value >= from && value <= to;
        }
    }

    public class LocationTaskServiceBinder : Binder
    {
        public LocationTaskService Service { get; }

        public bool IsBound { get; set; }
        public LocationTaskServiceBinder(LocationTaskService service) { Service = service; }
    }

    public class LocationServiceConnection : Java.Lang.Object, IServiceConnection
    {
        public LocationTaskServiceBinder Binder { get; set; }

        public LocationServiceConnection(LocationTaskServiceBinder binder)
        {
            if (binder != null)
            {
                Binder = binder;
            }
        }

        public event EventHandler<ServiceConnectedEventArgs> ServiceConnected = delegate { };

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            var serviceBinder = service as LocationTaskServiceBinder;

            if (serviceBinder != null)
            {
                Binder = serviceBinder;
                Binder.IsBound = true;

                // raise the service bound event
                ServiceConnected(this, new ServiceConnectedEventArgs() { Binder = service });

                // begin updating the location in the Service
                serviceBinder.Service.StartLocationUpdates();
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            Binder.IsBound = false;
            Log.Debug("ServiceConnection", "Service unbound");
        }
    }

    public class ServiceConnectedEventArgs : EventArgs
    {
        public IBinder Binder { get; set; }
    }

    public class LocationApp
    {
        public static LocationApp Current { get; }
        static LocationApp()
        {
            Current = new LocationApp();
        }

        public event EventHandler<ServiceConnectedEventArgs> LocationServiceConnected = delegate { };
        protected static LocationServiceConnection LocationServiceConnection;

        public LocationTaskService LocationService
        {
            get
            {
                if (LocationServiceConnection.Binder == null)
                    throw new Exception("Service not bound yet");
                return LocationServiceConnection.Binder.Service;
            }
        }

        protected LocationApp()
        {
            LocationServiceConnection = new LocationServiceConnection(null);

            LocationServiceConnection.ServiceConnected += (sender, e) => 
            {
                Log.Debug("LocationApp", "Service Connected");
                // we will use this event to notify MainActivity when to start updating the UI
                LocationServiceConnected(this, e);
            };
        }

        public static void StartLocationService()
        {
            new Task(() =>
            {
                Log.Debug("LocationApp", "Calling StartService");
                Application.Context.StartService(new Intent(Application.Context, typeof(LocationTaskService)));

                Log.Debug("LocationApp", "Calling service binding");
                var locationServiceIntent = new Intent(Application.Context, typeof(LocationTaskService));
                Application.Context.BindService(locationServiceIntent, LocationServiceConnection, Bind.AutoCreate);
            }).Start();
        }

        public static void StopLocationService()
        {
            Log.Debug("LocationApp", "StopLocationService");
            if (LocationServiceConnection != null)
            {
                Log.Debug("LocationApp", "Unbinding from LocationService");
                Application.Context.UnbindService(LocationServiceConnection);
            }

            if (Current.LocationService != null)
            {
                Log.Debug("LocationApp", "Stopping the LocationService");
                Current.LocationService.StopSelf();
            }
        }
    }
}