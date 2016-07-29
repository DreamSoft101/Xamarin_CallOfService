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
            global::Xamarin.FormsMaps.Init(this, bundle);
            SvgImageRenderer.Init();
            DependencyResolver.Initialize(new AndroidModule(), new FormsModule());
            LoadApplication(new App());

            CrashManager.Register(this, "635a7d2e041a42fca3421315597b6e5e");

            try
            {
                var pt = new PeriodicTask.Builder()
                    .SetPeriod(1800) // 30 minutes. Number is in seconds; minimum is 30 seconds
                    .SetService(Java.Lang.Class.FromType(typeof(LocationTaskService)))
                    .SetRequiredNetwork(0)
                    .SetTag("com.callofservice.mobile") // package name
                    .Build();
                GcmNetworkManager.GetInstance(this).Schedule(pt);
            }
            catch (Exception e)
            {
                //Toast.MakeText(this, "Error while scheduling location update", ToastLength.Long).Show();
            }
        }
    }
}

