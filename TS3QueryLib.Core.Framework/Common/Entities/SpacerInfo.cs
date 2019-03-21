using System;
using System.Text.RegularExpressions;
using TS3QueryLib.Core.Server.Entities;

namespace TS3QueryLib.Core.Common.Entities
{
    public class SpacerInfo : IDump
    {
        #region Properties

        public SpacerAlignment Alignment { get; private set; }
        public string Name { get; private set; }
        public string VisibleName { get; private set; }

        #endregion

        #region Constructor

        private SpacerInfo()
        {

        }

        #endregion

        #region Public Methods

        public static SpacerInfo Parse(string channelName)
        {
            if (channelName.IsNullOrTrimmedEmpty())
                return null;

            const string pattern = @"\[(?<Alignment>r|l|c|\\*)spacer.*?\](?<VisibleName>.*)";
            Match match = Regex.Match(channelName, pattern, RegexOptions.Singleline);

            if (!match.Success)
                return null;

            char alignmentChar = match.Groups["Alignment"].Length == 0 ? ' ' : match.Groups["Alignment"].Value[0];
            SpacerAlignment alignment;
            switch (alignmentChar)
            {
                case 'c': alignment = SpacerAlignment.Center;
                    break;
                case 'r': alignment = SpacerAlignment.Right;
                    break;
                case 'l': alignment = SpacerAlignment.Left;
                    break;
                default: alignment = SpacerAlignment.Repeat;
                    break;
            }

            string visibleName = match.Groups["VisibleName"].Value.Trim();

            if (visibleName.IsNullOrTrimmedEmpty())
                visibleName = channelName.Trim();

            return new SpacerInfo{Alignment = alignment, Name = channelName.Trim(), VisibleName = visibleName};
        }

        #endregion
    }
}