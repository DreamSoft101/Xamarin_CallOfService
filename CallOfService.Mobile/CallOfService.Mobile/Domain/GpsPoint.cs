using Xamarin.Forms.Maps;

namespace CallOfService.Mobile.Domain
{
    public class GpsPoint
    {
        public string Lat { get; set; }
        public string Lng { get; set; }

        public Position Position
        {
            get
            {
                if (string.IsNullOrEmpty(Lat) || string.IsNullOrEmpty(Lng))
                    return new Position();

                double lat;
                double lng;
                if (double.TryParse(Lat, out lat) && double.TryParse(Lng, out lng))
                    return new Position(lat, lng);

                return new Position();
            }
        }

        public bool IsValid { get; set; }
    }
}