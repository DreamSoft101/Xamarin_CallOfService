using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CallOfService.Mobile.Services.Abstracts;
using Xamarin.Forms;
using System.Windows.Input;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Core;
using CallOfService.Mobile.UI;
using Plugin.Media;
using Plugin.Media.Abstractions;
using PubSub;
using Segment.Model;

namespace CallOfService.Mobile.Features.JobDetails
{
    public class JobNoteViewModel : ViewModelBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IUserDialogs _userDialogs;
        private ImageSource _imageSource;
        private readonly IAnalyticsService _analyticsService;
        private readonly IImageCompressor _imageCompressor;
        private readonly ILogger _logger;

        public JobNoteViewModel(IAppointmentService appointmentService, IUserDialogs userDialogs, IImageCompressor imageCompressor, IAnalyticsService analyticsService, ILogger logger)
        {
            _appointmentService = appointmentService;
            _userDialogs = userDialogs;
            _imageCompressor = imageCompressor;
            _analyticsService = analyticsService;
            _logger = logger;
            NewNoteText = string.Empty;
            Attachments = new ObservableCollection<ImageSource>();
            AttachmentsStreams = new List<byte[]>();

            this.Subscribe<ViewJobNoteDetails>(m => LoadJobeDetails(m.JobId));
        }

        public ICommand NavigateBack
        {
            get
            {
                return new Command(async () =>
                {
					this.Publish(new ViewJobDetails(JobNumber));
                    await NavigationService.Navigation.PopModalAsync(true);
                });
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetPropertyValue(ref _title, value); }
        }

        private int _jobNumber;
        public int JobNumber
        {
            get { return _jobNumber; }
            set { SetPropertyValue(ref _jobNumber, value); }
        }

        private string _pageTitle;
        public string PageTitle
        {
            get { return _pageTitle; }
            set { SetPropertyValue(ref _pageTitle, value); }
        }

        private string _newNoteText;
        public string NewNoteText
        {
            get { return _newNoteText; }
            set { SetPropertyValue(ref _newNoteText, value); RaisePropertyChanged("CanAddNote"); }
        }

        private bool _hasAttachment;
        public bool HasAttachment
        {
            get { return _hasAttachment; }
            set { SetPropertyValue(ref _hasAttachment, value); RaisePropertyChanged("CanAddNote"); }
        }

        public bool CanAddNote => !string.IsNullOrWhiteSpace(NewNoteText) || Attachments.Any() || HasAttachment;

        public ICommand AddNote
        {
            get
            {
                return new Command(async () =>
				{
					if (!CanAddNote)
						return;

                    _analyticsService.Track("Adding note");
                    _userDialogs.ShowLoading("Adding note");
                    var noteSaved = await _appointmentService.SubmitNote(JobNumber, NewNoteText, AttachmentsStreams, DateTime.Now);
                    if (noteSaved)
                    {
                        _userDialogs.HideLoading();
                        Attachments.Clear();
                        AttachmentsStreams?.Clear();
                        NewNoteText = string.Empty;

                        this.Publish(new ViewJobDetails(JobNumber));
                        await NavigationService.Navigation.PopModalAsync(true);
                    }
                    else
                    {
                        _userDialogs.HideLoading();
                        _userDialogs.ShowError("Error saving note please try again");
                    }
                });
            }
        }

        public ObservableCollection<ImageSource> Attachments { get; set; }

        public List<byte[]> AttachmentsStreams { get; set; }

        private void LoadJobeDetails(int jobId)
        {
            JobNumber = jobId;
        }

        public ICommand AddNewNoteImage
        {
            get
            {
                return new Command(() =>
                {
                    _analyticsService.Track("Attaching Image");

                    var options = new List<ActionSheetOption>
                    {
                        new ActionSheetOption("Take a photo", async () => await TakeAPhoto()),
                        new ActionSheetOption("Select a photo", async () => await SelectAPhoto())
                    };

                    _userDialogs.ActionSheet(new ActionSheetConfig
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
                var mediaFile = await CrossMedia.Current.PickPhotoAsync();

                if (mediaFile == null)
                    _userDialogs.ShowError("Error while attaching photo, please try again.");
                else
                    ResizeAddToAttachmentsAndAssignImageSource(mediaFile);

                HasAttachment = Attachments.Any();
            }
            catch (Exception ex)
            {
                _logger.WriteError("Exceptoin while selecting a photo", exception: ex);

                _analyticsService.Track("Error Selecting a photo", new Properties
                {
                    {"exception", ex}
                });

                _userDialogs.ShowError("Error while attaching photo, please try again.");

                HasAttachment = Attachments.Any();
            }
        }

        private async Task TakeAPhoto()
        {
            _analyticsService.Track("Taking a photo");

            try
            {
                var mediaFile = await CrossMedia.Current.TakePhotoAsync(GetCameraMediaStorageOptions());

                if (mediaFile == null)
                    _userDialogs.ShowError("Error while taking photo, please try again or select existing photo.");
                else
                    ResizeAddToAttachmentsAndAssignImageSource(mediaFile);

                HasAttachment = Attachments.Any();
            }
            catch (Exception ex)
            {
                _logger.WriteError("Exceptoin while taking a photo", exception: ex);
                _analyticsService.Track("Error Taking a photo", new Properties
                {
                    {"exception", ex}
                });
                _userDialogs.ShowError("Error while taking photo, please try selecting existing photo.");
                HasAttachment = Attachments.Any();
            }
        }

        private void ResizeAddToAttachmentsAndAssignImageSource(MediaFile mediaFile)
        {
            if (mediaFile == null)
                return;

            _imageSource = null;

			try
			{
				var newImageStream = _imageCompressor.ResizeImage(mediaFile.GetStream(), 0.5f);
				//mediaFile.Dispose();
				AttachmentsStreams.Add(_imageCompressor.ToArray(newImageStream));
				_imageSource = ImageSource.FromStream(() =>
			    {
				   newImageStream.Position = 0;
				   return newImageStream;
			    });
			}
			catch (Exception e)
			{
			    try
			    {
			        _logger.WriteError("Exceptoin while manipulating photo", exception: e);
			        _analyticsService.Track("Error Compressing a photo", new Properties
			        {
			            {"exception", e}
			        });

			        AttachmentsStreams.Add(_imageCompressor.ToArray(mediaFile.GetStream()));
			        _imageSource = ImageSource.FromStream(() =>
			        {
			            var stream = mediaFile.GetStream();
			            //mediaFile.Dispose();
			            return stream;
			        });
			    }
			    catch (Exception ex)
			    {
                    _logger.WriteError("Exceptoin while manipulating photo", exception: ex);
                    _analyticsService.Track("Error attaching a photo", new Properties
                    {
                        {"exception", ex}
                    });
                }
			}

            Attachments.Add(_imageSource);
        }

        private StoreCameraMediaOptions GetCameraMediaStorageOptions()
        {
            return new StoreCameraMediaOptions
            {
                DefaultCamera = CameraDevice.Rear,
                Directory = "Notes",
                Name = $"{JobNumber}-{Guid.NewGuid()}-Note.jpg"
            };
        }

        public override void Dispose()
        {
            Attachments?.Clear();
            AttachmentsStreams?.Clear();
        }

        public override void OnAppearing()
        {
            _analyticsService.Screen("Job Note");
        }
    }
}