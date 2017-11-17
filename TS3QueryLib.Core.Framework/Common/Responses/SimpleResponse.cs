namespace TS3QueryLib.Core.Common.Responses
{
    public class SimpleResponse : ResponseBase<SimpleResponse>
    {
        #region Non Public Methods

        protected override void FillFrom(string responseText, params object[] additionalStates)
        {
            // do nothing here
        }

        #endregion

    }
}