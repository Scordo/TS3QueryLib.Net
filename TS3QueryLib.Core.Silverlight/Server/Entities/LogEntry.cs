using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public class LogEntry : LogEntryLight, IDump
    {
        #region Properties

        public DateTime TimeStamp { get; protected set; }
        public string Channel { get; protected set; } 

        #endregion

        #region Constructor

        private LogEntry()
        {
            
        }

        #endregion

        #region Public Methods

        public static LogEntry Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            return new LogEntry
            {
                TimeStamp = new DateTime(1970, 1, 1).AddSeconds(currentParameterGroup.GetParameterValue<ulong>("timestamp")),
                LogLevel = (LogLevel)currentParameterGroup.GetParameterValue<uint>("level"),
                Channel = currentParameterGroup.GetParameterValue("channel"),
                Message = currentParameterGroup.GetParameterValue("msg")
            };
        }

        #endregion
    }
}