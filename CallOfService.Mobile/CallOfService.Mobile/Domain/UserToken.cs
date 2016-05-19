using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallOfService.Mobile.Domain
{
    public class UserToken
    {
        public Token Token { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}
