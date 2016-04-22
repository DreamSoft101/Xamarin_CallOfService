using System;

namespace CallOfService.Technician.Mobile.Messages
{
    public class NewDateSelected
    {
        public DateTime DateTime { get; set; }

        public NewDateSelected(DateTime dateTime)
        {
            DateTime = dateTime;
        }
    }
}