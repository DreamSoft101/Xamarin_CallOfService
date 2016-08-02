using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CallOfService.Mobile.Database.Repos.Abstracts;
using CallOfService.Mobile.Domain;
using System.Linq;
using Newtonsoft.Json;

namespace CallOfService.Mobile.Database.Repos
{
    public class AppointmentRepo : IAppointmentRepo
    {
        private readonly IDbSet<Appointment> _appointmentDbset;
        private readonly IDbSet<JobDetails> _jobDetailsDbSet;

        public AppointmentRepo(IDbSet<Appointment> appointmentDbset, IDbSet<JobDetails> jobDetailsDbSet)
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
            var dateInt = int.Parse(date.ToString("yyyyMMdd"));
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

        public async Task<Job> GetJobById(int jobId)
        {
            var jobs = await _jobDetailsDbSet.Get(j => j.Id == jobId);
            var job = jobs.SingleOrDefault();
            if (job == null)
            {
                return null;
            }

            job.Job = JsonConvert.DeserializeObject<Job>(job.JobsString);
            return job.Job;
        }

        public async Task<int> SaveJob(Job job)
        {
            var existingJob = await _jobDetailsDbSet.Get(j => j.Id == job.Id);
            if (existingJob.SingleOrDefault() != null)
            {
                var jobDetails = new JobDetails(job);
                return await _jobDetailsDbSet.Update(jobDetails);
            }
            else
            {
                var jobDetails = new JobDetails(job);
                return await _jobDetailsDbSet.Add(jobDetails);
            }
        }
    }
}