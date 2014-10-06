using System;
using System.Linq;
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

            DateTime timeStamp;
            DateTime.TryParse(message.Split('|')[0], System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out timeStamp);

            LogLevel logLevel = LogLevel.None;
            if (message.Split('|').Length >= 2 && Enum.GetNames(typeof(LogLevel)).Contains(message.Split('|')[1].Trim(), StringComparer.CurrentCultureIgnoreCase))
            {
                logLevel = (LogLevel)Enum.Parse(typeof(LogLevel), message.Split('|')[1].Trim(), true);
            }

            return new LogEntry
            {
                TimeStamp = timeStamp,
                LogLevel = logLevel,
                LastPos = firstParameterGroup.GetParameterValue<uint>("last_pos"),
                FileSize = firstParameterGroup.GetParameterValue<uint>("file_size"),
                Message = message
            };
        }

        #endregion
    }
}