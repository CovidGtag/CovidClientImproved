using CovidClientImproved.GUI.Logic;
using System;
using System.Text;

namespace CovidClientImproved.GUI.UIElements
{
    /// <summary>
    /// This class contains definitions for UIElements
    /// </summary>
    public abstract class UIElement
    {
        public string ModName { get; set; }
        public ItemType Type { get; protected set; }
        public Action<object> Callback;
        public UILogic Parent;
        public abstract void HandleInput();
        public abstract void Draw(StringBuilder builder, bool isSelected);
        public abstract void Initialize(Page page);
    }
}
