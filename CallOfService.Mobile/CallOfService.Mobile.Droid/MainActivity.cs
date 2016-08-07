using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Util;
using Android.Widget;
using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Droid.Core.DI;
using CallOfService.Mobile.Droid.Services;
using HockeyApp.Android;
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
            UserDialogs.Init(() => this);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            global::Xamarin.FormsMaps.Init(this, bundle);
            SvgImageRenderer.Init();
            DependencyResolver.Initialize(new AndroidModule(), new FormsModule());
            LoadApplication(new App());

            CrashManager.Register(this, "635a7d2e041a42fca3421315597b6e5e");

            LocationApp.Current.LocationServiceConnected += (sender, e) =>
            {
                Log.Debug("MainActivity", "ServiceConnected Event Raised");
                LocationApp.Current.LocationService.LocationSentToServer += HandleLocationSentToServer;
                LocationApp.Current.LocationService.LocationChanged += HandleLocationChanged;
                LocationApp.Current.LocationService.ProviderDisabled += HandleProviderDisabled;
                LocationApp.Current.LocationService.ProviderEnabled += HandleProviderEnabled;
                LocationApp.Current.LocationService.StatusChanged += HandleStatusChanged;
            };
            StartLocatoinService();
        }

        protected override void OnDestroy()
        {
            Log.Debug("MainActivity", "OnDestroy");
            //LocationApp.StopLocationService();
            base.OnDestroy();
        }

        private void StartLocatoinService()
        {
            LocationApp.StopLocationService();
            LocationApp.StartLocationService();
        }

        public void HandleLocationSentToServer(object sender, LocationChangedEventArgs e)
        {
            Log.Debug("MainActivity", "Location sent to server toast");
            RunOnUiThread(() =>
            {
                Toast.MakeText(this, "Current location sent to Call of Service", ToastLength.Long).Show();
            });
        }

        public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            Log.Debug("MainActivity", "Location updated");
        }

        public void HandleProviderDisabled(object sender, ProviderDisabledEventArgs e)
        {
            Log.Debug("MainActivity", "Location provider disabled event raised");
        }

        public void HandleProviderEnabled(object sender, ProviderEnabledEventArgs e)
        {
            Log.Debug("MainActivity", "Location provider enabled event raised");
        }

        public void HandleStatusChanged(object sender, StatusChangedEventArgs e)
        {
            Log.Debug("MainActivity", "Location status changed, event raised");
        }
    }
}
