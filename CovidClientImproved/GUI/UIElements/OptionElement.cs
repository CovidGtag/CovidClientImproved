using CovidClientImproved.CC.Input;
using CovidClientImproved.GUI.Logic;
using CovidClientImproved.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace CovidClientImproved.GUI.UIElements
{
    public class OptionElement : UIElement
    {
        private readonly string[] Options;

        public Action<string> OptionSelected;

        public int ParentPageId;

        public OptionElement(UILogic parent, int parentPageId, params string[] options)
        {
            Parent = parent;
            Options = options ?? new string[0];
            ParentPageId = parentPageId;
        }

        public override void Draw(StringBuilder builder, bool isSelected)
        {
            var prefix = isSelected ? "--> " : "   ";
            builder.AppendLine($"{prefix}{ModName.ToUpper()}");
        }

        public override void HandleInput()
        {
            if (InputValue.LeftPrimary)
            {
                CreateDynamicElements();
            }
        }

        private void CreateDynamicElements()
        {
            List<UIElement> elements = new List<UIElement>();

            foreach (var option in Options)
            {
                var toggleButton = new ToggleButton(Parent)
                {
                    ModName = option,
                    IsSinglePressMode = true,
                    Callback = (state) =>
                    {
                        OptionSelected?.Invoke(option);
                        Parent.NavigateToPage(ParentPageId);
                    },
                    RenderState = false
                };
                elements.Add(toggleButton);
            }
            var pageType = new PageType("OPTION PAGE");
            var page = new Page
            {
                PageName = "FLY OPTIONS",
                PageId = 99,
                Elements = elements,
                PageType = pageType,
            };
            Parent.RegisterPageType(pageType);
            int pageId = Parent.AddPage(page);
            if (pageId == -1)
            {
                NotificationSystem.Instance.CreateNotification("FAILED TO ADD PAGE TO PAGE LIST", UnityEngine.Color.red);
                return;
            }
            Parent.NavigateToPage(pageId);
        }

        public override void Initialize(Page page)
        {
            Type = ItemType.Option;
        }
    }
}
