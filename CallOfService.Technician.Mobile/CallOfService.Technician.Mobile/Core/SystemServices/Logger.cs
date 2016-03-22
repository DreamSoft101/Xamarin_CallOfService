using System;
using System.Diagnostics;

namespace CallOfService.Technician.Mobile.Core.SystemServices
{
    public class Logger : ILogger
    {
        public void WriteInfo(string message)
        {
            Debug.WriteLine(message);
        }

        public void WriteError(string message)
        {
            Debug.WriteLine(message);
        }

        public void WriteError(Exception exception)
        {
            Debug.WriteLine(exception.ToString());
        }
    }

    public interface ILogger
    {
        void WriteInfo(string message);
        void WriteError(string message);
        void WriteError(Exception exception);
    }
}
