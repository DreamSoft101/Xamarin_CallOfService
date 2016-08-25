using System;

namespace CallOfService.Mobile.Messages
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