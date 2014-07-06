using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public class LogEntry : LogEntryLight, IDump
    {
        #region Properties

        public DateTime TimeStamp { get; protected set; }
        public uint LastPos { get; protected set; }
        public uint FileSize { get; protected set; }

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

            String message = currentParameterGroup.GetParameterValue("l");

            return new LogEntry
            {
                LastPos = firstParameterGroup.GetParameterValue<uint>("last_pos"),
                FileSize = firstParameterGroup.GetParameterValue<uint>("file_size"),
                Message = message,
                TimeStamp = DateTime.Parse(message.Split('|')[0])
            };
        }

        #endregion
    }
}