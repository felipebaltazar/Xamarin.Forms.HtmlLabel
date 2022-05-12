namespace Xamarin.Forms.HtmlLabel
{
    public class HtmlAttribute
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public HtmlAttribute()
        {
            Name = null;
            Value = null;
        }

        public override string ToString() => string.Format("{0}{1}",
            Name ?? "(null)",
            (Value != null) ? $"=\"{Value}\"" : string.Empty);
    }
}
