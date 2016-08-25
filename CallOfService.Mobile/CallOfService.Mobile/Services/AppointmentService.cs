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
        private static readonly AsyncLock Mutex = new AsyncLock();

        private readonly IAppointmentProxy _appointmentProxy;
        private readonly IUserService _userService;
        private readonly IAppointmentRepo _appointmentRepo;
        private readonly ILocationService _locationService;
        private DateTime? _lastUpdated;

        public AppointmentService(IAppointmentProxy appointmentProxy, IUserService userService, IAppointmentRepo appointmentRepo, ILocationService locationService)
        {
            _appointmentProxy = appointmentProxy;
            _userService = userService;
            _appointmentRepo = appointmentRepo;
            _locationService = locationService;
        }

        public async Task<bool> RetrieveAndSaveAppointments(DateTime? date = null)
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                var currentUser = await _userService.GetCurrentUser();
                var appointments = await _appointmentProxy.GetAppointments(currentUser.UserId, date);
                if (appointments != null)
                {
                    appointments.ForEach(a => a.UpdateDates());
                    //ToDo: Diff and check last updated instead of delete all and re-insert
                    await _appointmentRepo.DeleteAll();
                    await _appointmentRepo.SaveAppointment(appointments);

                    _lastUpdated = DateTime.UtcNow;
                    return true;
                }
                return false;
            }
        }

        public async Task<List<Appointment>> AppointmentsByDay(DateTime date, bool forceRefresh = false)
        {
            if (forceRefresh)
                await RetrieveAndSaveAppointments(date);
            else 
            {
                if (await NeedsRefresh())
                    await RetrieveAndSaveAppointments(date);
            }

            return await _appointmentRepo.AppointmentsByDay(date);
        }

        public Task<Appointment> GetAppointmentByJobId(int jobId)
        {
            return _appointmentRepo.GetAppointmentByJobId(jobId);
        }

        public async Task<Job> GetJobById(int jobId)
        {
            var job = await _appointmentProxy.GetJobById(jobId);
            if (job == null)
            {
                return await _appointmentRepo.GetJobById(jobId);
            }
            GpsPoint point;
            if (job.Location.Latitude == null || job.Location.Longitude == null)
            {
                point = await _appointmentProxy.GetJobLocation(job.Location);
            }
            else
            {
                point = new GpsPoint { Lng = job.Location.Longitude.Value.ToString(), Lat = job.Location.Latitude.Value.ToString(), IsValid = true };
            }
            job.Point = point;
            await _appointmentRepo.SaveJob(job);
            return job;
        }

        public Uri GetFileUri(FileReference fileReference, bool isThumbnil)
        {
            var userCredentials = _userService.GetUserCredentials();

            var url = $"{Helpers.Settings.ServerUrl}/{UrlConstants.FileUrl}?directory={fileReference.Directory}&fileName={fileReference.FileName}";

            if (isThumbnil) url = $"{url}&thumbnail=true";

            url = $"{url}&t={userCredentials.Token}";

            return new Uri(url);
        }

        public async Task<bool> StartJob(int jobId)
        {
            var position = await _locationService.GetCurrentLocation(timeoutInMilliseconds: 5000);
            return await _appointmentProxy.StartJob(jobId, position?.Latitude, position?.Longitude);
        }

        public async Task<bool> FinishJob(int jobId)
        {
            var position = await _locationService.GetCurrentLocation(timeoutInMilliseconds: 5000);
            return await _appointmentProxy.FinishJob(jobId, position?.Latitude, position?.Longitude);
        }

        public async Task<bool> SubmitNote(int jobNumber, string newNoteText, List<byte[]> attachments, DateTime now)
        {
            var position = await _locationService.GetCurrentLocation(timeoutInMilliseconds: 5000);
            return await _appointmentProxy.AddNote(jobNumber, newNoteText, attachments, now, position?.Latitude, position?.Longitude);
        }

        private async Task<bool> NeedsRefresh()
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                return _lastUpdated == null || (DateTime.UtcNow - _lastUpdated.Value).TotalMinutes > 1;
            }
        }
    }
}