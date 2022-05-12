using System;
using System.Text;

namespace Xamarin.Forms.HtmlLabel
{
    public sealed class TextParser
    {
        private int internalIndex;

        public const char NullChar = '\0';

        public string Text { get; private set; }

        public TextParser(string text)
        {
            Reset(text);
        }

        public void Reset(string text)
        {
            Text = text ?? string.Empty;
            internalIndex = 0;
        }

        public int Index
        {
            get => internalIndex;
            set
            {
                internalIndex = value;
                if (internalIndex < 0)
                    internalIndex = 0;
                else if (internalIndex > Text.Length)
                    internalIndex = Text.Length;
            }
        }

        public bool EndOfText => (internalIndex >= Text.Length);

        public char Peek()
        {
            return (internalIndex < Text.Length) ? Text[internalIndex] : NullChar;
        }

        public char Peek(int count)
        {
            int index = (internalIndex + count);
            return (index >= 0 && index < Text.Length) ? Text[index] : NullChar;
        }

        public char Get()
        {
            if (internalIndex < Text.Length)
                return Text[internalIndex++];
            return NullChar;
        }

        public void Next()
        {
            if (internalIndex < Text.Length)
                internalIndex++;
        }

        public void SkipWhile(Func<char, bool> predicate)
        {
            if (predicate is null)
                return;

            while (internalIndex < Text.Length && predicate(Text[internalIndex]))
                internalIndex++;
        }

        public void SkipWhiteSpace() => SkipWhile(char.IsWhiteSpace);

        public bool SkipTo(params char[] chars)
        {
            internalIndex = Text.IndexOfAny(chars, internalIndex);
            if (internalIndex >= 0)
                return true;
            internalIndex = Text.Length;
            return false;
        }

        public bool SkipTo(string s, bool includeToken = false)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            internalIndex = Text.IndexOf(s, internalIndex, StringComparison.Ordinal);
            if (internalIndex >= 0)
            {
                if (includeToken)
                    internalIndex += s.Length;
                return true;
            }

            internalIndex = Text.Length;
            return false;
        }

        public bool SkipTo(string s, StringComparison comparison, bool includeToken = false)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            internalIndex = Text.IndexOf(s, internalIndex, comparison);
            if (internalIndex >= 0)
            {
                if (includeToken)
                    internalIndex += s.Length;
                return true;
            }
            internalIndex = Text.Length;
            return false;
        }

        public string ParseCharacter()
        {
            if (internalIndex < Text.Length)
                return Text[internalIndex++].ToString();
            return string.Empty;
        }

        public string ParseWhile(Func<char, bool> predicate)
        {
            int start = internalIndex;
            SkipWhile(predicate);
            return Extract(start, internalIndex);
        }

        public string ParseQuotedText()
        {
            var builder = new StringBuilder();
            var quote = Get();

            while (!EndOfText)
            {
                builder.Append(ParseTo(quote));
                Next();

                if (Peek() == quote)
                {
                    builder.Append(quote);
                    Next();
                }

                else break;
            }
            return builder.ToString();
        }

        public string ParseTo(params char[] chars)
        {
            int start = internalIndex;
            SkipTo(chars);
            return Extract(start, internalIndex);
        }

        public string Extract(int start, int end)
        {
            if (start < 0 || start > Text.Length)
                throw new ArgumentOutOfRangeException(nameof(start));
            if (end < start || end > Text.Length)
                throw new ArgumentOutOfRangeException(nameof(end));
            return Text.Substring(start, end - start);
        }
    }
}
