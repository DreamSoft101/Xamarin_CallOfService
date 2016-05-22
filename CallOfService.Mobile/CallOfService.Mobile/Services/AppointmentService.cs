using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CallOfService.Mobile.Core;
using CallOfService.Mobile.Core.Networking;
using CallOfService.Mobile.Database.Repos.Abstracts;
using CallOfService.Mobile.Domain;
using CallOfService.Mobile.Proxies.Abstratcs;
using CallOfService.Mobile.Services.Abstracts;

namespace CallOfService.Mobile.Services
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
			if (job == null) {
				return await _appointmentRepo.GetJobById(jobId);
			}
            var point = await _appointmentProxy.GetJobLocation(job.Location);
            job.Point = point;
            await _appointmentRepo.SaveJob(job);
            return job;
        }

        public Uri GetFileUri(FileReference fileReference, bool isThumbnil)
        {
            var userCredentials = _userService.GetUserCredentials();

            var url = $"{UrlConstants.BaseUrl}{UrlConstants.FileUrl}?directory={fileReference.Directory}&fileName={fileReference.FileName}";

            if (isThumbnil) url = $"{url}&thumbnail=true";

            url = $"{url}&t={userCredentials.Token}";

            return new Uri(url);
        }

        public Task<bool> StartJob(int jobId)
        {
            return _appointmentProxy.StartJob(jobId);
        }

        public Task<bool> FinishJob(int jobId)
        {
            return _appointmentProxy.FinishJob(jobId);
        }

		public Task<bool> SubmitNote(int jobNumber, string newNoteText, List<byte[]> attachments, DateTime now)
        {
            return _appointmentProxy.AddNote(jobNumber, newNoteText, attachments, now);
        }
    }
}