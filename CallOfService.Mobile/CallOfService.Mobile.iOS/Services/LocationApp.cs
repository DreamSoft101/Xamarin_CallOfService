using System;
using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Services.Abstracts;
using CoreLocation;
using Foundation;
using UIKit;

namespace CallOfService.Mobile.iOS.Services
{
    public class LocationApp
    {
        protected CLLocationManager locMgr;

        public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };

        public LocationApp()
        {
            this.locMgr = new CLLocationManager {PausesLocationUpdatesAutomatically = false};

            // iOS 8 has additional permissions requirements
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                locMgr.RequestAlwaysAuthorization(); // works in background
                                                     //locMgr.RequestWhenInUseAuthorization (); // only in foreground
            }

            // iOS 9 requires the following for background location updates
            // By default this is set to false and will not allow background updates
            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                locMgr.AllowsBackgroundLocationUpdates = true;
            }

            LocationUpdated += SendLocation;

        }

        public CLLocationManager LocMgr
        {
            get { return this.locMgr; }
        }

        public void StartLocationUpdates()
        {

            // We need the user's permission for our app to use the GPS in iOS. This is done either by the user accepting
            // the popover when the app is first launched, or by changing the permissions for the app in Settings
            if (CLLocationManager.LocationServicesEnabled)
            {
                //set the desired accuracy, in meters
                LocMgr.DesiredAccuracy = 50;

                if (UIDevice.CurrentDevice.CheckSystemVersion(6, 0))
                {

                    LocMgr.LocationsUpdated += (sender, e) => {
                        // fire our custom Location Updated event
                        this.LocationUpdated(this, new LocationUpdatedEventArgs(e.Locations[e.Locations.Length - 1]));
                    };

                }
                else
                {

                    // this won't be called on iOS 6 (deprecated). We will get a warning here when we build.
                    LocMgr.UpdatedLocation += (sender, e) => {
                        this.LocationUpdated(this, new LocationUpdatedEventArgs(e.NewLocation));
                    };
                }

                // Start our location updates
                LocMgr.StartUpdatingLocation();

                // Get some output from our manager in case of failure
                LocMgr.Failed += (sender, e) => {
                    Console.WriteLine(e.Error);
                };

                //LocMgr.LocationsUpdated += (sender, e) => {
                //    // fire our custom Location Updated event
                //    LocationUpdated(this, new LocationUpdatedEventArgs(e.Locations[e.Locations.Length - 1]));
                //};

                LocMgr.StartUpdatingLocation();
            }
        }

        //This will keep going in the background and the foreground
        public void SendLocation(object sender, LocationUpdatedEventArgs e)
        {
            try
            {
                var location = e.Location;
                var locationService = DependencyResolver.Resolve<ILocationService>();
                locationService.SendLocatoinIfUpdated(location.Coordinate.Latitude, location.Coordinate.Longitude);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    public class LocationUpdatedEventArgs : EventArgs
    {
        readonly CLLocation location;

        public LocationUpdatedEventArgs(CLLocation location)
        {
            this.location = location;
        }

        public CLLocation Location
        {
            get { return location; }
        }
    }
}