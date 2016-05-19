using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CallOfService.Mobile.Domain;

namespace CallOfService.Mobile.Database.Repos.Abstracts
{
    public interface IAppointmentRepo
    {
        Task<int> SaveAppointment(List<Appointment> appointments);
        Task<List<Appointment>> AppointmentsByDay(DateTime date);
        Task DeleteAll();
        Task<Appointment> GetAppointmentByJobId(int jobId);
        Task<int> SaveJob(Job job);
		Task<Job> GetJobById (int jobId);
    }
}