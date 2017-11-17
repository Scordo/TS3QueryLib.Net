using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace TS3QueryLib.Core.CommandHandling
{
    public class CommandParameterGroup : List<CommandParameter>
    {
        #region Constructors

        public CommandParameterGroup()
        {

        }

        public CommandParameterGroup(IEnumerable<CommandParameter> parameters) : base(parameters)
        {

        }

        public CommandParameterGroup(int capacity): base(capacity)
        {

        }

        #endregion

        #region Public Methods

        public CommandParameter GetParameter(string name)
        {
            return this.FirstOrDefault(cp => string.Compare(cp.Name, name, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        public override string ToString()
        {
            return string.Join(" ", this.Select(c => c.ToString()).ToArray());
        }

        public string GetParameterValue(string parameterName)
        {
            return GetParameterValue(parameterName, null);
        }

        public T GetParameterValue<T>(string parameterName)
        {
            string parameterValue = GetParameterValue(parameterName, null);

            return ConvertValue<T>(parameterName, parameterValue);
        }

        public string GetParameterValue(string parameterName, string defaultValue)
        {
            CommandParameter parameter = GetParameter(parameterName);
            return parameter == null ? defaultValue : parameter.Value;
        }

        #endregion

        public static T ConvertValue<T>(string parameterName, string parameterValue)
        {
            Type targetType = typeof(T);

            try
            {
                if (default(T) != null && parameterValue == null)
                    throw new InvalidCastException(string.Format("Can not cast null value of parameter '{0}' to target type '{1}'.", parameterName, targetType));

                if ( parameterValue != null && (targetType == typeof(uint) || targetType == typeof(uint?)))
                {
                    decimal decimalValue = Convert.ToDecimal(parameterValue, CultureInfo.InvariantCulture);

                    if (decimalValue < 0)
                        return (T)(object)BitConverter.ToUInt32(BitConverter.GetBytes((long)decimalValue), 0);

                    return (T)(object)BitConverter.ToUInt32(BitConverter.GetBytes((ulong)decimalValue), 0);
                }

                return parameterValue.ChangeTypeInvariant<T>();
            }
            catch (OutOfMemoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidCastException(string.Format("Could not cast parameter with name '{0}' and value '{1}' to target type of '{2}'.", parameterName, parameterValue, targetType), ex);
            }
        }
    }
}