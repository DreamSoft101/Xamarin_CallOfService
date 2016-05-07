using Android.App;
using Android.Widget;
using Android.OS;
using XLabs.Forms;
using CallOfService.Technician.Mobile.Core.DI;
using CallOfService.Technician.Mobile.Droid.Core.DI;

namespace CallOfService.Technician.Mobile.Droid
{
	[Activity (Label = "CallOfService.Technician.Mobile.Droid", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : XFormsApplicationDroid
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			Xamarin.Forms.Forms.Init(this, savedInstanceState);
			Xamarin.FormsMaps.Init(this,savedInstanceState);
			DependencyResolver.Initialize(new AndroidModule(), new FormsModule());
			LoadApplication(new App());
		}
	}
}


