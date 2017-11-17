using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public class ComplainListEntry: IDump
    {
        #region Properties

        public uint TargetClientDatabaseId { get; protected set; }
        public string TargetName { get; protected set; }
        public uint SourceClientDatabaseId { get; protected set; }
        public string SourceName { get; protected set; }
        public string Message { get; protected set; }
        public DateTime Created { get; protected set; }

        #endregion

        #region Constructor

        private ComplainListEntry()
        {

        }

        #endregion

        #region Public Methods

        public static ComplainListEntry Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            return new ComplainListEntry
            {
                TargetClientDatabaseId = currentParameterGroup.GetParameterValue<uint>("tcldbid"),
                TargetName = currentParameterGroup.GetParameterValue("tname"),
                SourceClientDatabaseId = currentParameterGroup.GetParameterValue<uint>("fcldbid"),
                SourceName = currentParameterGroup.GetParameterValue("fname"),
                Message = currentParameterGroup.GetParameterValue("message"),
                Created = new DateTime(1970, 1, 1).AddSeconds(currentParameterGroup.GetParameterValue<ulong>("timestamp")),
            };
        }

        #endregion
    }
}