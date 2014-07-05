using TS3QueryLib.Core.CommandHandling;

namespace TS3QueryLib.Core.Common.Entities
{
    public abstract class ClientModificationBase : ModificationBase
    {
        #region Properties

        public string Nickname { get; set; }

        #endregion

        #region Public Methods

        public virtual void AddToCommand(Command command)
        {
            AddToCommand(command, "client_nickname", Nickname);
        }

        #endregion
    }
}
