using System;
using System.Collections.Generic;
using Elmah.Io.Client.Models;

namespace CallOfService.Mobile.Core.SystemServices
{
    public interface ILogger
    {
        void WriteInfo(string message, string details = null, IList<Item> dataList = null);
        void WriteWarning(string message, string details = null, Exception exception = null, IList<Item> dataList = null);
        void WriteError(string message, string details = null, Exception exception = null, IList<Item> dataList = null);
    }
}