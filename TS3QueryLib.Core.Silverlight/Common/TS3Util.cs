using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TS3QueryLib.Core.Common
{
    public class Ts3Util
    {
        #region Constants

        public const string QUERY_LINE_BREAK = "\n\r";
        public const string QUERY_REGEX_LINE_BREAK = @"\x0A\x0D";

        #endregion

        #region Non Public Members

        protected static Dictionary<string, string> _escapeCharacters;
        protected static string _decodePattern;

        #endregion

        #region Constructor

        static Ts3Util()
        {
            _escapeCharacters = new Dictionary<string, string>
            {
                { "\\", @"\\" }, { "/", @"\/" },  { " ", @"\s" },  { "|", @"\p" },  { "\a", @"\a" }, { "\b", @"\b" },
                { "\f", @"\f" }, { "\n", @"\n" }, { "\r", @"\r" }, { "\t", @"\t" }, { "\v", @"\v" }
            };

            _decodePattern = string.Join("|", _escapeCharacters.Select(x => string.Format("({0})", Regex.Escape(x.Value))).ToArray());
        }

        #endregion

        #region Public Methods

        public static string EncodeString(string value)
        {
            if (value == null)
                return null;

            if (value.Length == 0)
                return value;

            StringBuilder result = new StringBuilder(value);

            foreach (KeyValuePair<string, string> escapeCharacter in _escapeCharacters)
            {
                result.Replace(escapeCharacter.Key, escapeCharacter.Value);
            }

            return result.ToString();
        }

        public static string DecodeString(string value)
        {
            if (value == null)
                return null;

            if (value.Length == 0)
                return value;

            return Regex.Replace(value, _decodePattern,ReplaceEncodedValue , RegexOptions.Singleline);
        }

        private static string ReplaceEncodedValue(Match match)
        {
            return _escapeCharacters.First(kvp => kvp.Value == match.Value).Key;
        }

        #endregion
    }
}