using TS3QueryLib.Core.CommandHandling;
using System;

namespace TS3QueryLib.Core.Common.Responses
{
    public class SingleValueResponse<T> : ResponseBase<SingleValueResponse<T>>
    {
        #region Properties

        public T Value { get; protected set; }

        #endregion

        #region Non Public Methods

        protected override void FillFrom(string responseText, params object[] additionalStates)
        {
            CommandParameterGroupList list = CommandParameterGroupList.Parse(BodyText);

            if (list.Count == 0 || additionalStates == null || additionalStates.Length == 0)
            {
                Value = default(T);
                return;
            }

            Value = list.GetParameterValue(additionalStates[0].ToString()).ChangeTypeInvariant(default(T));
        }

        #endregion
    }
}