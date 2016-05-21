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

namespace CallOfService.Mobile.Features.JobDetails
{
    [ImplementPropertyChanged]
    public class JobDetailsViewModel : IViewAwareViewModel
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IUserDialogs _userDialogs;
        private ImageSource _imageSource;
        private readonly IAnalyticsService _analyticsService;
        private readonly IMediaPicker _mediaPicker;
        private readonly IImageCompressor _imageCompressor;
        private readonly ILogger _logger;

        public JobDetailsViewModel(IAppointmentService appointmentService, IUserDialogs userDialogs, IMediaPicker mediaPicker, IImageCompressor imageCompressor, IAnalyticsService analyticsService, ILogger logger)
        {
            _appointmentService = appointmentService;
            _userDialogs = userDialogs;
            _mediaPicker = mediaPicker;
            _imageCompressor = imageCompressor;
            _analyticsService = analyticsService;
            _logger = logger;
            Notes = new ObservableCollection<NoteViewModel>();
            Attachments = new ObservableCollection<ImageSource>();
            AttachmentsStreams = new List<byte[]>();
            CustomFields = new ObservableCollection<CustomFieldViewModel>();
            this.Subscribe<ViewJobDetails>(async m => await LoadJobeDetails(m.JobId));
            DataLoaded = false;
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

        public bool ShowActionButton { get; set; }

        public List<string> PhoneNumbers { get; set; }

        public List<string> Emails { get; set; }

        public string NewNoteText { get; set; }

        public ICommand AddNote
        {
            get
            {
                return new Command(async () =>
                {
#pragma warning disable 4014
                    _analyticsService.Track("Adding note");
#pragma warning restore 4014

                    _userDialogs.ShowLoading("Adding note");
                    var noteSaved = await _appointmentService.SubmitNote(JobNumber, NewNoteText, AttachmentsStreams, DateTime.Now);
                    if (noteSaved)
                    {
                        _userDialogs.HideLoading();
                        await LoadJobeDetails(JobNumber);
                        Attachments.Clear();
                        AttachmentsStreams.Clear();
                        NewNoteText = string.Empty;
                    }
                    else
                    {
                        _userDialogs.HideLoading();
                        _userDialogs.ShowError("Error saving note please try again");
                        await Task.Delay(3000);
                    }
                });
            }
        }

        public ICommand CallCustomerCommand
        {
            get
            {
                return new Command(() =>
                {
                    if (PhoneNumbers != null || PhoneNumbers.Count > 0)
                    {

#pragma warning disable 4014
                        _analyticsService.Track("Calling");
#pragma warning restore 4014

                        var options = new List<ActionSheetOption>();
                        foreach (var phoneNumber in PhoneNumbers)
                        {
                            options.Add(new ActionSheetOption(phoneNumber, () => Device.BeginInvokeOnMainThread(() => Device.OpenUri(new Uri($"mailto:{phoneNumber}")))));
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

        public ICommand SmsCustomerCommand
        {
            get
            {
                return new Command(() =>
                {
                    if (PhoneNumbers != null || PhoneNumbers.Count > 0)
                    {

#pragma warning disable 4014
                        _analyticsService.Track("Sending SMS");
#pragma warning restore 4014

                        var options = new List<ActionSheetOption>();
                        foreach (var phoneNumber in PhoneNumbers)
                        {
                            options.Add(new ActionSheetOption(phoneNumber, () => Device.BeginInvokeOnMainThread(() => Device.OpenUri(new Uri($"sms:{phoneNumber}")))));
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

        public ObservableCollection<ImageSource> Attachments { get; set; }

        public List<byte[]> AttachmentsStreams { get; set; }

        public ICommand EmailCustomerCommand
        {
            get
            {
                return new Command(() =>
                {
                    if (Emails != null || Emails.Count > 0)
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
                        var started = await _appointmentService.FinishJob(JobNumber);
                        if (started)
                        {
                            _userDialogs.ShowSuccess("Job Finished");
                            await Task.Delay(100);
                            _userDialogs.HideLoading();
                            ShowActionButton = false;
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
                await NavigationService.NavigateToDashboardScreen();
                return;
            }
               
            if (job.Date != null)
                Date = job.Date.Value;
            StartTime = appointment.Start.TimeOfDay;
            EndTime = appointment.End.TimeOfDay;
            TimeRangeFormat = $"{appointment.Start.ToUniversalTime().ToString("hh:mm tt")} - {appointment.End.ToUniversalTime().ToString("hh:mm tt")}";
            DateTimeFormat = $"{Date.ToString("dddd dd MMMM yyyy")} at {TimeRangeFormat}";
            Location = appointment.Location;
            Title = appointment.Title;
            Description = job.Description;
            CustomerName = job.Customer.Name;
            Contact = job.ContactName;
            JobNumber = job.Id;
            Status = job.StatusDescription;
            GpsPoint = job.Point;

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

            if (GpsPoint.IsValid)
                this.Publish(new ShowPinOnMap(GpsPoint, Location, Contact));

            PageTitle = appointment.Title;
            if (!string.IsNullOrWhiteSpace(appointment.JobType))
                PageTitle = PageTitle + $" [{appointment.JobType}]";

            ShowActionButton = true;
            if (Status == "Scheduled")
                ActionText = "Start";
            else if (Status == "InProgress")
                ActionText = "Finish";
            else
                ShowActionButton = false;

            ShowActionButton = true;
            if (Status == "Scheduled")
                ActionText = "Start";
            else if (Status == "In Progress")
                ActionText = "Finish";
            else
                ShowActionButton = false;

            PhoneNumbers = job.PhoneNumbers.Select(p => p.Number).ToList();
            Emails = job.Emails.Select(e => e.Value).ToList();
            DataLoaded = true;
            _userDialogs.HideLoading();

        }

        public ICommand AddNewNoteImage
        {
            get
            {
                return new Command(() =>
                {
#pragma warning disable 4014
                    _analyticsService.Track("Attaching Image");
#pragma warning restore 4014

                    var options = new List<ActionSheetOption>();
                    options.Add(new ActionSheetOption("Take a photo", async () => await TakeAPhoto()));
                    options.Add(new ActionSheetOption("Select a photo", async () => await SelectAPhoto()));

                    _userDialogs.ActionSheet(new ActionSheetConfig()
                    {
                        Options = options,
                        Cancel = new ActionSheetOption("Cancel")
                    });
                });
            }
        }

        private async Task SelectAPhoto()
        {
            try
            {
                var mediaFile = await _mediaPicker.SelectPhotoAsync(GetCameraMediaStorageOptions());
                ResizeAddToAttachmentsAndAssignImageSource(mediaFile);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex);
            }
        }

        private async Task TakeAPhoto()
        {
#pragma warning disable 4014
            _analyticsService.Track("Taking a photo");
#pragma warning restore 4014

            try
            {
                var mediaFile = await _mediaPicker.TakePhotoAsync(GetCameraMediaStorageOptions()).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        return null;
                    }

                    return t.Result;
                });

                ResizeAddToAttachmentsAndAssignImageSource(mediaFile);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex);
            }
        }

        private void ResizeAddToAttachmentsAndAssignImageSource(MediaFile mediaFile)
        {
            if (mediaFile == null)
                return;

            _imageSource = null;

            var newImageStream = _imageCompressor.ResizeImage(mediaFile.Source, 0.2f);
            AttachmentsStreams.Add(_imageCompressor.ToArray(newImageStream));
            _imageSource = ImageSource.FromStream(() =>
            {
                newImageStream.Position = 0;
                return newImageStream;
            });

            Attachments.Add(_imageSource);
        }

        private static CameraMediaStorageOptions GetCameraMediaStorageOptions()
        {
            return new CameraMediaStorageOptions
            {
                DefaultCamera = CameraDevice.Rear,
                MaxPixelDimension = 400
            };
        }

        public void Dispose()
        {
            Notes.Clear();
            Attachments.Clear();
            AttachmentsStreams.Clear();
            CustomFields.Clear();
            DataLoaded = false;
        }

        public void OnAppearing()
        {
#pragma warning disable 4014
            _analyticsService.Screen("Job Details");
#pragma warning restore 4014
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