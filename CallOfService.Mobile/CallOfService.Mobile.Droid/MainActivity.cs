using System;
using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.Gms.Gcm;
using Android.OS;
using Android.Widget;
using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Droid.Core.DI;
using CallOfService.Mobile.Droid.Services;
using CallOfService.Mobile.Services.Abstracts;
using HockeyApp.Android;
using Plugin.Geolocator.Abstractions;
using TwinTechs.Droid;

namespace CallOfService.Mobile.Droid
{
    [Activity(Label = "Call Of Service", Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            UserDialogs.Init(this);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            //global::Xamarin.FormsMaps.Init(this, bundle);
            SvgImageRenderer.Init();
            DependencyResolver.Initialize(new AndroidModule(), new FormsModule());
            LoadApplication(new App());

            CrashManager.Register(this, "635a7d2e041a42fca3421315597b6e5e");

            try
            {
                var pt = new PeriodicTask.Builder()
                    .SetPeriod(600) // 10 minutes. number is in seconds; minimum is 30 seconds
                    .SetService(Java.Lang.Class.FromType(typeof(LocationTaskService)))
                    .SetRequiredNetwork(0)
                    .SetTag("com.callofservice.mobile") // package name
                    .Build();
                GcmNetworkManager.GetInstance(this).Schedule(pt);

                var userService = DependencyResolver.Resolve<IUserService>();
                var locationService = DependencyResolver.Resolve<ILocationService>();

                System.Threading.Tasks.Task.Run(async () =>
                {
                    try
                    {
                        var userCredentials = userService.GetUserCredentials();

                        if (!string.IsNullOrEmpty(userCredentials?.Token))
                        {
                            var locationSentSuccessfully = await locationService.SendCurrentLocationUpdate(position =>
                            {
                                var builder = new Notification.Builder(Application.Context)
                                      .SetContentTitle("Location Update Inititated")
                                      .SetContentText($"Location update has been sent: Lat={position.Latitude}, Lng={position.Longitude}")
                                      .SetSmallIcon(Resource.Drawable.icon);
                                var notification = builder.Build();
                                var notificationManager = (NotificationManager)GetSystemService(NotificationService);

                                var notificationId = Guid.NewGuid().GetHashCode();
                                notificationManager.Notify(notificationId, notification);
                            });
                        }

                        locationService.LocationUpdated += LocationUpdated;
                        var locationUpdateRegisteredSuccessfully = await locationService.StartListening();
                    }
                    catch (Exception e)
                    {
                        Toast.MakeText(Application.Context, "Error while sending location update", ToastLength.Long).Show();
                    }
                });
            }
            catch (Exception e)
            {
                Toast.MakeText(this, "Error while sending location update", ToastLength.Long).Show();
            }
        }

        private void LocationUpdated(object sender, PositionEventArgs e)
        {
            var builder = new Notification.Builder(Application.Context)
                                      .SetContentTitle("Location Update Triggered")
                                      .SetContentText($"Location update has been sent: Lat={e.Position.Latitude}, Lng={e.Position.Longitude}")
                                      .SetSmallIcon(Resource.Drawable.icon);
            var notification = builder.Build();
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);

            var notificationId = Guid.NewGuid().GetHashCode();
            notificationManager.Notify(notificationId, notification);
        }
    }
}

