using System;

namespace CallOfService.Technician.Mobile
{
	public class ViewJobDetails
	{
		public int JobId { get; set; }

		public ViewJobDetails (int jobId)
		{
			JobId = jobId;
		}
	}
}

