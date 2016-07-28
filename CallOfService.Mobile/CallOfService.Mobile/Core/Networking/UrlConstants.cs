namespace CallOfService.Mobile.Core.Networking
{
    public class UrlConstants
    {
        public static string BaseUrl = "https://dashboard.callofservice.com/api/";
        public const string LoginUrl = "auth/login";
		public const string LogoutUrl = "auth/logout";
        public const string AppointmentsUrl = "appointments";
        public const string JobByIdUrl = "job/";
        public const string FileUrl = "file/";
        public const string StartJob = "job/start";
        public const string FinishJob = "job/finish";
        public const string NewNoteUrl = "jobnote";
        public const string SendLocation = "workerlocation";
        public const string Availability = "workeravailability";
    }
}
