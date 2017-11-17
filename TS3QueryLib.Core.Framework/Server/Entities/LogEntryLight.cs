using System;
using TS3QueryLib.Core.CommandHandling;

namespace TS3QueryLib.Core.Server.Entities
{
    public class LogEntryLight
    {
        #region Properties

        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }

        #endregion

        #region Constructor

        protected LogEntryLight()
        {

        }

        public LogEntryLight(LogLevel logLevel, string message)
        {
            if (message.IsNullOrTrimmedEmpty())
                throw new ArgumentException("message is null or trimmed empty");

            LogLevel = logLevel;
            Message = message;
        }

        #endregion
    }
}