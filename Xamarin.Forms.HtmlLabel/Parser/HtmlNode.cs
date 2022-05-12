namespace Xamarin.Forms.HtmlLabel
{
    public abstract class HtmlNode
    {
        public HtmlElementNode ParentNode { get; internal set; }

        public HtmlNode NextNode { get; internal set; }

        public HtmlNode PrevNode { get; internal set; }

        public bool IsTopLevelNode => ParentNode == null;

        public virtual string InnerHtml
        {
            get => string.Empty;
            set { }
        }

        public virtual string OuterHtml
        {
            get => string.Empty;
        }

        public virtual string Text
        {
            get => string.Empty;
            set { }
        }
    }
}
