using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CallOfService.Mobile.Domain;

namespace CallOfService.Mobile.Proxies.Abstratcs
{
    public interface IAppointmentProxy
    {
        Task<List<Appointment>> GetAppointments(int userId, DateTime? date = null, string view = "year");
        Task<Job> GetJobById(int jobId);
        Task<GpsPoint> GetJobLocation(AddressInfo location);
        Task<bool> StartJob(int jobId, double? latitude, double? longitude);
        Task<bool> FinishJob(int jobId, double? latitude, double? longitude);
		Task<bool> AddNote(int jobNumber, string newNoteText, List<byte[]> attachments, DateTime now, double? latitude, double? longitude);
    }
}