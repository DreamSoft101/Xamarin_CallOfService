using System;
using Newtonsoft.Json;
using SQLite.Net.Attributes;
using System.Collections.Generic;

namespace CallOfService.Mobile.Domain
{
    [Table("JobDetails")]
    public class JobDetails
    {
        public JobDetails(Job job)
        {
            Job = job;
            Id = job.Id;
            JobsString = JsonConvert.SerializeObject(job);
        }

        public JobDetails()
        {
        }

        [PrimaryKey]
        public int Id { get; set; }

        [Ignore]
        public Job Job { get; set; }

        public string JobsString { get; set; }
    }

    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public CustomerLookupView Customer { get; set; }
        public AddressInfo Location { get; set; }
        public string ContactName { get; set; }
        public PhoneNumberInfo[] PhoneNumbers { get; set; }
        public EncapsulatedStringInfo[] Emails { get; set; }
        public int? JobType { get; set; }
        public string JobTypeTitle { get; set; }
        public WorkerInfo[] Workers { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public int? NextFollowUpId { get; set; }
        public DateTime? NextFollowUpTime { get; set; }
        public int? EmailReminderToCustomerIntervalInMinutes { get; set; }
        public int? SmsReminderToCustomerIntervalInMinutes { get; set; }
        public int? SourceEstimateId { get; set; }
        public int? SourceRecurringJobId { get; set; }
        public RecurringReminderInfo Reminder { get; set; }
        public JobInfoItem[] FollowUpJobs { get; set; }
        public ServiceItemInfo[] ServiceItems { get; set; }
        public Note[] Notes { get; set; }
        public bool IsDeleted { get; set; }
        public string Status { get; set; }
        public string AutoSchedulePeriod { get; set; }
        public Dictionary<string, string> CustomFields { get; set; }
        public string CancellationReason { get; set; }
        public string StatusDescription { get; set; }
        public GpsPoint Point { get; set; }
    }

    public class WorkerInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
    }

    public class EncapsulatedStringInfo
    {
        public string Value { get; set; }
    }

    public class RecurringReminderInfo
    {
        public int Interval { get; set; }
        public string Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string RecurrencePatternValue { get; set; }
        public DateTime? NextReminder { get; set; }
        public string AutoSchedulePeriod { get; set; }
    }

    public class PhoneNumberInfo
    {
        public string Number { get; set; }
        public TypeInfo[] PhoneNumberTypes { get; set; }
    }

    public class AddressInfo
    {
        public string FormattedAddress { get; set; }
        public TypeInfo[] AddressTypes { get; set; }
    }

    public class TypeInfo
    {
        public string FormattedAddress { get; set; }
        public TypeInfo[] AddressTypes { get; set; }
    }

    public class JobInfoItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public CustomerLookupView Customer { get; set; }
        public string Location { get; set; }
        public string Schedule { get; set; }
        public string JobType { get; set; }
        public string Tags { get; set; }
        public string NextReminder { get; set; }
        public string Status { get; set; }
        public int? SourceEstimateId { get; set; }
        public string ActualTimeDescription { get; set; }
        public string Description { get; set; }
        public string StatusDescription { get; set; }
        public string Contacts { get; set; }
    }

    public class CustomerLookupView
    {
        public string Name { get; set; }
    }

    public class ServiceItemInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal? Price { get; set; }
    }

    public class Note
    {
        public UtcDate Timestamp { get; set; }
        public string Description { get; set; }
        public FileReference[] Files { get; set; }
    }

    public class FileReference
    {
        public string Directory { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }

    }

    public class UtcDate
    {
        public DateTime DateTime { get; set; }
    }
}