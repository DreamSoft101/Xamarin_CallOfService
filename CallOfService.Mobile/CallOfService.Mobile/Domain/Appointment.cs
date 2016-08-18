using System;
using SQLite.Net.Attributes;

namespace CallOfService.Mobile.Domain
{
    [Table("Appointment")]
    public class Appointment
    {
        [PrimaryKey]
        public int JobId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public string JobType { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int StartDate { get; set; }
        public int EndDate { get; set; }
        [Ignore]
        public int[] Workers { get; set; }
        public string WorkersText { get; set; }
        public bool CanEdit { get; set; }
        public bool IsFinished { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsInProgress { get; set; }
        public string StartString { get; set; }
        public string EndString { get; set; }
        public double? LocationLatitude { get; set; }
        public double? LocationLongitude { get; set; }

        public void UpdateDates()
        {
            StartDate = int.Parse(Start.ToString("yyyyMMdd"));
            EndDate = int.Parse(End.ToString("yyyyMMdd"));
        }
    }
}