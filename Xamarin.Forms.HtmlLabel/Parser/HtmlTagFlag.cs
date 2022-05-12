using System;

namespace Xamarin.Forms.HtmlLabel
{
    [Flags]
    public enum HtmlTagFlag
    {
        None = 0x0000,
        HtmlHeader = 0x0001,
        XmlHeader = 0x0002,
        NoChildren = 0x0004,
        NoNested = 0x0008,
        NoSelfClosing = 0x0010,
        CData = 0x0020,
    }
}
