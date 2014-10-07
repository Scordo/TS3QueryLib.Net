using System;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Linq;

namespace TS3QueryLib.Core.Common
{
    public interface IDump { }

    public static class DumpExtender
    {
        public static string GetDumpString(this IDump instance)
        {
            return GetDumpString(instance, null);
        }

        public static string GetDumpString(this IDump instance, bool? includeTypeHeader)
        {
            includeTypeHeader = includeTypeHeader ?? true;
            StringBuilder result = new StringBuilder();

            if (includeTypeHeader.Value && instance != null)
                result.AppendLine(string.Format("[{0}]", instance.GetType()));

            instance.AddDumpString(result, includeTypeHeader.Value ? 1 : 0);

            return result.ToString();
        }

        public static void AddDumpString(this IDump instance, StringBuilder output, int depth)
        {
            AddDumpString(output, depth, instance);
        }

        private static void AddDumpString(StringBuilder output, int depth, object instance)
        {
            if (instance == null)
                return;

            Type[] excludedEnumerableTypes = new [] {typeof(string)};

            if (instance is IEnumerable && !excludedEnumerableTypes.Any(t => t == instance.GetType()))
            {
                int counter = 0;
                foreach (object listItem in (IEnumerable)instance)
                {
                    string propertyString = listItem is IDump ? string.Empty : Convert.ToString(listItem);
                    output.AppendFormat("{0}[{1}]: {2}{3}", string.Empty.PadLeft(depth, '\t'), counter, propertyString, Environment.NewLine);

                    if (listItem is IDump)
                        AddDumpString(output, depth + 1, listItem);

                    counter++;
                }
            }

            if (!(instance is IDump))
                return;

            foreach (PropertyInfo property in instance.GetType().GetProperties())
            {
                object propertyValue = property.GetValue(instance, null);

                if (propertyValue != null && propertyValue is IEnumerable && !excludedEnumerableTypes.Any(t => t == propertyValue.GetType()))
                {
                    output.AppendFormat("{0}{1}: {2}", string.Empty.PadLeft(depth, '\t'), property.Name, Environment.NewLine);
                    AddDumpString(output, depth + 1, propertyValue);
                }
                else
                {
                    string propertyString = propertyValue is IDump ? string.Empty : Convert.ToString(propertyValue);
                    output.AppendFormat("{0}{1}: {2}{3}", string.Empty.PadLeft(depth, '\t'), property.Name, propertyString, Environment.NewLine);

                    if (propertyValue is IDump)
                        AddDumpString(output, depth + 1, propertyValue);
                }
            }
        }
    }
}