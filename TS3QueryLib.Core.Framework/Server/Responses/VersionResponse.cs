using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Responses;

namespace TS3QueryLib.Core.Server.Responses
{
    public class VersionResponse : ResponseBase<VersionResponse>
    {
        #region Properties

        public string Version { get; protected set; }
        public string Build { get; protected set; }
        public string Platform { get; protected set; }

        #endregion

        #region Non Public Methods

        protected override void FillFrom(string responseText, params object[] additionalStates)
        {
            CommandParameterGroupList list = CommandParameterGroupList.Parse(BodyText);

            if (list.Count == 0)
                return;

            Version = list.GetParameterValue("version");
            Build = list.GetParameterValue("build");
            Platform = list.GetParameterValue("platform");
        }

        #endregion
    }
}