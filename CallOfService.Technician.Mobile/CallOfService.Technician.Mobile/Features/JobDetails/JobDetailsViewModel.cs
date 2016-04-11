using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CallOfService.Technician.Mobile.Core.DI;
using CallOfService.Technician.Mobile.Domain;
using CallOfService.Technician.Mobile.Services.Abstracts;
using CallOfService.Technician.Mobile.UI;
using PubSub;
using Xamarin.Forms;

namespace CallOfService.Technician.Mobile.Features.JobDetails
{
    public class JobDetailsViewModel : IViewAwareViewModel
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IUserDialogs _userDialogs;

        public JobDetailsViewModel(IAppointmentService appointmentService, IUserDialogs userDialogs)
        {
            _appointmentService = appointmentService;
            _userDialogs = userDialogs;
            Notes = new ObservableCollection<NoteViewModel>();
            this.Subscribe<ViewJobDetails>(async m => await LoadJobeDetails(m.JobId));
        }

        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Location { get; set; }
        public string Title { get; set; }
        public string Contact { get; set; }
        public string JobNumber { get; set; }
        public string Status { get; set; }
        public string Strata { get; set; }
        private ObservableCollection<NoteViewModel> Notes { get; set; }
        public string Custom { get; set; }

        private async Task LoadJobeDetails(int jobId)
        {
            _userDialogs.ShowLoading("Loading Job Details");
            Appointment appointment = await _appointmentService.GetAppointmentByJobId(jobId);
            var job = await _appointmentService.GetJobById(jobId);
            if (job.Date != null) Date = job.Date.Value;
            StartTime = appointment.Start.TimeOfDay;
            EndTime = appointment.End.TimeOfDay;
            Location = appointment.Location;
            Title = appointment.Title;
            Contact = job.ContactName;
            JobNumber = job.Id.ToString();
            Status = job.StatusDescription;
            Notes.Clear();
            foreach (var note in job.Notes)
            {
                var noteViewModel = DependencyResolver.Resolve<NoteViewModel>();
                noteViewModel.LoadNote(note);
                Notes.Add(noteViewModel);
            }

            _userDialogs.HideLoading();
        }

        public void Dispose()
        {

        }

        public void OnAppearing()
        {

        }

        public void OnDisappearing()
        {

        }
    }

    public class NoteViewModel
    {
        private readonly IAppointmentService _appointmentService;

        public NoteViewModel(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        public string Description { get; set; }
        public DateTime Date { get; set; }
        private ObservableCollection<ImageSource> ThumbnilImageSources { get; set; }

        public void LoadNote(Note note)
        {
            Description = note.Description;
            Date = note.Timestamp.DateTime;
            ThumbnilImageSources = new ObservableCollection<ImageSource>();
            foreach (var fileReference in note.Files)
            {
                ThumbnilImageSources.Add(new UriImageSource
                {
                    Uri = _appointmentService.GetFileUri(fileReference, true)
                });
            }
        }
    }
}