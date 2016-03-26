using System.Collections.Generic;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Domain;
using CallOfService.Technician.Mobile.Proxies.Abstratcs;
using CallOfService.Technician.Mobile.Services.Abstracts;

namespace CallOfService.Technician.Mobile.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentProxy _appointmentProxy;

        public AppointmentService(IAppointmentProxy appointmentProxy)
        {
            _appointmentProxy = appointmentProxy;
        }

        public Task<List<Appointment>> GetAppointments()
        {
            return _appointmentProxy.GetAppointments();
        }
    }
}