using System.Collections.Generic;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Domain;

namespace CallOfService.Technician.Mobile.Proxies.Abstratcs
{
    public interface IAppointmentProxy
    {
        Task<List<Appointment>> GetAppointments(int userId);
        Task<Job> GetJobById(int jobId);
        Task<GpsPoint> GetJobLocation(AddressInfo location);
        Task<bool> StartJob(int jobId);
        Task<bool> FinishJob(int jobId);
    }
}