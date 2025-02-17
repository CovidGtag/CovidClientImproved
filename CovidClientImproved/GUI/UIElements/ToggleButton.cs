using CovidClientImproved.CC.Input;
using CovidClientImproved.GUI.Logic;
using CovidClientImproved.Utils;
using System;
using System.Text;

namespace CovidClientImproved.GUI.UIElements
{
    public class ToggleButton : UIElement
    {
        private bool _state;
        public bool State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    UnityEngine.PlayerPrefs.SetInt(GetKey(), (value ? 1 : 0));
                    _state = value;
                    RaiseStateChanged(_state);

                    if (IsSinglePressMode && value)
                    {
                        State = false;
                    }
                }
            }
        }

        public bool IsSinglePressMode { get; set; }
        public bool RenderState { get; set; } = true;

        public Action EnabledState;

        public ToggleButton(UILogic parent)
        {
            Parent = parent;
        }

        private void RaiseStateChanged(bool value)
        {
            Callback?.Invoke(value);
        }

        private string GetKey()
        {
            return $"Button_{ModName}";
        }

        public override void HandleInput()
        {
            if (InputValue.LeftPrimary && Cooldown.CheckCooldown("ToggleState", 1f))
            {
                State = !State;
                Parent?.Draw();
            }
        }

        public override void Draw(StringBuilder builder, bool isSelected)
        {
            var prefix = isSelected ? "--> " : "   ";
            builder.AppendLine($"{prefix}{ModName.ToUpper()}{(RenderState ? (State ? ": ON" : ": OFF") : "")}");
        }

        public override void Initialize(Page page)
        {
            Type = ItemType.Button;
            bool state = UnityEngine.PlayerPrefs.GetInt(GetKey(), 0) == 0 ? false : true;
            State = state;
            RaiseStateChanged(state);
        }
    }
}
