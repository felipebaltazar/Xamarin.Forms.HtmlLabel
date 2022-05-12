using System;
using System.Collections.Generic;

namespace Xamarin.Forms.HtmlLabel
{
    public static class HtmlRules
    {
        #region Constant values

        public const char TAG_START = '<';
        public const char TAG_END = '>';
        public const char FORWARD_SLASH = '/';

        public const char DOUBLE_QUOTE = '"';
        public const char SINGLE_QUOTE = '\'';

        public const StringComparison TagStringComparison = StringComparison.CurrentCultureIgnoreCase;
        public static readonly StringComparer TagStringComparer = StringComparer.CurrentCultureIgnoreCase;

        #endregion

        #region String and character classification

        public static bool IsQuoteChar(char c) => c == DOUBLE_QUOTE || c == SINGLE_QUOTE;

        private static readonly HashSet<char> InvalidChars;

        static HtmlRules()
        {
            InvalidChars = new HashSet<char>
            {
                '!',
                '?',
                '<',
                '"',
                '\'',
                '>',
                '/',
                '='
            };

            for (int i = 0xfdd0; i <= 0xfdef; i++)
                InvalidChars.Add((char)i);

            InvalidChars.Add('\ufffe');
            InvalidChars.Add('\uffff');
        }

        public static bool IsTagCharacter(char c) =>
            !InvalidChars.Contains(c) && !char.IsControl(c) && !char.IsWhiteSpace(c);

        public static bool IsAttributeNameCharacter(char c) =>
            !InvalidChars.Contains(c) && !char.IsControl(c) && !char.IsWhiteSpace(c);

        public static bool IsAttributeValueCharacter(char c) =>
            !InvalidChars.Contains(c) && !char.IsControl(c) && !char.IsWhiteSpace(c);

        #endregion

        #region Tag classification

        private static readonly Dictionary<string, HtmlTagFlag> TagRules = new Dictionary<string, HtmlTagFlag>(StringComparer.CurrentCultureIgnoreCase)
        {
            ["!doctype"] = HtmlTagFlag.HtmlHeader,
            ["?xml"] = HtmlTagFlag.XmlHeader,
            ["a"] = HtmlTagFlag.NoNested,
            ["area"] = HtmlTagFlag.NoChildren,
            ["base"] = HtmlTagFlag.NoChildren,
            ["basefont"] = HtmlTagFlag.NoChildren,
            ["bgsound"] = HtmlTagFlag.NoChildren,
            ["br"] = HtmlTagFlag.NoChildren,
            ["col"] = HtmlTagFlag.NoChildren,
            ["dd"] = HtmlTagFlag.NoNested,
            ["dt"] = HtmlTagFlag.NoNested,
            ["embed"] = HtmlTagFlag.NoChildren,
            ["frame"] = HtmlTagFlag.NoChildren,
            ["hr"] = HtmlTagFlag.NoChildren,
            ["img"] = HtmlTagFlag.NoChildren,
            ["input"] = HtmlTagFlag.NoChildren,
            ["isindex"] = HtmlTagFlag.NoChildren,
            ["keygen"] = HtmlTagFlag.NoChildren,
            ["li"] = HtmlTagFlag.NoNested,
            ["link"] = HtmlTagFlag.NoChildren,
            ["menuitem"] = HtmlTagFlag.NoChildren,
            ["meta"] = HtmlTagFlag.NoChildren,
            ["noxhtml"] = HtmlTagFlag.CData,
            ["p"] = HtmlTagFlag.NoNested,
            ["param"] = HtmlTagFlag.NoChildren,
            ["script"] = HtmlTagFlag.CData,
            ["select"] = HtmlTagFlag.NoSelfClosing,
            ["source"] = HtmlTagFlag.NoChildren,
            ["spacer"] = HtmlTagFlag.NoChildren,
            ["style"] = HtmlTagFlag.CData,
            ["table"] = HtmlTagFlag.NoNested,
            ["td"] = HtmlTagFlag.NoNested,
            ["th"] = HtmlTagFlag.NoNested,
            ["textarea"] = HtmlTagFlag.NoSelfClosing,
            ["track"] = HtmlTagFlag.NoChildren,
            ["wbr"] = HtmlTagFlag.NoChildren,
        };

        public static HtmlTagFlag GetTagFlags(string tag) => TagRules.TryGetValue(tag, out HtmlTagFlag flags) ? flags : HtmlTagFlag.None;

        private static readonly Dictionary<string, int> NestLevelLookup = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase)
        {
            ["div"] = 150,
            ["td"] = 160,
            ["th"] = 160,
            ["tr"] = 170,
            ["thead"] = 180,
            ["tbody"] = 180,
            ["tfoot"] = 180,
            ["table"] = 190,
            ["head"] = 200,
            ["body"] = 200,
            ["html"] = 220,
        };

        public static int GetTagNestLevel(string tag) => NestLevelLookup.TryGetValue(tag, out int priority) ? priority : 100;

        #endregion

        #region Tag nesting rules logic

        public static bool TagMayContain(string parentTag, string childTag) =>
            TagMayContain(parentTag, childTag, GetTagFlags(parentTag));

        public static bool TagMayContain(string parentTag, string childTag, HtmlTagFlag parentFlags)
        {
            if (parentFlags.HasFlag(HtmlTagFlag.NoChildren))
                return false;
            if (parentFlags.HasFlag(HtmlTagFlag.NoNested) && parentTag.Equals(childTag, TagStringComparison))
                return false;

            if (GetTagNestLevel(childTag) > GetTagNestLevel(parentTag))
                return false;
            return true;
        }

        #endregion
    }
}
