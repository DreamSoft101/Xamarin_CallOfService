using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallOfService.Technician.Mobile.Core.Security
{
    public class Credential
    {

        public bool Valid { get; set; }

        /// <summary>
        /// Represent Empty Credential which means user never logged in
        /// </summary>
        public Credential()
        {
            Valid = false;
        }

        public Credential(string email, string password, string token)
        {
            Valid = true;
            Email = email;
            Password = password;
            Token = token;
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
