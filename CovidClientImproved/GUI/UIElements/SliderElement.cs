using CovidClientImproved.CC.Input;
using CovidClientImproved.GUI.Logic;
using System;
using System.Text;

namespace CovidClientImproved.GUI.UIElements
{
    public class SliderElement : UIElement
    {
        private float _value;
        private float _targetValue;
        private float _smoothSpeed;

        public float MinValue;
        public float MaxValue;

        public SliderElement(string modName, float minValue, float maxValue, UILogic parent)
        {
            ModName = modName;
            MinValue = minValue;
            MaxValue = maxValue;
            _value = UnityEngine.PlayerPrefs.GetFloat(GetKey(), minValue);
            _targetValue = _value;
            Parent = parent ?? throw new ArgumentNullException(nameof(parent), "Parent is required in constructor");
            Type = ItemType.Slider;
        }

        private string GetKey()
        {
            return $"Slider_{ModName}";
        }

        public override void HandleInput()
        {
            if (UnityEngine.Mathf.Abs(InputValue.LeftThumbstickAxis.x) >= 0f)
            {
                _targetValue = InputValue.LeftThumbstickAxis.x > 0 ? MaxValue : MinValue;

                float magnitude = UnityEngine.Mathf.Clamp01(UnityEngine.Mathf.Abs(InputValue.LeftThumbstickAxis.x));
                _smoothSpeed = (MaxValue / 4) * magnitude;

                _value = UnityEngine.Mathf.MoveTowards(_value, _targetValue, _smoothSpeed * UnityEngine.Time.deltaTime);

                if (_value != UnityEngine.PlayerPrefs.GetFloat(GetKey()))
                {
                    UnityEngine.PlayerPrefs.SetFloat(GetKey(), _value);
                    UnityEngine.PlayerPrefs.Save();
                }

                Callback?.Invoke(_value);
            }
            Parent?.Draw();
        }

        public override void Draw(StringBuilder builder, bool isSelected)
        {
            var prefix = isSelected ? "--> " : "   ";
            builder.AppendLine($"{prefix}{ModName.ToUpper()}: {_value:F2}/{MaxValue:F2}");
        }

        public override void Initialize(Page page)
        {
            Type = ItemType.Slider;
            _value = UnityEngine.PlayerPrefs.GetFloat(GetKey(), MinValue);
            Callback?.Invoke(_value);
        }
    }
}
