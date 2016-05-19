using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallOfService.Mobile.Core.Security
{
    public interface ICredentialManager
    {
        bool Save(Credential credential);
        Credential Retrive();
		void Delete();
    }
}
