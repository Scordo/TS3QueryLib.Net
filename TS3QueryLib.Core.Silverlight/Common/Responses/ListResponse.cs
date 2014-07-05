using System;
using System.Collections.Generic;
using System.Linq;
using TS3QueryLib.Core.CommandHandling;

namespace TS3QueryLib.Core.Common.Responses
{
    public class ListResponse<T> : ResponseBase<ListResponse<T>>, IEnumerable<T>
    {
        #region Properties

        public List<T> Values { get; protected set; }

        #endregion

        #region Constructor

        public ListResponse()
        {
            Values = new List<T>();
        }

        #endregion

        #region Public Methods

        public static ListResponse<T> Parse(string response, Func<CommandParameterGroup, CommandParameterGroup, T> parseMethod, params object[] additionalStates)
        {
            ListResponse<T> instance = Parse(response, additionalStates);

            instance.FillFrom(response, parseMethod, additionalStates);

            return instance;
        }

        #endregion

        #region Non Public Methods

        protected override void FillFrom(string responseText, params object[] additionalStates)
        {
            FillFrom(responseText, null, additionalStates);
        }

        protected virtual void FillFrom(string responseText, Func<CommandParameterGroup, CommandParameterGroup, T> parseMethod, params object[] additionalStates)
        {
            CommandParameterGroupList list = CommandParameterGroupList.Parse(BodyText);

            if (list.Count == 0 || ((additionalStates == null || additionalStates.Length == 0) && parseMethod == null))
            {
                Values = new List<T>();
                return;
            }

            if (parseMethod != null)
            {
                CommandParameterGroup first = list.Count == 0 ? null : list[0];
                Values = list.Select(x => parseMethod(x, first)).ToList();
            }
            else
                Values = list.Select(cpg => cpg.GetParameterValue(additionalStates[0].ToString()).ChangeTypeInvariant(default(T))).ToList();
        }

        protected IEnumerator<T> GetEnumerator()
        {
            if (Values == null)
                yield break;

            foreach (T value in Values)
            {
                yield return value;
            }
        }

        protected override void Assign(IResponse response)
        {
            base.Assign(response);

            ListResponse<T> listResponse = response as ListResponse<T>;
            if (listResponse == null)
                return;

            Values.Clear();
            Values.AddRange(listResponse);
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}