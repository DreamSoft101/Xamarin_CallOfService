using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;

namespace CallOfService.Mobile.Core.Security
{
    public class CredentialManager : ICredentialManager
    {
        public bool Save(Credential credential)
        {
            var account = new Account
            {
                Username = credential.Email
            };
            account.Properties.Add("Password", credential.Password);
            account.Properties.Add("AuthToken", credential.Token);
            AccountStore.Create().Save(account, App.AppName);
            return true;
        }

        public Credential Retrive()
        {
            var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
            if (account == null) return null;

            return new Credential(account.Username, account.Properties["Password"], account.Properties["AuthToken"]);
        }

        public void Delete()
        {
            var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
            if (account != null)
            {
                AccountStore.Create().Delete(account, App.AppName);
            }
        }
    }
}