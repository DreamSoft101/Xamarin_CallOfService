using System.Windows.Input;
using CallOfService.Mobile.UI;
using Xamarin.Forms;
using PubSub;

namespace CallOfService.Mobile.Features.Jobs
{
    public class JobsViewModel : ViewModelBase
    {
        private JobsViewModelData _model;
        public JobsViewModelData Model
        {
            get { return _model; }
            set { SetPropertyValue(ref _model, value); }
        }
    }

    //Check if you need notify property changed
    public class AppointmentModel
    {
        public string Title { get; set; }
        public string CustomerName { get; set; }

        public string StartTimeEndTimeFormated { get; set; }

        public string Location { get; set; }

        public int JobId { get; set; }

        public double? LocationLatitude { get; set; }
        public double? LocationLongitude { get; set; }

        public bool IsFinished { get; set; }
        public bool IsInProgress { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsScheduled => !IsFinished && !IsInProgress && !IsCancelled;

        public ICommand ViewDetails { get { return new Command(() => this.Publish(new JobSelected(JobId))); } }
    }
}