using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CallOfService.Technician.Mobile.Domain;
using CallOfService.Technician.Mobile.Services.Abstracts;
using CallOfService.Technician.Mobile.UI;
using PropertyChanged;
using Xamarin.Forms;
using PubSub;
using CallOfService.Technician.Mobile.Core.SystemServices;
using Acr.UserDialogs;
using CallOfService.Technician.Mobile.Messages;
using XLabs.Platform.Services.Media;

namespace CallOfService.Technician.Mobile.Features.Jobs
{
	[ImplementPropertyChanged]
	public class JobsViewModel : IViewAwareViewModel
	{
		private readonly IAppointmentService _appointmentService;
		private readonly IUserDialogs _userDialogs;
		private ImageSource _imageSource;
		private IMediaPicker _mediaPicker;
		private IImageCompressor _imageCompressor;
		public JobsViewModel (IAppointmentService appointmentService, IUserDialogs userDialogs, IMediaPicker mediaPicker,IImageCompressor imageCompressor)
		{
			_userDialogs = userDialogs;
			_appointmentService = appointmentService;
			_mediaPicker = mediaPicker;
			_imageCompressor = imageCompressor;
			Appointments = new ObservableCollection<AppointmentViewModel> ();
			this.Subscribe<JobSelected> (async m => {
				await NavigationService.NavigateToJobDetails ();
				this.Publish (new ViewJobDetails (m.Appointment.JobId));
			});
			this.Subscribe<NewDateSelected> (m => {
				Date = m.DateTime;
				OnAppearing ();
			});
			Date = DateTime.Today;
		}

		public ObservableCollection<AppointmentViewModel> Appointments { get; set; }

		public DateTime Date { get; set; }

		public ICommand GoToNextDay {
			get {
				return new Command (() => {
					Date = Date.AddDays (1);
					OnAppearing ();
				});
			}
		}

		public ICommand GoToPrevDay {
			get {
				return new Command (() => {
					Date = Date.AddDays (-1);
					OnAppearing ();
				});
			}
		}

		public bool IsRefreshing { get; set; }

		public ICommand RefreshListOfJobsCommand {
			get { 
				return new Command (() => {
					OnAppearing ();
				}); 
			}
		}

		public void Dispose ()
		{   
			Appointments.Clear ();
		}

		public async void OnAppearing ()
		{
			IsRefreshing = true;

			var appointments = await _appointmentService.AppointmentsByDay (Date);
			Appointments.Clear ();
			foreach (var appointment in appointments) {
				Appointments.Add (new AppointmentViewModel {
					Title = appointment.Title,
					Location = appointment.Location,
					StartTimeEndTimeFormated =$"{appointment.StartString} - {appointment.EndString}",
					JobId = appointment.JobId
				});
			}

			IsRefreshing = false;
		}

		private async void SelectAPhoto ()
		{
			try {
				var mediaFile = await _mediaPicker.SelectPhotoAsync (GetCameraMediaStorageOptions ());
				ResizeAddToAttachmentsAndAssignImageSource (mediaFile);
			} catch (Exception) {
				// ignored
			}
		}

		private async void TakeAPhoto ()
		{
			try {
				var mediaFile = await _mediaPicker.TakePhotoAsync (GetCameraMediaStorageOptions ()).ContinueWith (t => {
					if (t.IsFaulted || t.IsCanceled) {
						return null;
					}

					return t.Result;
				});

				ResizeAddToAttachmentsAndAssignImageSource (mediaFile);
			} catch (Exception) {
				//Ignored
			}
		}

		private void ResizeAddToAttachmentsAndAssignImageSource (MediaFile mediaFile)
		{
			if (mediaFile == null)
				return;

			_imageSource = null;

			var newImageStream = _imageCompressor.ResizeImage (mediaFile.Source, 0.5f);
			_imageSource = ImageSource.FromStream (() => {
				newImageStream.Position = 0;
				return newImageStream;
			});
		}

		private static CameraMediaStorageOptions GetCameraMediaStorageOptions ()
		{
			return new CameraMediaStorageOptions {
				DefaultCamera = CameraDevice.Rear,
				MaxPixelDimension = 400
			};
		}

		public void OnDisappearing ()
		{
		}
	}

	[ImplementPropertyChanged]
	public class AppointmentViewModel
	{
		public string Title { get; set; }

		public string StartTimeEndTimeFormated { get; set; }

		public string Location { get; set; }

		public int JobId { get; set; }

		public ICommand ViewDetails { get { return new Command (() => this.Publish (new JobSelected (this))); } }
	}
}