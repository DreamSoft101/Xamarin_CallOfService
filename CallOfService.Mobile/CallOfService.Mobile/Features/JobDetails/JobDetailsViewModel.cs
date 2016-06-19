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
using PropertyChanged;
using CallOfService.Mobile.Core;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Connectivity;
using Segment.Model;

namespace CallOfService.Mobile.Features.JobDetails
{
    [ImplementPropertyChanged]
    public class JobDetailsViewModel : IViewAwareViewModel
    {
		private readonly IAppointmentService _appointmentService;
        private readonly IUserDialogs _userDialogs;
        private readonly IAnalyticsService _analyticsService;
        private readonly IImageCompressor _imageCompressor;
        private readonly ILogger _logger;

        public JobDetailsViewModel(IAppointmentService appointmentService, IUserDialogs userDialogs, IImageCompressor imageCompressor, IAnalyticsService analyticsService, ILogger logger)
        {
            _appointmentService = appointmentService;
            _userDialogs = userDialogs;
            _imageCompressor = imageCompressor;
            _analyticsService = analyticsService;
            _logger = logger;
            Notes = new ObservableCollection<NoteViewModel>();
            CustomFields = new ObservableCollection<CustomFieldViewModel>();
            this.Subscribe<ViewJobDetails>(async m => await LoadJobeDetails(m.JobId));
            DataLoaded = false;

			IsOnline = CrossConnectivity.Current.IsConnected;
			CrossConnectivity.Current.ConnectivityChanged += HandleConnectivityChangedEventHandler;
        }

        public ICommand NavigateBack
        {
            get
            {
                return new Command(async () =>
                {
                    await NavigationService.NavigateBack();
                });
            }
        }

		public bool IsOnline { get; set; } = true;
		public bool IsOffline { get { return !IsOnline; } }

        public bool DataLoaded { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string TimeRangeFormat { get; set; }

        public string DateTimeFormat { get; set; }

        public string Location { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string CustomerName { get; set; }

        public string Contact { get; set; }

        public bool ShowMap { get; set; }

        public bool HasContact
        {
            get { return !string.IsNullOrEmpty(Contact); }
        }

        public bool HasNoContact
        {
            get { return !HasContact; }
        }

        public bool HasDescription
        {
            get { return !string.IsNullOrEmpty(Description); }
        }

        public int JobNumber { get; set; }

        public string Status { get; set; }

        public string Strata { get; set; }

        public ObservableCollection<NoteViewModel> Notes { get; set; }

        public ObservableCollection<CustomFieldViewModel> CustomFields { get; set; }

        public bool HasCustomFields
        {
            get { return CustomFields != null && CustomFields.Any(); }
        }

        public bool HasNotes
        {
            get { return Notes != null && Notes.Any(); }
        }

        public string Custom { get; set; }

        public GpsPoint GpsPoint { get; set; }

        public string PageTitle { get; set; }

        public string ActionText { get; set; }

        public bool CanStartOrFinish { get; set; }

        public List<string> PhoneNumbers { get; set; }

        public List<string> Emails { get; set; }

        public ICommand AddNote
        {
            get
            {
                return new Command(async () =>
				{
                    await NavigationService.ShowModal<JobNotePage, JobNoteViewModel>();
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

#pragma warning disable 4014
                        _analyticsService.Track("Calling");
#pragma warning restore 4014

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

#pragma warning disable 4014
                        _analyticsService.Track("Sending SMS");
#pragma warning restore 4014

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

#pragma warning disable 4014
                        _analyticsService.Track("Emailing");
#pragma warning restore 4014

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

#pragma warning disable 4014
                    _analyticsService.Track("Directions");
#pragma warning restore 4014

                    // Windows Phone doesn't like ampersands in the names and the normal URI escaping doesn't help
                    var name = Location.Replace("&", "and"); // var name = Uri.EscapeUriString(place.Name);
                    var loc = string.Format("{0},{1}", GpsPoint.Lat, GpsPoint.Lng);
                    var request = Device.OnPlatform(
                                      // iOS doesn't like %s or spaces in their URLs, so manually replace spaces with +s
                                      string.Format("http://maps.apple.com/maps?q={0}&sll={1}", Location.Replace(' ', '+'), loc),

                                      // pass the address to Android if we have it
                                      string.Format("geo:0,0?q={0})", loc),

                                      // WinPhone
                                      string.Format("bingmaps:?cp={0}&q={1}", loc, name)
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
#pragma warning disable 4014
                        _analyticsService.Track("Starting Job");
#pragma warning restore 4014

                        _userDialogs.ShowLoading("Starting Job");
                        var started = await _appointmentService.StartJob(JobNumber);
                        if (started)
                        {
                            _userDialogs.ShowSuccess("Job Started");
                            await Task.Delay(100);
                            _userDialogs.HideLoading();
                        }
                        else
                        {
                            _userDialogs.HideLoading();
                            _userDialogs.ShowError("Error Starting Job");
                            await Task.Delay(3000);
                        }
                    }
                    else if (Status == "In Progress")
                    {
#pragma warning disable 4014
                        _analyticsService.Track("Finishing Job");
#pragma warning restore 4014
                        _userDialogs.ShowLoading("Finishing Job");
                        var finished = await _appointmentService.FinishJob(JobNumber);
                        if (finished)
                        {
                            _userDialogs.ShowSuccess("Job Finished");
                            await Task.Delay(100);
                            _userDialogs.HideLoading();
                            CanStartOrFinish = false;
                        }
                        else
                        {
                            _userDialogs.HideLoading();
                            _userDialogs.ShowError("Error Finishing Job");
                            await Task.Delay(3000);
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
                await Task.Delay(3000);
                await NavigationService.NavigateBack();
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
                var noteViewModel = DependencyResolver.Resolve<NoteViewModel>();
                noteViewModel.LoadNote(note);
                Notes.Add(noteViewModel);
            }

            CustomFields.Clear();
            foreach (var field in job.CustomFields)
            {
                CustomFields.Add(new CustomFieldViewModel { Label = field.Key, Value = field.Value });
            }

            if (GpsPoint != null && GpsPoint.IsValid)
                this.Publish(new ShowPinOnMap(GpsPoint, Location, Contact));

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

            PhoneNumbers = job.PhoneNumbers.Select(p => p.Number).ToList();
            Emails = job.Emails.Select(e => e.Value).ToList();
            DataLoaded = true;
            _userDialogs.HideLoading();

        }

        public void Dispose()
        {
            Notes.Clear();
            CustomFields.Clear();
            DataLoaded = false;

			CrossConnectivity.Current.ConnectivityChanged -= HandleConnectivityChangedEventHandler;
        }

        public async void OnAppearing()
        {
#pragma warning disable 4014
            _analyticsService.Screen("Job Details");
#pragma warning restore 4014

            if(JobNumber > 0)
               await LoadJobeDetails(JobNumber);

			IsOnline = CrossConnectivity.Current.IsConnected;
        }

        public void OnDisappearing()
        {
        }
    }

    [ImplementPropertyChanged]
    public class NoteViewModel
    {
        private readonly IAppointmentService _appointmentService;

        public NoteViewModel(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
            ThumbnilImageSources = new ObservableCollection<ImageSource>();
        }

        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string DateFormat { get; set; }

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

    [ImplementPropertyChanged]
    public class CustomFieldViewModel
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }
}