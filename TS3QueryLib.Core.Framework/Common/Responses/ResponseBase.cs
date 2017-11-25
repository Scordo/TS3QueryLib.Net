using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Exceptions;

namespace TS3QueryLib.Core.Common.Responses
{
    public abstract class ResponseBase<T> : IDump, IResponse where T : ResponseBase<T>
    {
        #region Properties

        public uint ErrorId { get; protected set; }
        public string ErrorMessage { get; protected set; }
        public uint? FailedPermissionId { get; protected set; }
        public bool IsErroneous { get { return (ErrorId != 0 && ErrorId != 1281) || IsBanned || ResponseText == null; } }
        public bool IsBanned { get; protected set; }
        public string BanExtraMessage { get; protected set; }
        public string BodyText { get; protected set; }
        public string StatusText { get; protected set; }
        public string ResponseText { get; protected set; }

        #endregion

        #region Public Methods

        public static T Parse(string response, params object[] additionalStates)
        {
            try
            {
                T instance = (T)Activator.CreateInstance(typeof(T));
                instance.ResponseText = response;

                if (response != null)
                {
                    SplitResponse(response, out var body, out var statusLine);
                    instance.BodyText = body;
                    instance.StatusText = statusLine;

                    instance.DetermineErrorDetails(statusLine);
                    instance.FillFrom(response, additionalStates);
                }

                return instance;
            }
            catch (Exception e)
            {
                throw new ParseException($"Error while trying to parse the response.\n\nRaw-Response:"+response, e);
            }

        }

        #endregion

        #region Non Public Methods

        protected abstract void FillFrom(string responseText, params object[] additionalStates);

        private void DetermineErrorDetails(string statusLine)
        {
            CommandParameterGroupList list = CommandParameterGroupList.Parse(statusLine);

            if (list.Count == 0)
                return;

            ErrorId = list.GetParameterValue<uint>("id");
            ErrorMessage = list.GetParameterValue("msg");
            BanExtraMessage = list.GetParameterValue("extra_msg");
            FailedPermissionId = list.GetParameterValue<uint?>("failed_permid");
            IsBanned = ErrorId == 3329 || ErrorId == 3331;
        }

        private static void SplitResponse(string response, out string body, out string statusLine)
        {
            response = response.Trim();
            int index = response.LastIndexOf(Ts3Util.QUERY_LINE_BREAK);

            body = index == -1 ? null : response.Substring(0, index).Trim();
            statusLine = index == -1 ? response : response.Substring(index + 2).Trim();
        }

        protected virtual void Assign(IResponse response)
        {
            ErrorId = response.ErrorId;
            ErrorMessage = response.ErrorMessage;
            FailedPermissionId = response.FailedPermissionId;
            IsBanned = response.IsBanned;
            BanExtraMessage = response.BanExtraMessage;
            BodyText = response.BodyText;
            StatusText = response.StatusText;
            ResponseText = response.ResponseText;
        }

        #endregion
    }

    public interface IResponse
    {
        uint ErrorId { get; }
        string ErrorMessage { get; }
        uint? FailedPermissionId { get; }
        bool IsBanned { get; }
        string BanExtraMessage { get; }
        string BodyText { get; }
        string StatusText { get; }
        string ResponseText { get; }
    }
}