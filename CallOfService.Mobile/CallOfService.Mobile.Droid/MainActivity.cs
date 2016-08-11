using System;
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
using Gcm.Client;

namespace CallOfService.Mobile.Droid
{
    [Activity(Label = "Call Of Service", Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static MainActivity CurrentActivity { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            CurrentActivity = this;

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

            //try
            //{
            //    GcmClient.CheckDevice(this);
            //    GcmClient.CheckManifest(this);

            //    GcmClient.Register(this, NotificationConstants.SenderId);
            //}
            //catch (Java.Net.MalformedURLException)
            //{
            //    Toast.MakeText(this, "There was an error creating the client. Verify the URL.", ToastLength.Long).Show();
            //}
            //catch (Exception e)
            //{
            //    Toast.MakeText(this, e.Message, ToastLength.Long).Show();
            //}

            //LocationApp.Current.LocationServiceConnected += (sender, e) =>
            //{
            //    Log.Debug("MainActivity", "ServiceConnected Event Raised");
            //    LocationApp.Current.LocationService.LocationSentToServer += HandleLocationSentToServer;
            //    LocationApp.Current.LocationService.LocationChanged += HandleLocationChanged;
            //    LocationApp.Current.LocationService.ProviderDisabled += HandleProviderDisabled;
            //    LocationApp.Current.LocationService.ProviderEnabled += HandleProviderEnabled;
            //    LocationApp.Current.LocationService.StatusChanged += HandleStatusChanged;
            //};
            StartLocatoinService();
        }

        protected override void OnDestroy()
        {
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
            RunOnUiThread(() =>
            {
                Toast.MakeText(this, "Current location sent to Call of Service", ToastLength.Long).Show();
            });
        }

        public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
        }

        public void HandleProviderDisabled(object sender, ProviderDisabledEventArgs e)
        {
        }

        public void HandleProviderEnabled(object sender, ProviderEnabledEventArgs e)
        {
        }

        public void HandleStatusChanged(object sender, StatusChangedEventArgs e)
        {
        }
    }
}
