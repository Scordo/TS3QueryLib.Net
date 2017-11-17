using System;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.CommandHandling
{
    public class CommandParameter
    {
        #region Properties

        public string Name { get; protected set; }
        public string EncodedName { get { return Ts3Util.EncodeString(Name); } }
        public string Value { get; set; }
        public string EncodedValue { get { return Ts3Util.EncodeString(Value); } }
        public bool EncodeNameWhenValueIsNull { get; set; }

        #endregion

        #region Constructors

        public CommandParameter(string name) : this(name, null)
        {

        }

        public CommandParameter(string name, string value, bool encodeNameWhenValueIsNull = true)
        {
            if (name.IsNullOrTrimmedEmpty())
                throw new ArgumentException("name is null or trimmed empty", "name");

            Name = name.Trim();
            Value = value == null? null : value.Trim();
            EncodeNameWhenValueIsNull = encodeNameWhenValueIsNull;
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return Value == null ? (EncodeNameWhenValueIsNull ? EncodedName : Name) : string.Format("{0}={1}", Name, EncodedValue);
        }

        #endregion
    }
}