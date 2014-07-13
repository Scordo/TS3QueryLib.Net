using System;
using TS3QueryLib.Core.CommandHandling;

namespace TS3QueryLib.Core.Server.Entities
{
    public class LogEntryLight
    {
        #region Properties

        public string Message { get; set; }

        #endregion

        #region Constructor

        protected LogEntryLight()
        {

        }

        public LogEntryLight(string message)
        {
            if (message.IsNullOrTrimmedEmpty())
                throw new ArgumentException("message is null or trimmed empty");

            Message = message;
        }

        #endregion
    }
}