using System.Collections.Generic;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Domain;

namespace CallOfService.Technician.Mobile.Services.Abstracts
{
    public interface IAppointmentService
    {
        Task<List<Appointment>> GetAppointments();
    }
}