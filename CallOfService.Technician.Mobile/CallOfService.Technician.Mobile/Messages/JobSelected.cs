using System;
using CallOfService.Technician.Mobile.Features.Jobs;

namespace CallOfService.Technician.Mobile
{
	public class JobSelected
	{

		public AppointmentViewModel Appointment {
			get;
			set;
		}

		public JobSelected (AppointmentViewModel appointmentViewModel)
		{
			Appointment = appointmentViewModel;
		}
	}
}

