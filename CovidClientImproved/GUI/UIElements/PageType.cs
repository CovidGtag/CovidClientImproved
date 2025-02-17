using System;
using System.Collections.Generic;

namespace CovidClientImproved.GUI.UIElements
{
    public class PageType
    {
        public string Type { get; }
        public List<Func<Page, UIElement>> ElementFactories { get; } = new List<Func<Page, UIElement>>();

        public PageType(string type)
        {
            Type = type;
        }

        public void AddElementFactory(Func<Page, UIElement> factory)
        {
            ElementFactories.Add(factory);
        }
    }
}
