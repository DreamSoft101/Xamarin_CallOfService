using System;
using CallOfService.Mobile.Features.Jobs;

namespace CallOfService.Mobile
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

