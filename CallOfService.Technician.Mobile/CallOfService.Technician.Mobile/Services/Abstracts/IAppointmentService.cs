using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Domain;

namespace CallOfService.Technician.Mobile.Services.Abstracts
{
    public interface IAppointmentService
    {
        Task<bool> RetrieveAndSaveAppointments();
        Task<List<Appointment>> AppointmentsByDay(DateTime date);
        Task<Appointment> GetAppointmentByJobId(int jobId);
        Task<Job> GetJobById(int jobId);
        Uri GetFileUri(FileReference fileReference, bool isThumbnil);
    }
}