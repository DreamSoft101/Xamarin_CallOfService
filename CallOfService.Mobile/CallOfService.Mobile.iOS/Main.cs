using System;
using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Core.SystemServices;
using Elmah.Io.Client;
using UIKit;

namespace CallOfService.Mobile.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            try
            {
                UIApplication.Main(args, null, "AppDelegate");
            }
            catch (Exception e)
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
}
