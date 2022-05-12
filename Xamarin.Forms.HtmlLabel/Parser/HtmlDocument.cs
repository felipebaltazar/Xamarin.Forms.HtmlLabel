using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms.HtmlLabel
{
    public class HtmlDocument
    {
        public string Path { get; private set; }

        public HtmlNodeCollection RootNodes { get; private set; }

        public HtmlDocument()
        {
            Path = null;
            RootNodes = new HtmlNodeCollection(null);
        }

        public static IEnumerable<HtmlNode> Find(IEnumerable<HtmlNode> nodes, Func<HtmlNode, bool> predicate)
        {
            if (nodes is null || predicate is null)
                yield break;

            foreach (var node in nodes)
            {
                if (predicate(node))
                    yield return node;

                if (node is HtmlElementNode elementNode)
                {
                    foreach (var childNode in Find(elementNode.Children, predicate))
                        yield return childNode;
                }
            }
        }
    }
}
