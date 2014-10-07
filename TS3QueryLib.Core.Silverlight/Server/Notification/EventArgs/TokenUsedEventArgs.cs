using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Notification.EventArgs
{
    public class TokenUsedEventArgs : System.EventArgs, IDump
    {
        #region Properties

        public uint ClientId { get; protected set; }
        public string ClientUniqueId { get; protected set; }
        public uint ClientDatabaseId { get; protected set; }
        public string TokenText { get; protected set; }
        public uint GroupId { get; protected set; }
        public uint ChannelId { get; protected set; }
        public ReadOnlyDictionary<string, string> CustomSettings { get; protected set; }

        #endregion

        #region Constructors

        internal TokenUsedEventArgs(CommandParameterGroupList commandParameterGroupList)
        {
            if (commandParameterGroupList == null)
                throw new ArgumentNullException("commandParameterGroupList");

            const string PATTERN = @"ident=(?<ident>[^\s=]+)\s+(?<value>value=.*)";

            string customSettingsString = commandParameterGroupList.GetParameterValue("tokencustomset");
            Dictionary<string, string> customSettings = new Dictionary<string, string>();

            if (!customSettingsString.IsNullOrTrimmedEmpty())
            {
                foreach (string splittedSetting in customSettingsString.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Match match = Regex.Match(splittedSetting, PATTERN, RegexOptions.Singleline | RegexOptions.IgnoreCase);

                    if (!match.Success)
                        continue;

                    customSettings[match.Groups["ident"].Value] = match.Groups["value"].Value;
                }
            }

            ClientId = commandParameterGroupList.GetParameterValue<uint>("clid");
            ClientUniqueId = commandParameterGroupList.GetParameterValue("cluid");
            ClientDatabaseId = commandParameterGroupList.GetParameterValue<uint>("cldbid");
            TokenText = commandParameterGroupList.GetParameterValue("token");
            GroupId = commandParameterGroupList.GetParameterValue<uint>("token1");
            ChannelId = commandParameterGroupList.GetParameterValue<uint>("token2");
            CustomSettings = customSettings.AsReadOnly();
        }

        #endregion
    }
}