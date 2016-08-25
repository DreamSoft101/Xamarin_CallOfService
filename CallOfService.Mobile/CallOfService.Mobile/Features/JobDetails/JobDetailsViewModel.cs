using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Domain;
using CallOfService.Mobile.Services.Abstracts;
using CallOfService.Mobile.UI;
using PubSub;
using Xamarin.Forms;
using System.Windows.Input;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Messages;
using CallOfService.Mobile.Core;
using Plugin.Connectivity;

namespace CallOfService.Mobile.Features.JobDetails
{
    public class JobDetailsViewModel : ViewModelBase
    {
		private readonly IAppointmentService _appointmentService;
        private readonly IUserDialogs _userDialogs;
        private readonly IAnalyticsService _analyticsService;

        public JobDetailsViewModel(IAppointmentService appointmentService, IUserDialogs userDialogs, IAnalyticsService analyticsService)
        {
            _appointmentService = appointmentService;
            _userDialogs = userDialogs;
            _analyticsService = analyticsService;
            Notes = new ObservableCollection<NoteModel>();
            CustomFields = new ObservableCollection<CustomFieldModel>();
            PhoneNumbers = new ObservableCollection<string>();
            Emails = new ObservableCollection<string>();
            this.Subscribe<ViewJobDetails>(async m => await LoadJobeDetails(m.JobId));
            DataLoaded = false;

            HasPaddingTop = Device.OS == TargetPlatform.iOS;

            IsOnline = CrossConnectivity.Current.IsConnected;
			CrossConnectivity.Current.ConnectivityChanged += HandleConnectivityChangedEventHandler;
        }

        public ICommand NavigateBack
        {
            get
            {
                return new Command(async () =>
                {
                    await NavigationService.NavigateToRootAsync();
                    //await NavigationService.NavigateBackAsync();
                    //await NavigationService.NavigateBackAsync();
                });
            }
        }

        private bool _isOnline;
        public bool IsOnline
        {
            get { return _isOnline; }
            set { SetPropertyValue(ref _isOnline, value); RaisePropertyChanged("IsOffline"); }
        }

		public bool IsOffline => !IsOnline;

        private bool _hasPaddingTop;
        public bool HasPaddingTop
        {
            get { return _hasPaddingTop; }
            set { SetPropertyValue(ref _hasPaddingTop, value); }
        }

        private bool _dataLoaded;
        public bool DataLoaded
        {
            get { return _dataLoaded; }
            set { SetPropertyValue(ref _dataLoaded, value); }
        }

        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set { SetPropertyValue(ref _date, value); }
        }

        private TimeSpan _startTime;
        public TimeSpan StartTime
        {
            get { return _startTime; }
            set { SetPropertyValue(ref _startTime, value); }
        }

        private TimeSpan _endTime;
        public TimeSpan EndTime
        {
            get { return _endTime; }
            set { SetPropertyValue(ref _endTime, value); }
        }

        private string _timeRangeFormat;
        public string TimeRangeFormat
        {
            get { return _timeRangeFormat; }
            set { SetPropertyValue(ref _timeRangeFormat, value); }
        }

        private string _dateTimeFormat;
        public string DateTimeFormat
        {
            get { return _dateTimeFormat; }
            set { SetPropertyValue(ref _dateTimeFormat, value); }
        }

        private string _location;
        public string Location
        {
            get { return _location; }
            set { SetPropertyValue(ref _location, value); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetPropertyValue(ref _title, value); RaisePropertyChanged("HasDescription"); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetPropertyValue(ref _description, value); }
        }

        private string _customerName;
        public string CustomerName
        {
            get { return _customerName; }
            set { SetPropertyValue(ref _customerName, value); }
        }

        private string _contact;
        public string Contact
        {
            get { return _contact; }
            set { SetPropertyValue(ref _contact, value); RaisePropertyChanged("HasContact"); RaisePropertyChanged("HasNoContact"); }
        }

        private bool _showMap;
        public bool ShowMap
        {
            get { return _showMap; }
            set { SetPropertyValue(ref _showMap, value); }
        }

        public bool HasContact => !string.IsNullOrEmpty(Contact);

        public bool HasNoContact => !HasContact;

        public bool HasDescription => !string.IsNullOrEmpty(Description);

        private int _jobNumber;
        public int JobNumber
        {
            get { return _jobNumber; }
            set { SetPropertyValue(ref _jobNumber, value); }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set { SetPropertyValue(ref _status, value); }
        }

        private ObservableCollection<NoteModel> _notes;
        public ObservableCollection<NoteModel> Notes
        {
            get { return _notes; }
            set { SetPropertyValue(ref _notes, value); RaisePropertyChanged("HasNotes");}
        }

        private ObservableCollection<CustomFieldModel> _customFields;
        public ObservableCollection<CustomFieldModel> CustomFields
        {
            get { return _customFields; }
            set { SetPropertyValue(ref _customFields, value); RaisePropertyChanged("HasCustomFields"); }
        }

        public bool HasCustomFields => CustomFields != null && CustomFields.Any();

        public bool HasNotes => Notes != null && Notes.Any();

