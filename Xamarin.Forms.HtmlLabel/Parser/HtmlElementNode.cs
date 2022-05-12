using System.Linq;
using System.Text;

namespace Xamarin.Forms.HtmlLabel
{
    public class HtmlElementNode : HtmlNode
    {
        public string TagName { get; set; }

        public HtmlAttributeCollection Attributes { get; private set; }

        public HtmlNodeCollection Children { get; private set; }

        public HtmlElementNode(string tagName, HtmlAttributeCollection attributes = null)
        {
            TagName = tagName ?? string.Empty;
            Attributes = attributes ?? new HtmlAttributeCollection();
            Children = new HtmlNodeCollection(this);
        }

        public bool IsSelfClosing => !Children.Any() && !HtmlRules.GetTagFlags(TagName).HasFlag(HtmlTagFlag.NoSelfClosing);

        public override string InnerHtml
        {
            get
            {
                if (!Children.Any())
                    return string.Empty;
                StringBuilder builder = new StringBuilder();
                foreach (var node in Children)
                    builder.Append(node.OuterHtml);
                return builder.ToString();
            }
            set
            {
                Children.Clear();
                if (!string.IsNullOrEmpty(value))
                {
                    var parser = new HtmlParser();
                    Children.AddRange(parser.ParseChildren(value));
                }
            }
        }

        public override string OuterHtml
        {
            get
            {
                var builder = new StringBuilder();

                builder.Append(HtmlRules.TAG_START);
                builder.Append(TagName);
                builder.Append(Attributes.ToString());

                if (IsSelfClosing)
                {
                    builder.Append(' ');
                    builder.Append(HtmlRules.FORWARD_SLASH);
                    builder.Append(HtmlRules.TAG_END);
                }
                else
                {
                    builder.Append(HtmlRules.TAG_END);
                    builder.Append(InnerHtml);
                    builder.Append(HtmlRules.TAG_START);
                    builder.Append(HtmlRules.FORWARD_SLASH);
                    builder.Append(TagName);
                    builder.Append(HtmlRules.TAG_END);
                }
                return builder.ToString();
            }
        }

        public override string Text
        {
            get
            {
                if (!Children.Any(c => c is HtmlTextNode))
                    return string.Empty;

                var builder = new StringBuilder();

                foreach (var node in Children.Where(c => c is HtmlTextNode))
                    builder.Append(node.Text);

                return builder.ToString();
            }
            set
            {
                Children.Clear();
                if (!string.IsNullOrEmpty(value))
                    Children.Add(new HtmlTextNode() { Text = value });
            }
        }

        public override string ToString() => $"<{TagName ?? "(null)"} />";
    }

}
