using System.Threading.Tasks;
using CallOfService.Mobile.Domain;

namespace CallOfService.Mobile.Database
{
    public class DbInitializer
    {
        private readonly IDbSet<UserProfile> _userProfileDbSet;
        private readonly IDbSet<Appointment> _appointmentDbSet;
        private readonly IDbSet<JobDetails> _jobDetailsDbSet;

        public DbInitializer(IDbSet<UserProfile> userProfileDbSet,IDbSet<Appointment> appointmentDbSet, IDbSet<JobDetails> jobDetailsDbSet)
        {
            _userProfileDbSet = userProfileDbSet;
            _appointmentDbSet = appointmentDbSet;
            _jobDetailsDbSet = jobDetailsDbSet;
        }

        public async Task InitDb()
        {
            await _userProfileDbSet.CreateTable();
            await _appointmentDbSet.CreateTable();
            await _jobDetailsDbSet.CreateTable();
        }   
    }
}
