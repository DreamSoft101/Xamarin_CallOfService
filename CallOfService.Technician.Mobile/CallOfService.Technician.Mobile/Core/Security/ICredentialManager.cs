using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallOfService.Technician.Mobile.Core.Security
{
    public interface ICredentialManager
    {
        bool Save(Credential credential);
        Credential Retrive();
		void Delete();
    }
}
