using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CallOfService.Mobile.Services.Abstracts;
using CallOfService.Mobile.UI;
using Xamarin.Forms;
using System.Windows.Input;
using CallOfService.Mobile.Core.SystemServices;
using PropertyChanged;
using CallOfService.Mobile.Core;
using Plugin.Media;
using Plugin.Media.Abstractions;
using PubSub;
using Segment.Model;

namespace CallOfService.Mobile.Features.JobDetails
{
    [ImplementPropertyChanged]
    public class JobNoteViewModel : IViewAwareViewModel
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
                    await NavigationService.Navigation.PopModalAsync(true);
                });
            }
        }

        public string Title { get; set; }
        public int JobNumber { get; set; }
        public string PageTitle { get; set; }
        public string NewNoteText { get; set; }
        public bool HasAttachment { get; set; }

        public bool CanAddNote => !string.IsNullOrWhiteSpace(NewNoteText) || Attachments.Any() || HasAttachment;

        public ICommand AddNote
        {
            get
            {
                return new Command(async () =>
				{
					if (!CanAddNote)
						return;
					
#pragma warning disable 4014
                    _analyticsService.Track("Adding note");
#pragma warning restore 4014

                    _userDialogs.ShowLoading("Adding note");
                    var noteSaved = await _appointmentService.SubmitNote(JobNumber, NewNoteText, AttachmentsStreams, DateTime.Now);
                    if (noteSaved)
                    {
                        _userDialogs.HideLoading();
                        Attachments.Clear();
                        AttachmentsStreams.Clear();
                        NewNoteText = string.Empty;
                        await NavigationService.Navigation.PopModalAsync(true);
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
#pragma warning disable 4014
                    _analyticsService.Track("Attaching Image");
#pragma warning restore 4014

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
                _logger.WriteError(ex);
#pragma warning disable 4014
                _analyticsService.Track("Error Selecting a photo", new Properties
                {
                    {"exception", ex.Message}
                });
#pragma warning restore 4014
                _userDialogs.ShowError("Error while attaching photo, please try again.");

                HasAttachment = Attachments.Any();
            }
        }

        private async Task TakeAPhoto()
        {
#pragma warning disable 4014
            _analyticsService.Track("Taking a photo");
#pragma warning restore 4014

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
                _logger.WriteError(ex);
#pragma warning disable 4014
                _analyticsService.Track("Error Taking a photo", new Properties
                {
                    {"exception", ex.Message}
                });
#pragma warning restore 4014
                _userDialogs.ShowError("Error while taking photo, please try selecting existing photo.");
                HasAttachment = Attachments.Any();
            }
        }

        private void ResizeAddToAttachmentsAndAssignImageSource(MediaFile mediaFile)
        {
            if (mediaFile == null)
                return;

            _imageSource = null;

            var newImageStream = _imageCompressor.ResizeImage(mediaFile.GetStream(), 0.2f);
            mediaFile.Dispose();
            AttachmentsStreams.Add(_imageCompressor.ToArray(newImageStream));
            _imageSource = ImageSource.FromStream(() =>
            {
                newImageStream.Position = 0;
                return newImageStream;
            });

            Attachments.Add(_imageSource);
        }

        private static StoreCameraMediaOptions GetCameraMediaStorageOptions()
        {
            return new StoreCameraMediaOptions
            {
                DefaultCamera = CameraDevice.Rear
            };
        }

        public void Dispose()
        {
            Attachments.Clear();
            AttachmentsStreams.Clear();
        }

        public void OnAppearing()
        {
#pragma warning disable 4014
            _analyticsService.Screen("Job Note");
#pragma warning restore 4014
        }

        public void OnDisappearing()
        {
        }
    }
}