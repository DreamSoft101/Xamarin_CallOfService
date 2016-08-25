using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CallOfService.Mobile.Domain;

namespace CallOfService.Mobile.Services.Abstracts
{
    public interface IAppointmentService
    {
        Task<bool> RetrieveAndSaveAppointments(DateTime? date = null);
        Task<List<Appointment>> AppointmentsByDay(DateTime date, bool forceRefresh = false);
        Task<Appointment> GetAppointmentByJobId(int jobId);
        Task<Job> GetJobById(int jobId);
        Uri GetFileUri(FileReference fileReference, bool isThumbnil);
		Task<bool> StartJob(int jobId);
		Task<bool> FinishJob(int jobId);
		Task<bool> SubmitNote(int jobNumber, string newNoteText, List<byte[]> attachments, DateTime now);
    }
}