using System.Collections.Generic;

namespace Xamarin.Forms.HtmlLabel
{
    public class HtmlParser
    {
        private TextParser Parser;

        public HtmlDocument Parse(string html)
        {
            var document = new HtmlDocument();
            document.RootNodes.AddRange(ParseChildren(html));
            return document;
        }

        public IEnumerable<HtmlNode> ParseChildren(string html)
        {
            var rootNode = new HtmlElementNode("[TempContainer]");
            var parentNode = rootNode;
            Parser = new TextParser(html);

            bool selfClosing;
            string tag;

            while (!Parser.EndOfText)
            {
                if (Parser.Peek() == HtmlRules.TAG_START)
                {
                    if (Parser.Peek(1) == HtmlRules.FORWARD_SLASH)
                    {
                        Parser.Index += 2;
                        tag = Parser.ParseWhile(HtmlRules.IsTagCharacter);
                        if (tag.Length > 0)
                        {
                            if (parentNode.TagName.Equals(tag, HtmlRules.TagStringComparison))
                            {
                                parentNode = parentNode.ParentNode;
                            }
                            else
                            {
                                var tagPriority = HtmlRules.GetTagNestLevel(tag);
                                while (!parentNode.IsTopLevelNode && tagPriority > HtmlRules.GetTagNestLevel(parentNode.TagName))
                                    parentNode = parentNode.ParentNode;
                                if (parentNode.TagName.Equals(tag, HtmlRules.TagStringComparison))
                                {
                                    parentNode = parentNode.ParentNode;
                                }
                            }
                        }

                        Parser.SkipTo(HtmlRules.TAG_END);
                        Parser.Next();
                        continue;
                    }

                    if (ParseTag(out tag))
                    {
                        HtmlTagFlag flags = HtmlRules.GetTagFlags(tag);
                        if (flags.HasFlag(HtmlTagFlag.HtmlHeader))
                        {
                            parentNode.Children.Add(ParseHtmlHeader());
                        }
                        else if (flags.HasFlag(HtmlTagFlag.XmlHeader))
                        {
                        }
                        else
                        {
                            var attributes = ParseAttributes();

                            if (Parser.Peek() == HtmlRules.FORWARD_SLASH)
                            {
                                Parser.Next();
                                Parser.SkipWhiteSpace();
                                selfClosing = true;
                            }
                            else
                            {
                                selfClosing = false;
                            }

                            Parser.SkipTo(HtmlRules.TAG_END);
                            Parser.Next();

                            var node = new HtmlElementNode(tag, attributes);
                            while (!HtmlRules.TagMayContain(parentNode.TagName, tag) && !parentNode.IsTopLevelNode)
                            {
                                parentNode = parentNode.ParentNode;
                            }

                            parentNode.Children.Add(node);

                            if (selfClosing && flags.HasFlag(HtmlTagFlag.NoSelfClosing))
                                selfClosing = false;
                            if (!selfClosing && !flags.HasFlag(HtmlTagFlag.NoChildren))
                                parentNode = node;
                        }
                        continue;
                    }
                }

                string text = Parser.ParseCharacter();
                text += Parser.ParseTo(HtmlRules.TAG_START);
                parentNode.Children.Add(new HtmlTextNode(text));
            }

            return rootNode.Children;
        }

        private bool ParseTag(out string tag)
        {
            tag = null;
            int pos = 0;

            char c = Parser.Peek(++pos);
            if (c == '!' || c == '?')
                c = Parser.Peek(++pos);

            if (HtmlRules.IsTagCharacter(c))
            {
                while (HtmlRules.IsTagCharacter(Parser.Peek(++pos))) ;

                Parser.Next();

                var length = pos - 1;
                tag = Parser.Text.Substring(Parser.Index, length);

                Parser.Index += length;
                return true;
            }

            return false;
        }

        private HtmlAttributeCollection ParseAttributes()
        {
            var attributes = new HtmlAttributeCollection();

            Parser.SkipWhiteSpace();
            var ch = Parser.Peek();

            while (HtmlRules.IsAttributeNameCharacter(ch) || HtmlRules.IsQuoteChar(ch))
            {
                var attribute = new HtmlAttribute();
                if (HtmlRules.IsQuoteChar(ch))
                    attribute.Name = $"\"{Parser.ParseQuotedText()}\"";
                else
                    attribute.Name = Parser.ParseWhile(HtmlRules.IsAttributeNameCharacter);

                Parser.SkipWhiteSpace();
                if (Parser.Peek() == '=')
                {
                    Parser.Next();
                    Parser.SkipWhiteSpace();
                    if (HtmlRules.IsQuoteChar(Parser.Peek()))
                    {
                        attribute.Value = Parser.ParseQuotedText();
                    }
                    else
                    {
                        attribute.Value = Parser.ParseWhile(HtmlRules.IsAttributeValueCharacter);
                    }
                }
                else
                {
                    attribute.Value = null;
                }

                attributes.Add(attribute);
                Parser.SkipWhiteSpace();

                ch = Parser.Peek();
            }

            return attributes;
        }

        private HtmlHeaderNode ParseHtmlHeader()
        {
            var node = new HtmlHeaderNode(ParseAttributes());
            Parser.SkipTo(HtmlRules.TAG_END);
            Parser.Index++;

            return node;
        }
    }
}
