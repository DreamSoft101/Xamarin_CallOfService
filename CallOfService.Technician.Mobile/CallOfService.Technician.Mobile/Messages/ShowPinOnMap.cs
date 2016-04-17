using System;
using CallOfService.Technician.Mobile.Domain;

namespace CallOfService.Technician.Mobile.Messages
{
    internal class ShowPinOnMap
    {
        public string Location { get; set; }
        public GpsPoint GpsPoint { get; set; }
        public string Contact { get; set; }

        public ShowPinOnMap(GpsPoint gpsPoint,string location,string contact)
        {
            Location = location;
            GpsPoint = gpsPoint;
            Contact = contact;
        }
    }
}