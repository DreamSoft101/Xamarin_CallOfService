using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.iOS.Core.DI;
using CallOfService.Mobile.iOS.Services;
using Foundation;
using HockeyApp.iOS;
using TwinTechs.iOS;
using UIKit;

namespace CallOfService.Mobile.iOS
{   

    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public static LocationApp LocationManager { get; set; }

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            var manager = BITHockeyManager.SharedHockeyManager;
            manager.Configure("95bc2d92ecb04f179edf35df5e942eca");
            manager.StartManager();
            global::Xamarin.Forms.Forms.Init();
            Xamarin.FormsMaps.Init();
            DependencyResolver.Initialize(new IosModule(), new FormsModule());
            LoadApplication(new App());
            SvgImageRenderer.Init();

            try
            {
                LocationManager = new LocationApp();
                LocationManager.StartLocationUpdates();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return base.FinishedLaunching(app, options);
        }
    }
}