        private GpsPoint _gpsPoint;
        public GpsPoint GpsPoint
        {
            get { return _gpsPoint; }
            set { SetPropertyValue(ref _gpsPoint, value); }
        }

        private string _pageTitle;
        public string PageTitle
        {
            get { return _pageTitle; }
            set { SetPropertyValue(ref _pageTitle, value); }
        }

        private string _actionText;
        public string ActionText
        {
            get { return _actionText; }
            set { SetPropertyValue(ref _actionText, value); }
        }

        private bool _canStartOrFinish;
        public bool CanStartOrFinish
        {
            get { return _canStartOrFinish; }
            set { SetPropertyValue(ref _canStartOrFinish, value); }
        }

        private ObservableCollection<string> _phoneNumbers;
        public ObservableCollection<string> PhoneNumbers
        {
            get { return _phoneNumbers; }
            set { SetPropertyValue(ref _phoneNumbers, value); }
        }

        private ObservableCollection<string> _emails;
        public ObservableCollection<string> Emails
        {
            get { return _emails; }
            set { SetPropertyValue(ref _emails, value); }
        }

        public ICommand AddNote
        {
            get
            {
                return new Command(async () =>
				{
                    await NavigationService.ShowModalAsync<JobNotePage, JobNoteViewModel>();
                    this.Publish(new ViewJobNoteDetails(JobNumber));
                });
            }
        }

        public ICommand CallCustomerCommand
        {
            get
            {
                return new Command(() =>
                {
                    if (PhoneNumbers != null && PhoneNumbers.Count > 0)
                    {

                        _analyticsService.Track("Calling");

                        var options = new List<ActionSheetOption>();
                        foreach (var phoneNumber in PhoneNumbers)
                        {
                            options.Add(new ActionSheetOption(phoneNumber, () => Device.BeginInvokeOnMainThread(() => Device.OpenUri(new Uri($"tel:{phoneNumber}")))));
                        }
                        _userDialogs.ActionSheet(new ActionSheetConfig
                        {
                            Options = options,
                            Cancel = new ActionSheetOption("Cancel")
                        });
                    }
                });
            }
        }

        public ICommand SmsCustomerCommand
        {
            get
            {
                return new Command(() =>
                {
                    if (PhoneNumbers != null && PhoneNumbers.Count > 0)
                    {
                        _analyticsService.Track("Sending SMS");

                        var options = new List<ActionSheetOption>();
                        foreach (var phoneNumber in PhoneNumbers)
                        {
                            options.Add(new ActionSheetOption(phoneNumber, () => Device.BeginInvokeOnMainThread(() => Device.OpenUri(new Uri($"sms:{phoneNumber}")))));
                        }
                        _userDialogs.ActionSheet(new ActionSheetConfig
                        {
                            Options = options,
                            Cancel = new ActionSheetOption("Cancel")
                        });
                    }
                });
            }
        }

        public ICommand EmailCustomerCommand
        {
            get
            {
                return new Command(() =>
                {
                    if (Emails != null && Emails.Count > 0)
                    {
                        _analyticsService.Track("Emailing");

                        var options = new List<ActionSheetOption>();
                        foreach (var email in Emails)
                        {
                            options.Add(new ActionSheetOption(email, () => Device.BeginInvokeOnMainThread(() => Device.OpenUri(new Uri($"mailto:{email}")))));
                        }
                        _userDialogs.ActionSheet(new ActionSheetConfig()
                        {
                            Options = options,
                            Cancel = new ActionSheetOption("Cancel")
                        });
                    }
                });
            }
        }

        public ICommand DirectionCommand
        {
            get
            {
                return new Command(() =>
                {
                    _analyticsService.Track("Directions");

                    // Windows Phone doesn't like ampersands in the names and the normal URI escaping doesn't help
                    var name = Location.Replace("&", "and"); // var name = Uri.EscapeUriString(place.Name);
                    var loc = $"{GpsPoint.Lat},{GpsPoint.Lng}";
                    var request = Device.OnPlatform(
                        // iOS doesn't like %s or spaces in their URLs, so manually replace spaces with +s
                        $"http://maps.apple.com/maps?q={Location.Replace(' ', '+')}&sll={loc}",

                        // pass the address to Android if we have it
                        $"geo:0,0?q={loc})",

                        // WinPhone
                        $"bingmaps:?cp={loc}&q={name}"
                        );

                    Device.OpenUri(new Uri(request));
                });
            }
        }

        public ICommand StartFinishJob
        {
            get
            {
                return new Command(async () =>
                {
                    if (Status == "Scheduled")
                    {
                        _analyticsService.Track("Starting Job");
                        _userDialogs.ShowLoading("Starting Job");
                        var started = await _appointmentService.StartJob(JobNumber);
                        if (started)
                        {
                            _userDialogs.HideLoading();
                            _userDialogs.ShowSuccess("Job Started");
                        }
                        else
                        {
                            _userDialogs.HideLoading();
                            _userDialogs.ShowError("Error Starting Job");
                        }
                    }
                    else if (Status == "In Progress")
                    {
                        _analyticsService.Track("Finishing Job");
                        _userDialogs.ShowLoading("Finishing Job");
                        var finished = await _appointmentService.FinishJob(JobNumber);
                        if (finished)
                        {
                            _userDialogs.HideLoading();
                            _userDialogs.ShowSuccess("Job Finished");
                            CanStartOrFinish = false;
                        }
                        else
                        {
                            _userDialogs.HideLoading();
                            _userDialogs.ShowError("Error Finishing Job");
                        }
                    }

                    await LoadJobeDetails(JobNumber);
                });
            }
        }

