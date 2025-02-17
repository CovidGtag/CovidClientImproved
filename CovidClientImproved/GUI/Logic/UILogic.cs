using CovidClientImproved.GUI.UIElements;
using CovidClientImproved.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CovidClientImproved.GUI.Logic
{
    public class UILogic
    {
        private readonly Dictionary<int, Page> _pages = new Dictionary<int, Page>();
        private readonly Dictionary<string, PageType> _pageTypes = new Dictionary<string, PageType>();
        
        public int CurrentPageId { get; private set; }
        public Page CurrentPage => CurrentPageId >= 0 && _pages.ContainsKey(CurrentPageId)
            ? _pages[CurrentPageId]
            : null;

        public Canvas renderer; // Custom made canvas system

        public IReadOnlyDictionary<int, Page> Pages => _pages.ToDictionary(x => x.Key, x => x.Value);

        public int LastPageId { get; private set; } = -1;

        public UILogic()
        {
            renderer = new Canvas(Canvas.AnchorPoint.TopRight);
        }

        public void RegisterPageType(PageType pageType)
        {
            _pageTypes[pageType.Type] = pageType;
        }

        public void RegisterPageTypes(List<PageType> pageTypes)
        {
            foreach (var pageType in pageTypes)
            {
                _pageTypes[pageType.Type] = pageType;
            }
        }

        public void AddPages(IEnumerable<Page> pages)
        {
            if (pages == null)
                throw new ArgumentNullException(nameof(pages));

            foreach (var page in pages)
            {
                AddPage(page);
            }

            Draw();
        }

        public int AddPage(Page page)
        {
            if (!_pages.ContainsValue(page))
            {
                int newId = _pages.Any() ? _pages.Max(kvp => kvp.Key) + 1 : 0;
                _pages[newId] = page;

                if (_pageTypes.TryGetValue(page.PageType.Type, out var pageType))
                {
                    foreach (var factory in pageType.ElementFactories)
                    {
                        var element = factory(page);
                        page.AddElement(element);
                    }
                }

                if (page.PageId == 0)
                    NavigateToPage(0);

                return newId;
            }
            return -1;
        }

        public void HandleStateMethods()
        {
            if (CurrentPage == null || CurrentPage.Elements.Count == 0)
                return;

            foreach (var element in CurrentPage.Elements)
            {
                if (element.Type == ItemType.Button)
                {
                    var toggleButton = element as ToggleButton;
                    if (toggleButton != null && toggleButton.State)
                    {
                        toggleButton.EnabledState?.Invoke();
                    }
                }
            }
        }

        public void HandleInput()
        {
            if (CurrentPage == null || CurrentPage.Elements.Count == 0)
                return;

            var currentIndex = CurrentPage.SelectionIndex;
            if (currentIndex >= 0 && currentIndex < CurrentPage.Elements.Count)
            {
                CurrentPage.Elements[currentIndex].HandleInput();
            }
        }

        public void ChangePage(bool goToLastPage)
        {
            if (_pages.Count <= 1 || CurrentPage == null)
                return;
            
            string currentPageType = CurrentPage.PageType.Type;
            var validPages = _pages.Where(kvp =>
                kvp.Value.PageType.Type == currentPageType)
                .ToList();
            
            if (!validPages.Any())
                return;
            
            validPages.Sort((x, y) => x.Key.CompareTo(y.Key));

            int currentIndex = validPages.FindIndex(p => p.Key == CurrentPageId);
            
            int nextIndex = goToLastPage
                ? (currentIndex - 1 + validPages.Count) % validPages.Count
                : (currentIndex + 1) % validPages.Count;
            
            NavigateToPage(validPages[nextIndex].Key);
        }

        public void NavigateToPage(int pageId)
        {
            if (!Cooldown.CheckCooldown("ChangePage", 1f))
            {
                return;
            }

            if (_pages.ContainsKey(pageId))
            {
                LastPageId = CurrentPageId;
                CurrentPageId = pageId;
                Draw();
            }
            else
            {
                NotificationSystem.Instance.CreateNotification($"PAGE {pageId} NOT FOUND", UnityEngine.Color.white);
            }
        }

        public bool NavigateBack()
        {
            if (LastPageId != -1 && LastPageId != CurrentPageId)
            {
                NavigateToPage(LastPageId);
                return true;
            }
            return false;
        }

        public void ChangeSelectionIndex(bool moveUp)
        {
            if (CurrentPage == null || CurrentPage.Elements.Count == 0)
                return;

            int newIndex = moveUp
                ? (CurrentPage.SelectionIndex - 1 + CurrentPage.Elements.Count) % CurrentPage.Elements.Count
                : (CurrentPage.SelectionIndex + 1) % CurrentPage.Elements.Count;

            CurrentPage.SelectionIndex = newIndex;
            Draw();
        }

        public void ToggleCurrentItem()
        {
            if (CurrentPage == null)
                return;

            var currentIndex = CurrentPage.SelectionIndex;
            if (currentIndex >= 0 && currentIndex < CurrentPage.Elements.Count &&
                Cooldown.CheckCooldown("ToggleItem", 0.25f))
            {
                var element = CurrentPage.Elements[currentIndex];
                if (element is ToggleButton button)
                {
                    button.State = !button.State;
                }
            }
        }

        public void ToggleItem(string itemName, bool newState = false)
        {
            if (_pages == null || _pages.Count == 0)
                return;
            
            var buttonsToToggle = new List<ToggleButton>();

            foreach (var page in _pages.Values)
            {
                var matchingButtons = page.Elements
                    .Where(e => e is ToggleButton button &&
                              button.ModName.Equals(itemName, StringComparison.OrdinalIgnoreCase))
                    .Cast<ToggleButton>()
                    .ToList();

                buttonsToToggle.AddRange(matchingButtons);
            }

            foreach (var button in buttonsToToggle)
            {
                button.State = newState;
            }

            Draw();
        }

        public void Draw()
        {
            if (CurrentPage == null || CurrentPage.Elements.Count == 0)
                return;

            var builder = new StringBuilder();

            var pageTitle = CurrentPage.PageName;
            builder.AppendLine($"   [CC] {pageTitle.ToUpper()}\n");

            for (int i = 0; i < CurrentPage.Elements.Count; i++)
            {
                var element = CurrentPage.Elements[i];
                var isSelected = i == CurrentPage.SelectionIndex;

                element.Draw(builder, isSelected);
            }

            renderer.UpdateText(builder.ToString());
        }
    }
}
