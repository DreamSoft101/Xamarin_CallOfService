using System;

namespace CallOfService.Mobile.Messages
{
    public enum Source
    {
        Jobs,
        Map
    }
    public class NewDateSelected
    {
        public DateTime DateTime { get; set; }
        public Source Source { get; set; } 

        public NewDateSelected(DateTime dateTime, Source source)
        {
            DateTime = dateTime;
            Source = source;
        }
    }
}