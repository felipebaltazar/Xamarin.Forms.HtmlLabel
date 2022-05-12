using System.Net;

namespace Xamarin.Forms.HtmlLabel
{
    public sealed class HtmlTextNode : HtmlNode
    {
        private string content;

        public HtmlTextNode(string html = null)
        {
            content = html ?? string.Empty;
        }

        public override string InnerHtml
        {
            get => content;
            set => content = value;
        }

        public override string OuterHtml
        {
            get => InnerHtml;
        }

        public override string Text
        {
            get => WebUtility.HtmlDecode(content);
            set => content = WebUtility.HtmlEncode(value);
        }
    }

}
