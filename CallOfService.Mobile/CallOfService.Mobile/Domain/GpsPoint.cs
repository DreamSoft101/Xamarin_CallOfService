using Xamarin.Forms.Maps;

namespace CallOfService.Mobile.Domain
{
    public class GpsPoint
    {
        public string Lat { get; set; }
        public string Lng { get; set; }

        public Position Position
        {
            get { return (string.IsNullOrEmpty(Lat) || string.IsNullOrEmpty(Lng)) ? new Position() : new Position(double.Parse(Lat), double.Parse(Lng)); }
        }

        public bool IsValid { get; set; }
    }
}