		private void HandleConnectivityChangedEventHandler(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
		{
			IsOnline = e.IsConnected;
		}

        private async Task LoadJobeDetails(int jobId)
        {
            _userDialogs.ShowLoading("Loading Job Details");
            var appointment = await _appointmentService.GetAppointmentByJobId(jobId);
            var job = await _appointmentService.GetJobById(jobId);
            if (job == null)
            {
                _userDialogs.HideLoading();
                _userDialogs.ShowError("Error while loading job details");
                await NavigationService.NavigateBackAsync();
                return;
            }
               
            if (job.Date != null)
                Date = job.Date.Value;
            StartTime = appointment.Start.TimeOfDay;
            EndTime = appointment.End.TimeOfDay;
            TimeRangeFormat = $"{appointment.Start.ToUniversalTime().ToString("hh:mm tt")} - {appointment.End.ToUniversalTime().ToString("hh:mm tt")}";
            DateTimeFormat = $"{Date.ToString("ddd dd MMMM yyyy")} at {TimeRangeFormat}";
            Location = appointment.Location;
            Title = appointment.Title;
            Description = job.Description;
            CustomerName = job.Customer.Name;
            Contact = job.ContactName;
            JobNumber = job.Id;
            Status = job.StatusDescription;
            GpsPoint = job.Point;
            ShowMap = GpsPoint?.IsValid ?? false;

            Notes.Clear();
            foreach (var note in job.Notes)
            {
                var noteViewModel = DependencyResolver.Resolve<NoteModel>();
                noteViewModel.LoadNote(note);
                Notes.Add(noteViewModel);
            }

            CustomFields.Clear();
            foreach (var field in job.CustomFields)
            {
                CustomFields.Add(new CustomFieldModel { Label = field.Key, Value = field.Value });
            }

            var contact = string.IsNullOrEmpty(Contact) ? CustomerName : Contact;

            if (GpsPoint != null && GpsPoint.IsValid)
                this.Publish(new ShowPinOnMap(GpsPoint, Location, contact));

            PageTitle = appointment.Title;
            if (!string.IsNullOrWhiteSpace(appointment.JobType))
                PageTitle = PageTitle + $" [{appointment.JobType}]";

            CanStartOrFinish = true;
            if (Status == "Scheduled")
                ActionText = "Start Job";
            else if (Status == "In Progress")
                ActionText = "Finish Job";
            else
                CanStartOrFinish = false;

            PhoneNumbers.Clear();
            foreach (var phone in job.PhoneNumbers)
            {
                PhoneNumbers.Add(phone.Number);
            }

            Emails.Clear();
            foreach (var email in job.Emails)
            {
                Emails.Add(email.Value);
            }

            DataLoaded = true;
            _userDialogs.HideLoading();
        }

        public override void Dispose()
        {
            Notes?.Clear();
            CustomFields?.Clear();
            DataLoaded = false;

			CrossConnectivity.Current.ConnectivityChanged -= HandleConnectivityChangedEventHandler;
        }

        public override void OnAppearing()
        {
            _analyticsService.Screen("Job Details");

			IsOnline = CrossConnectivity.Current.IsConnected;
        }

        public override void OnDisappearing()
        {
        }
    }

    public class NoteModel: ViewModelBase
    {
        private readonly IAppointmentService _appointmentService;

        public NoteModel(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
            ThumbnilImageSources = new ObservableCollection<ImageSource>();
        }

        private string _description;
        public string Description
        {
            get { return _description;}
            set { SetPropertyValue(ref _description, value); }
        }

        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set { SetPropertyValue(ref _date, value); }
        }

        private string _dateFormat;
        public string DateFormat
        {
            get { return _dateFormat; }
            set { SetPropertyValue(ref _dateFormat, value); }
        }

        public ObservableCollection<ImageSource> ThumbnilImageSources { get; set; }

        public void LoadNote(Note note)
        {
            Description = note.Description;
            Date = note.Timestamp.DateTime;
            DateFormat = $"at {Date.ToString("dd MMMM yyyy")} {Date.ToUniversalTime().ToString("hh:mm tt")}";
            foreach (var fileReference in note.Files)
            {
                ThumbnilImageSources.Add(new UriImageSource
                {
                    Uri = _appointmentService.GetFileUri(fileReference, true)
                });
            }
        }
    }

    public class CustomFieldModel : ViewModelBase
    {
        private string _label;
        public string Label
        {
            get { return _label; }
            set { SetPropertyValue(ref _label, value); }
        }

        private string _value;
        public string Value
        {
            get { return _value; }
            set { SetPropertyValue(ref _value, value); }
        }
    }
}