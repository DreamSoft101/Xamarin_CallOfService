using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Domain;

namespace CallOfService.Technician.Mobile.Database.Repos.Abstracts
{
    public interface IAppointmentRepo
    {
        Task<int> SaveAppointment(List<Appointment> appointments);
        Task<List<Appointment>> AppointmentsByDay(DateTime date);
    }
}