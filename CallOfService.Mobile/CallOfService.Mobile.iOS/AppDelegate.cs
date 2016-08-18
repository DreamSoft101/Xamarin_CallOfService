using System;
using System.Net;
using System.Threading.Tasks;
using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.iOS.Core.DI;
using CallOfService.Mobile.iOS.Services;
using Elmah.Io.Client;
using Foundation;
using HockeyApp.iOS;
using TK.CustomMap.iOSUnified;
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
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            var manager = BITHockeyManager.SharedHockeyManager;
            manager.Configure("95bc2d92ecb04f179edf35df5e942eca");
            manager.StartManager();
            global::Xamarin.Forms.Forms.Init();
            Xamarin.FormsMaps.Init();
            TKCustomMapRenderer.InitMapRenderer();
            NativePlacesApi.Init();
            DependencyResolver.Initialize(new IosModule(), new FormsModule());
            LoadApplication(new App());
            SvgImageRenderer.Init();

            //try
            //{
            //    LocationManager = new LocationApp();
            //    LocationManager.StartLocationUpdates();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}

            return base.FinishedLaunching(app, options);
        }

        protected override void Dispose(bool disposing)
        {
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;

            base.Dispose(disposing);
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", e.Exception);
            LogUnhandledException(newExc);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", e.ExceptionObject as Exception);
            LogUnhandledException(newExc);
        }

        internal static void LogUnhandledException(Exception e)
        {
            try
            {
                var logger = DependencyResolver.Resolve<ILogger>();
                logger.WriteError(e.Message, e.ToString(), e, e.ToDataList());
            }
            catch
            {
            }
        }
    }
}
