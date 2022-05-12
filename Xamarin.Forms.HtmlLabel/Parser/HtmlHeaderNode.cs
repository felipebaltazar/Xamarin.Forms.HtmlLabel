namespace Xamarin.Forms.HtmlLabel
{
    public class HtmlHeaderNode : HtmlNode
    {
        public HtmlAttributeCollection Attributes { get; private set; }

        public HtmlHeaderNode(HtmlAttributeCollection attributes)
        {
            Attributes = attributes;
        }
    }
}
