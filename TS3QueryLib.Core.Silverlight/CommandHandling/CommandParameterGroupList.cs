using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TS3QueryLib.Core.Common;
using System.Linq;

namespace TS3QueryLib.Core.CommandHandling
{
    public class CommandParameterGroupList : List<CommandParameterGroup>
    {
        #region Constructors

        public CommandParameterGroupList()
        {

        }

        public CommandParameterGroupList(IEnumerable<CommandParameterGroup> groups) : base(groups)
        {

        }

        public CommandParameterGroupList(int capacity): base(capacity)
        {

        }

        #endregion

        #region Public Methods

        public void AddRaw(string rawText)
        {
            AddParameter(rawText, null, 0, false);
        }

        public void AddParameter(string name)
        {
            AddParameter(name, null);
        }

        public void AddParameter(string name, string value)
        {
            AddParameter(name, value, null);
        }

        public void AddParameter(string name, string value, uint? groupIndex, bool encodeNameWhenValueIsNull = true)
        {
            groupIndex = groupIndex ?? 0;

            if (groupIndex > Count)
                throw new ArgumentOutOfRangeException(string.Format("Can not add parameter '{0}' with value '{1}' to group with index '{2}', because the index is '{3}' too big.", name, value, groupIndex, Count-groupIndex));

            if (groupIndex == Count)
                Add(new CommandParameterGroup{new CommandParameter(name, value, encodeNameWhenValueIsNull) });
            else
                this[(int) groupIndex].Add(new CommandParameter(name, value, encodeNameWhenValueIsNull));
        }

        public CommandParameter GetParameter(string name)
        {
            return GetParameter(name, null);
        }

        public CommandParameter GetParameter(string name, uint? groupIndex)
        {
            groupIndex = groupIndex ?? 0;
            if (groupIndex >= Count)
                return null;

            return this[(int) groupIndex].GetParameter(name);
        }

        public override string ToString()
        {
            return string.Join("|", this.Select(cpg => cpg.ToString()).ToArray());
        }

        public static CommandParameterGroupList Parse(string source)
        {
            if (source.IsNullOrTrimmedEmpty())
                return new CommandParameterGroupList();

            CommandParameterGroupList result = new CommandParameterGroupList();

            foreach (string groupText in source.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries))
            {
                CommandParameterGroup group = new CommandParameterGroup();

                foreach (string parameterText in Regex.Split(groupText, " +", RegexOptions.Singleline))
                {
                    int equalSignIndex = parameterText.IndexOf('=');

                    if (equalSignIndex > 0)
                        group.Add(new CommandParameter(parameterText.Substring(0, equalSignIndex), Ts3Util.DecodeString(parameterText.Substring(equalSignIndex+1))));
                    else if (equalSignIndex == -1)
                        group.Add(new CommandParameter(Ts3Util.DecodeString(parameterText)));
                }

                if (group.Count > 0)
                    result.Add(group);
            }

            return result;
        }

        public string GetParameterValue(string parameterName)
        {
            return GetParameterValue(parameterName, null);
        }

        public T GetParameterValue<T>(string parameterName)
        {
            string parameterValue = GetParameterValue(parameterName, null);

            return CommandParameterGroup.ConvertValue<T>(parameterName, parameterValue);
        }

        public string GetParameterValue(string parameterName, string defaultValue)
        {
            CommandParameter parameter = GetParameter(parameterName);
            return parameter == null ? defaultValue : parameter.Value;
        }

        #endregion
    }
}