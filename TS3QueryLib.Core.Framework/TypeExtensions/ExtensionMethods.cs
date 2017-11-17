using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using TS3QueryLib.Core.Common;

namespace System
{
    public static class ExtensionMethods
    {
        public static bool IsNullOrTrimmedEmpty(this string text)
        {
            return text == null || text.Trim().Length == 0;
        }

        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return dictionary == null ? null : new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }

        public static string[] ToArray(this IDictionary<string, string> dictionary)
        {
            if (dictionary == null)
                return null;

            List<string> values = new List<string>();

            foreach (var pair in dictionary)
            {
                values.Add(pair.Key);
                values.Add(pair.Value);
            }

            return values.ToArray();
        }

        public static T ChangeTypeInvariant<T>(this object sourceValue)
        {
            return ChangeTypeInvariant(sourceValue, default(T));
        }

        public static T ChangeTypeInvariant<T>(this object sourceValue, T defaultValue)
        {
            return ChangeType(sourceValue, defaultValue, CultureInfo.InvariantCulture);
        }

        public static T ChangeType<T>(this object sourceValue, IFormatProvider formatProvider)
        {
            return ChangeType(sourceValue, default(T), formatProvider);
        }

        public static T ChangeType<T>(this object sourceValue)
        {
            return ChangeType(sourceValue, default(T), null);
        }

        public static T ChangeType<T>(this object sourceValue, T defaultValue)
        {
            return ChangeType(sourceValue, defaultValue, null);
        }

        public static T ChangeType<T>(this object sourceValue, T defaultValue, IFormatProvider formatProvider)
        {
            if (formatProvider == null)
                formatProvider = Thread.CurrentThread.CurrentCulture;

            Type targetType = typeof(T);
            bool targetTypeIsNullableValueTyoe = targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>);

            if (targetTypeIsNullableValueTyoe && sourceValue == null)
                return defaultValue;

            if (targetTypeIsNullableValueTyoe)
                targetType = Nullable.GetUnderlyingType(targetType);

            return (T)Convert.ChangeType(sourceValue, targetType, formatProvider);
        }

        public static bool ToBool(this byte value)
        {
            return value == 1;
        }

        public static bool ToBool(this string value)
        {
            return value == "1";
        }

        public static bool? ToNullableBool(this string value)
        {
            return value == null ? null : (bool?) (value == "1");
        }

        public static bool? ToNullableBool(this byte? value)
        {
            return value.HasValue ? (bool?) (value == 1) : null;
        }

        public static List<uint> ToIdList(this string idString)
        {
            List<uint> result = new List<uint>();

            if (!idString.IsNullOrTrimmedEmpty())
            {
                string[] idArray = idString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string singleIdString in idArray)
                {
                    uint id;
                    if (uint.TryParse(singleIdString, NumberStyles.Number, CultureInfo.InvariantCulture, out id))
                        result.Add(id);
                }
            }

            return result;
        }

        /// <summary>
        /// Posts on the provided SyncContext if it is not <value>null</value>, otherwise executes the callback on the current thread.
        /// </summary>
        /// <param name="syncContext">The synchronize context.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        public static void PostEx(this SynchronizationContext syncContext, SendOrPostCallback callback, object state)
        {
            if (syncContext == null)
                callback(state);
            else
                syncContext.Post(callback, state);
        }
    }
}