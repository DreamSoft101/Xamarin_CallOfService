using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.Gms.Gcm;
using Android.OS;
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
            UserDialogs.Init(this);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            //global::Xamarin.FormsMaps.Init(this, bundle);
            SvgImageRenderer.Init();
            DependencyResolver.Initialize(new AndroidModule(), new FormsModule());
            LoadApplication(new App());

            CrashManager.Register(this, "635a7d2e041a42fca3421315597b6e5e");

            Toast.MakeText(this, "Starting location check service ...", ToastLength.Short).Show();
            var pt = new PeriodicTask.Builder()
                .SetPeriod(600) // in seconds; minimum is 30 seconds
                .SetService(Java.Lang.Class.FromType(typeof(LocationService)))
                .SetRequiredNetwork(0)
                .SetTag("com.callofservice.mobile") // package name
                .Build();

            GcmNetworkManager.GetInstance(this).Schedule(pt);
        }
    }
}

