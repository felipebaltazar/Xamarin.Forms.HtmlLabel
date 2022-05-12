using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Forms.HtmlLabel
{
    public class HtmlNodeCollection : List<HtmlNode>
    {
        private readonly HtmlElementNode ParentNode;

        public HtmlNodeCollection(HtmlElementNode parentNode)
        {
            ParentNode = parentNode;
        }

        #region Add/remove nodes

        public T Add<T>(T node) where T : HtmlNode
        {
            if (node is null)
                return node;

            if (Count > 0)
            {
                HtmlNode lastNode = this[Count - 1];

                if (node.GetType() == typeof(HtmlTextNode) && lastNode.GetType() == typeof(HtmlTextNode))
                {
                    lastNode.InnerHtml += node.InnerHtml;
                    return lastNode as T;
                }
                else
                {
                    lastNode.NextNode = node;
                    node.PrevNode = lastNode;
                }
            }
            else node.PrevNode = null;
            node.NextNode = null;
            node.ParentNode = ParentNode;

            base.Add(node);

            return node;
        }

        public new void AddRange(IEnumerable<HtmlNode> nodes)
        {
            if (IsNullOrEmpty(nodes))
                return;

            foreach (HtmlNode node in nodes)
                Add(node);
        }

        #endregion

        private bool IsNullOrEmpty(IEnumerable<HtmlNode> nodes)
        {
            if (nodes is null)
                return true;

            return !nodes.Any();
        }
    }
}
