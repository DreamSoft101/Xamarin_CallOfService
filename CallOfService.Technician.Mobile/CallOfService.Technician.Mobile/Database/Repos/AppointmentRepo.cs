using System.Collections.Generic;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Database.Repos.Abstracts;
using CallOfService.Technician.Mobile.Domain;

namespace CallOfService.Technician.Mobile.Database.Repos
{
    public class AppointmentRepo : IAppointmentRepo
    {
        readonly IDbSet<Appointment> _appointmentDbset;

        public AppointmentRepo(IDbSet<Appointment> appointmentDbset)
        {
            _appointmentDbset = appointmentDbset;
        }

        public Task<int> SaveAppointment(List<Appointment> appointments)
        {
            return _appointmentDbset.Add(appointments);
        }
    }
}