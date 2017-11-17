using TS3QueryLib.Core.Server.Responses;

namespace TS3QueryLib.Core.Common.Responses
{
    public abstract class WhoAmIResponseBase<T> : ResponseBase<T> where T: WhoAmIResponseBase<T>
    {
        #region Properties

        public uint ClientId { get; protected set; }
        public uint ChannelId { get; protected set; }

        #endregion
    }
}