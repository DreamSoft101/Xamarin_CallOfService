using System;

namespace CallOfService.Mobile
{
	public class ViewJobDetails
	{
		public int JobId { get; set; }

		public ViewJobDetails (int jobId)
		{
			JobId = jobId;
		}
	}

    public class ViewJobNoteDetails
    {
        public int JobId { get; set; }

        public ViewJobNoteDetails(int jobId)
        {
            JobId = jobId;
        }
    }
}

