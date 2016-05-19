using SQLite.Net.Attributes;

namespace CallOfService.Mobile.Domain
{
    [Table("UserProfile")]
    public class UserProfile
    {
        [PrimaryKey]
        public int UserId { get; set; }
        public string TenantId { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PhotoUrl { get; set; }
		[Ignore]
        public string[] Roles { get; set; }
        public bool IsMainAdmin { get; set; }
        public bool PasswordReset { get; set; }
        public string StripeCustomerId { get; set; }
        public string Source { get; set; }
        public string ReferrerUrl { get; set; }

        public UserProfile()
        { }
    }
    
}
