using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Domain;
using Xamarin.Forms;

namespace CallOfService.Technician.Mobile.Proxies.Abstratcs
{
    public interface IAppointmentProxy
    {
        Task<List<Appointment>> GetAppointments(int userId);
        Task<Job> GetJobById(int jobId);
    }
}