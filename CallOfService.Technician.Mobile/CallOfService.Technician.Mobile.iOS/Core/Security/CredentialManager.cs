using System;
using CallOfService.Technician.Mobile.Core.Security;
using CallOfService.Technician.Mobile.Core.SystemServices;
using Foundation;
using Newtonsoft.Json;
using ObjCRuntime;
using Security;

namespace CallOfService.Technician.Mobile.iOS.Core.Security
{
    public class CredentialManager : ICredentialManager
    {

        private string ServiceId = "CallOfServiceLogin";
        private string Label = "CallOfServiceLogin";


        private readonly ILogger _logger;

        public CredentialManager(ILogger logger)
        {
            _logger = logger;
        }
        
        public bool Save(Credential credential)
        {
            try
            {
                DeletedOldSecureRecords();

                var secureRecord = GetNewSecureRecord(credential);

                var status = SecKeyChain.Add(secureRecord);

                if (status == SecStatusCode.Success)
                {
                    _logger.WriteInfo("User credentials has bee added to key-chain");
                    return true;
                }

                _logger.WriteInfo($"User credentials has not been added to key-chain with status code {status}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.WriteError($"Error Persisting User Credentials , {ex}");
                return false;
            }
        }

        public Credential Retrive(string account)
        {
            var match = SearchForSecRecord(account);
            if (match == null)
            {
                _logger.WriteInfo($"User credentials for user-name {account} not found");
                return null;
            }

            _logger.WriteInfo($"Found user credentials for user-name {match.Account}");


            if (match.Generic == null)
                return new Credential();

            var credential = JsonConvert.DeserializeObject<Credential>(match.Generic.ToString());
            return new Credential(credential.Email,credential.Password,credential.Token);
        }

        private string GetSavedAccount()
        {
            var cred = Retrive(null);
            return cred == null ? null : cred.Email;
        }

        private void DeletedOldSecureRecords()
        {
            var savedRecords = SearchForAllSecRecord();

            if (savedRecords == null) return;

            foreach (var record in savedRecords)
            {
                var toBeRemovedSecureRecord = new SecRecord(SecKind.GenericPassword)
                {
                    Account = record.Account,
                    Service = ServiceId,
                    Label = Label
                };

                var removeStatus = SecKeyChain.Remove(toBeRemovedSecureRecord);
                if (removeStatus == SecStatusCode.Success)
                    _logger.WriteInfo($"Removed duplicate record for account {record.Account}");
            }
        }

        private SecRecord GetNewSecureRecord(Credential credential)
        {
            var secureRecord = new SecRecord(SecKind.GenericPassword)
            {
                Account = credential.Email,
                Generic = NSData.FromString(JsonConvert.SerializeObject(credential), NSStringEncoding.UTF8),
                Service = ServiceId,
                Label = Label,
                Accessible = SecAccessible.Always
            };
            
            return secureRecord;
        }

        private SecRecord SearchForSecRecord(string account)
        {
            var searchQuery = GetSearchQuery(account);

            SecStatusCode status;
            var match = SecKeyChain.QueryAsRecord(searchQuery, out status);

            return status == SecStatusCode.Success ? match : null;
        }

        private SecRecord[] SearchForAllSecRecord()
        {
            var searchQuery = new SecRecord(SecKind.GenericPassword)
            {
                Service = ServiceId,
                Label = Label
            };


            SecStatusCode status;
            var match = SecKeyChain.QueryAsRecord(searchQuery, 100, out status);

            return status == SecStatusCode.Success ? match : null;
        }

        private bool InSimulator()
        {
            return Runtime.Arch == Arch.SIMULATOR;
        }

        private SecRecord GetSearchQuery(string account)
        {
            var searchCriteria = new SecRecord(SecKind.GenericPassword)
            {
                Service = ServiceId,
                Label = Label
            };

            if (!string.IsNullOrEmpty(account))
                searchCriteria.Account = account;
            
            return searchCriteria;
        }
    }
    
}