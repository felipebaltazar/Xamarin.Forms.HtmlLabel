using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Forms.HtmlLabel
{
    public class HtmlAttributeCollection : IEnumerable<HtmlAttribute>
    {
        private readonly Dictionary<string, HtmlAttribute> Attributes;

        public HtmlAttributeCollection()
        {
            Attributes = new Dictionary<string, HtmlAttribute>(HtmlRules.TagStringComparer);
        }

        public void Add(HtmlAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));
            if (string.IsNullOrEmpty(attribute.Name))
                throw new ArgumentException("Attribute name cannot be null or empty.");

            if (Attributes.TryGetValue(attribute.Name, out HtmlAttribute existingAttribute))
                existingAttribute.Value = attribute.Value;
            else
                Attributes.Add(attribute.Name, attribute);
        }

        public HtmlAttribute this[string name]
        {
            get => Attributes.TryGetValue(name, out HtmlAttribute value) ? value : null;
            set => Attributes[name] = value;
        }

        public int Count => Attributes.Count;

        #region IEnumerable

        public IEnumerator<HtmlAttribute> GetEnumerator()
        {
            return Attributes.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Attributes.Values.GetEnumerator();
        }

        #endregion

        public override string ToString() =>
            Attributes.Any() ? $" {string.Join(" ", this)}" : string.Empty;
    }
}
