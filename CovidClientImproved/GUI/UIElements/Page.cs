using System.Collections.Generic;

namespace CovidClientImproved.GUI.UIElements
{
    public class Page
    {
        public int PageId;
        public string PageName;
        public PageType PageType;
        public int SelectionIndex = 0;

        public List<UIElement> Elements { get; set; } = new List<UIElement>();

        public void AddElement(UIElement element)
        {
            if (element != null)
            {
                Elements.Add(element);
                element.Initialize(this);
            }
        }

        public void RemoveElement(UIElement element)
        {
            Elements.Remove(element);
        }
    }
}
