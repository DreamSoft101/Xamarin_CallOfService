using System;
using System.Collections.Generic;
using System.Diagnostics;
using Elmah.Io.Client.Models;

namespace CallOfService.Mobile.Core.SystemServices
{
    public class DebugLogger : ILogger
    {
        public void WriteInfo(string message, string details = null, IList<Item> dataList = null)
        {
            Debug.WriteLine(message);
        }

        public void WriteWarning(string message, string details = null, Exception exception = null, IList<Item> dataList = null)
        {
            Debug.WriteLine(message);
        }

        public void WriteError(string message, string details = null, Exception exception = null, IList<Item> dataList = null)
        {
            Debug.WriteLine(message);
        }
    }
}
