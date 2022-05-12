using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Xamarin.Essentials;

namespace Xamarin.Forms.HtmlLabel
{
    public partial class HtmlFormattedLabel : Label
    {
        private readonly HtmlParser _parser = new HtmlParser();

        public new static readonly BindableProperty TextProperty = BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(HtmlFormattedLabel),
            string.Empty,
            BindingMode.OneWay,
            propertyChanged: OnHtmlTextChanged);

        public static readonly BindableProperty BoldStyleProperty = BindableProperty.Create(
            nameof(BoldStyle),
            typeof(Style),
            typeof(HtmlFormattedLabel),
            null,
            BindingMode.OneWay);

        public static readonly BindableProperty ItalicStyleProperty = BindableProperty.Create(
            nameof(ItalicStyle),
            typeof(Style),
            typeof(HtmlFormattedLabel),
            null,
            BindingMode.OneWay);

        public new static readonly BindableProperty StyleProperty = BindableProperty.Create(
            nameof(Style),
            typeof(Style),
            typeof(HtmlFormattedLabel),
            null,
            BindingMode.OneWay);

        private static void OnHtmlTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is HtmlFormattedLabel htmlLabel)
            {
                var formatedString = new FormattedString();
                if (newValue is string htmlText)
                {
                    var formattedHtmlText = SpanFormat(htmlText);
                    formatedString = htmlLabel.BuildFormatedTextFromHtml(formattedHtmlText);
                }

                htmlLabel.FormattedText = formatedString;
            }
        }

        private static string SpanFormat(string htmlText)
        {
            if (htmlText.StartsWith($"<span>"))
                return htmlText;

            return $"<span>{htmlText}</span>";
        }

        public new string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Style BoldStyle
        {
            get => (Style)GetValue(BoldStyleProperty);
            set => SetValue(BoldStyleProperty, value);
        }

        public Style ItalicStyle
        {
            get => (Style)GetValue(ItalicStyleProperty);
            set => SetValue(ItalicStyleProperty, value);
        }

        public new Style Style
        {
            get => (Style)GetValue(StyleProperty);
            set => SetValue(StyleProperty, value);
        }

        private FormattedString BuildFormatedTextFromHtml(string htmlText)
        {
            var formattedText = new FormattedString();
            if (string.IsNullOrWhiteSpace(htmlText)) return formattedText;

            try
            {

                //Tags suportadas:
                //<p>, <span>, <b>, <i>, <br/>
                var decodedPolicy = $"<html>{WebUtility.HtmlDecode(htmlText.Replace("<br>", "<br/>").Replace("</br>", "<br/>"))}</html>";
                var xmlDoc = _parser.Parse(decodedPolicy);

                var rootNode = xmlDoc.RootNodes.First() as HtmlElementNode;

                foreach (HtmlElementNode node in rootNode.Children)
                    ParseElement(node, formattedText);
                return formattedText;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return formattedText;
        }

        private IEnumerable<HtmlElementNode> GetInnerElements(HtmlElementNode element)
        {
            return element?.Children?.OfType<HtmlElementNode>()?.Where(e =>
                 "b".Equals(e.TagName, StringComparison.OrdinalIgnoreCase)
                 || "i".Equals(e.TagName, StringComparison.OrdinalIgnoreCase)
                 || "br".Equals(e.TagName, StringComparison.OrdinalIgnoreCase)
                 || "a".Equals(e.TagName, StringComparison.OrdinalIgnoreCase));
        }

        private void ApplyTextFormat(string name, Span span, HtmlElementNode node)
        {
            switch (name.ToUpperInvariant())
            {
                case "SPAN":
                    break;
                case "B":
                    span.FontAttributes = span.FontAttributes == FontAttributes.Italic
                        ? FontAttributes.Italic | FontAttributes.Bold
                        : FontAttributes.Bold;
                    span.Style = BoldStyle ?? Style;
                    break;
                case "I":
                    span.FontAttributes = span.FontAttributes == FontAttributes.Bold
                        ? FontAttributes.Bold | FontAttributes.Italic
                        : FontAttributes.Italic;
                    span.Style = ItalicStyle ?? Style;
                    break;
                case "P":
                    span.Text = $"\t{span.Text}";
                    break;
                case "BR":
                    span.Text = $"\n{span.Text}";
                    break;
                case "A":
                    var attr = node.Attributes.FirstOrDefault(a => "href".Equals(a.Name, StringComparison.OrdinalIgnoreCase));
                    var url = attr?.Value ?? string.Empty;
                    span.TextColor = Color.Blue;
                    span.TextDecorations = TextDecorations.Underline;
                    span.GestureRecognizers.Add(new TapGestureRecognizer()
                    {
                        Command = new Command(async () => await Browser.OpenAsync(url))
                    });
                    break;
                default:
                    return;
            }
        }

        private void ParseElement(HtmlElementNode element, FormattedString formattedString)
        {
            if (string.IsNullOrWhiteSpace(element.InnerHtml))
                return;

            var text = element.InnerHtml;
            var name = element.TagName;
            var innerElements = GetInnerElements(element);

            var span = new Span
            {
                Style = Style,
                FontSize = FontSize
            };

            ApplyTextFormat(name, span, element);

            if (innerElements is null || !innerElements.Any())
            {
                span.Text = $"{span.Text}{text}";
                formattedString.Spans.Add(span);
                return;
            }

            ParseChildren(text, innerElements, formattedString, span);
        }

        private void ParseChildren(string elementText, IEnumerable<HtmlElementNode> innerElements, FormattedString formattedString, Span span)
        {
            //tratar os innerElements
            var processingString = elementText;
            foreach (var innerElement in innerElements)
            {
                var elementStartIndex = processingString.IndexOf(innerElement.OuterHtml, StringComparison.InvariantCultureIgnoreCase);
                var elementEndIndex = elementStartIndex + innerElement.OuterHtml.Length;

                var children = GetInnerElements(innerElement);
                var childrenFormattedString = new FormattedString();

                //caso haja texto no início, adiciona
                if (elementStartIndex > 0)
                    formattedString.Spans.Add(new Span
                    {
                        Style = Style,
                        FontSize = FontSize,
                        Text = processingString.Substring(0, elementStartIndex)
                    });

                //caso haja filhos, adiciona o texto formatado dos filhos
                if (children != null && children.Any())
                {
                    ParseChildren(innerElement.InnerHtml, children, childrenFormattedString, span);

                    foreach (var childSpan in childrenFormattedString.Spans)
                        formattedString.Spans.Add(childSpan);
                }
                else
                {
                    span.Text = innerElement.InnerHtml;
                    ApplyTextFormat(innerElement.TagName, span, innerElement);
                    formattedString.Spans.Add(span);
                    span = new Span
                    {
                        Style = Style,
                        FontSize = FontSize
                    };
                }

                //remove as partes já processadas
                processingString = processingString.Substring(elementEndIndex, processingString.Length - elementEndIndex);
            }

            if (!string.IsNullOrWhiteSpace(processingString))
            {
                //caso haja texto no final
                formattedString.Spans.Add(new Span
                {
                    Style = Style,
                    FontSize = FontSize,
                    Text = processingString
                });
            }
        }

    }
}
