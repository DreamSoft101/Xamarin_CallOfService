using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Util;
using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Services.Abstracts;

namespace CallOfService.Mobile.Droid.Services
{
    [Service()]
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

            LocationManager.RequestLocationUpdates(locationProvider, TimeSpan.FromMinutes(9).Milliseconds, 50, this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            LocationManager.RemoveUpdates(this);
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            return StartCommandResult.Sticky;
        }

        public void OnLocationChanged(Location location)
        {
            LocationChanged(this, new LocationChangedEventArgs(location));

            Task.Run(async () =>
            {
                try
                {
                    var locationService = DependencyResolver.Resolve<ILocationService>();
                    var updated = await locationService.SendLocatoinIfUpdated(location.Latitude, location.Longitude);
                    if(updated) LocationSentToServer(this, new LocationChangedEventArgs(location));
                }
                catch (Exception e)
                {
                    try
                    {
                        var logger = DependencyResolver.Resolve<ILogger>();
                        logger.WriteWarning("Exception while handling location changed", exception: e);
                    }
                    catch
                    {

                    }
                }
            });
        }

        public void OnProviderDisabled(string provider)
        {
            ProviderDisabled(this, new ProviderDisabledEventArgs(provider));
        }

        public void OnProviderEnabled(string provider)
        {
            ProviderEnabled(this, new ProviderEnabledEventArgs(provider));
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            StatusChanged(this, new StatusChangedEventArgs(provider, status, extras));
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

                ServiceConnected(this, new ServiceConnectedEventArgs { Binder = service });

                try
                {
                    var logger = DependencyResolver.Resolve<ILogger>();
                    logger.WriteInfo("Starting location service...");
                }
                catch
                {

                }

                serviceBinder.Service.StartLocationUpdates();
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            Binder.IsBound = false;
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
                    return null;
                    //throw new Exception("Service not bound yet");
                return LocationServiceConnection.Binder.Service;
            }
        }

        protected LocationApp()
        {
            LocationServiceConnection = new LocationServiceConnection(null);

            LocationServiceConnection.ServiceConnected += (sender, e) => 
            {
                // we will use this event to notify MainActivity when to start updating the UI
                LocationServiceConnected(this, e);
            };
        }

        public static void StartLocationService()
        {
            new Task(() =>
            {
                Application.Context.StartService(new Intent(Application.Context, typeof(LocationTaskService)));

                var locationServiceIntent = new Intent(Application.Context, typeof(LocationTaskService));
                Application.Context.BindService(locationServiceIntent, LocationServiceConnection, Bind.AutoCreate);
            }).Start();
        }

        public static void StopLocationService()
        {
            if (LocationServiceConnection != null)
            {
                try
                {
                    Application.Context.UnbindService(LocationServiceConnection);
                }
                catch (Exception e)
                {
                    try
                    {
                        var logger = DependencyResolver.Resolve<ILogger>();
                        logger.WriteWarning("Exception while stopping location service", exception: e);
                    }
                    catch
                    {
                        
                    }
                }
            }

            Current.LocationService?.StopSelf();
        }
    }
}