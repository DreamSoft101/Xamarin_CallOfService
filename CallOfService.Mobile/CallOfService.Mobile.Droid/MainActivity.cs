using Android.App;
using Android.OS;
using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Droid.Core.DI;
using Acr.UserDialogs;
using Android.Content.PM;
using Android.Util;
using HockeyApp;
using TwinTechs.Droid;

namespace CallOfService.Mobile.Droid
{
	[Activity (Label = "Call Of Service", 
	           Icon = "@mipmap/icon",
	           Theme = "@style/MyTheme",
			   ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate (bundle);
			UserDialogs.Init(this);
			global::Xamarin.Forms.Forms.Init(this, bundle);
			//global::Xamarin.FormsMaps.Init(this, bundle);
            SvgImageRenderer.Init();
            DependencyResolver.Initialize(new AndroidModule(), new FormsModule());
			LoadApplication(new App());

            CrashManager.Register(this, "635a7d2e041a42fca3421315597b6e5e");

            //DisplayMetrics metrics = new DisplayMetrics();
			//WindowManager.DefaultDisplay.GetMetrics(metrics);
			/*var density = metrics.DensityDpi;
			if (density == DisplayMetricsDensity.High)
			{
				Toast.MakeText(this, "DENSITY_HIGH... Density is " + density, ToastLength.Long).Show();
			}
			else if (density == DisplayMetricsDensity.Medium)
			{
				Toast.MakeText(this, "DENSITY_MEDIUM... Density is " + density, ToastLength.Long).Show();
			}
			else if (density == DisplayMetricsDensity.Low)
			{
				Toast.MakeText(this, "DENSITY_LOW... Density is " + density, ToastLength.Long).Show();
			}
			else if (density == DisplayMetricsDensity.Xhigh)
			{
				Toast.MakeText(this, "DENSITY_XHIGH... Density is " + density, ToastLength.Long).Show();
			}
			else if (density == DisplayMetricsDensity.Xxhigh)
			{
				Toast.MakeText(this, "DENSITY_XXHIGH... Density is " + density, ToastLength.Long).Show();
			}
			else if (density == DisplayMetricsDensity.Xxxhigh)
			{
				Toast.MakeText(this, "DENSITY_XXXHIGH... Density is " + density, ToastLength.Long).Show();
			}
			else {
				Toast.MakeText(this, "Density is neither HIGH, MEDIUM OR LOW.  Density is " + density, ToastLength.Long).Show();
			}*/
		}
	}
}


