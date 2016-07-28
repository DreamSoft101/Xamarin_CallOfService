using System.Threading.Tasks;

namespace CallOfService.Mobile.Proxies.Abstratcs
{
    public interface ILocationProxy
    {
        Task<bool> SendLocation(double latitude, double longitude);
    }
}