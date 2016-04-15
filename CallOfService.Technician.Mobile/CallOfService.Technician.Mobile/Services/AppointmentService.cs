using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Core;
using CallOfService.Technician.Mobile.Core.Networking;
using CallOfService.Technician.Mobile.Core.Security;
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

        public AppointmentService(IAppointmentProxy appointmentProxy, IUserService userService,
            IAppointmentRepo appointmentRepo)
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
				appointments.ForEach(a => a.UpdateDates());
                await _appointmentRepo.DeleteAll();
                await _appointmentRepo.SaveAppointment(appointments);
                return true;
            }
            return false;
        }

        public Task<List<Appointment>> AppointmentsByDay(DateTime date)
        {
            //TODO Needs to get the appointments  from the server
            return _appointmentRepo.AppointmentsByDay(date);
        }

        public Task<Appointment> GetAppointmentByJobId(int jobId)
        {
            return _appointmentRepo.GetAppointmentByJobId(jobId);
        }

        public async Task<Job> GetJobById(int jobId)
        {
            var job = await _appointmentProxy.GetJobById(jobId);
            await _appointmentRepo.SaveJob(job);
            return job;
        }

        public Uri GetFileUri(FileReference fileReference, bool isThumbnil)
        {
            var userCredentials = _userService.GetUserCredentials();

            var url =
                $"{UrlConstants.BaseUrl}{UrlConstants.FileUrl}directory={fileReference.Directory}&fileName={fileReference.FileName}";

            if (isThumbnil) url = $"{url}&thumbnail=true";

            url = $"{url}&t={userCredentials.Token}";

            return new Uri(url);
        }
    }
}