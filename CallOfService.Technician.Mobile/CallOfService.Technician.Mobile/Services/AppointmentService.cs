using System.Collections.Generic;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Database.Repos;
using CallOfService.Technician.Mobile.Database.Repos.Abstracts;
using CallOfService.Technician.Mobile.Domain;
using CallOfService.Technician.Mobile.Proxies.Abstratcs;
using CallOfService.Technician.Mobile.Services.Abstracts;

namespace CallOfService.Technician.Mobile.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentProxy _appointmentProxy;
        private readonly IUserService _userService;
        private readonly IAppointmentRepo _appointmentRepo;

        public AppointmentService(IAppointmentProxy appointmentProxy,IUserService userService, IAppointmentRepo appointmentRepo)
        {
            _appointmentProxy = appointmentProxy;
            _userService = userService;
            _appointmentRepo = appointmentRepo;
        }

        public async Task<bool> RetrieveAndSaveAppointments()
        {
            var currentUser = await _userService.GetCurrentUser();
            var appointments = await _appointmentProxy.GetAppointments(currentUser.UserId);
            if (appointments != null)
            {
                await _appointmentRepo.SaveAppointment(appointments);
                return true;
            }
            return false;
        }
    }
}