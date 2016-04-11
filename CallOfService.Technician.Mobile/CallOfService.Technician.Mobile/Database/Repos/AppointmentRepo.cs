using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Database.Repos.Abstracts;
using CallOfService.Technician.Mobile.Domain;

namespace CallOfService.Technician.Mobile.Database.Repos
{
    public class AppointmentRepo : IAppointmentRepo
    {
        readonly IDbSet<Appointment> _appointmentDbset;
        private readonly IDbSet<JobDetails> _jobDetailsDbSet;

        public AppointmentRepo(IDbSet<Appointment> appointmentDbset,IDbSet<JobDetails> jobDetailsDbSet)
        {
            _appointmentDbset = appointmentDbset;
            _jobDetailsDbSet = jobDetailsDbSet;
        }

        public Task<int> SaveAppointment(List<Appointment> appointments)
        {
            return _appointmentDbset.Add(appointments);
        }

        public Task<List<Appointment>> AppointmentsByDay(DateTime date)
        {
            var dateInt = Int32.Parse(date.ToString("yyyyMMdd"));
            return _appointmentDbset.Get(a => a.StartDate <= dateInt && a.EndDate >= dateInt);
        }

        public Task DeleteAll()
        {
            return _appointmentDbset.DeleteAll();
        }

        public Task<Appointment> GetAppointmentByJobId(int jobId)
        {
            return _appointmentDbset.GetById(jobId);
        }

        public Task<int> SaveJob(Job job)
        {
            var jobDetails = new JobDetails(job);
            return _jobDetailsDbSet.Add(jobDetails);
        }
    }
}