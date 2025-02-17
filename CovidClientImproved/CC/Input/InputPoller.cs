using UnityEngine;
using UnityEngine.XR;

namespace CovidClientImproved.CC.Input
{
    public class InputPoller
    {
        public enum XRHand
        {
            Left,
            Right
        }

        public static bool GripButtonDown(XRHand hand)
        {
            InputBool(hand, CommonUsages.gripButton, out bool result);
            return result;
        }

        public static bool GripButtonUp(XRHand hand)
        {
            InputBool(hand, CommonUsages.gripButton, out bool result);
            return !result;
        }

        public static bool PrimaryButtonDown(XRHand hand)
        {
            InputBool(hand, CommonUsages.primaryButton, out bool result);
            return result;
        }

        public static bool SecondaryButtonDown(XRHand hand)
        {
            InputBool(hand, CommonUsages.secondaryButton, out bool result);
            return result;
        }

        public static bool ThumbStickDown(XRHand hand)
        {
            InputBool(hand, CommonUsages.primary2DAxisClick, out bool result);
            return result;
        }

        public static bool MenuButtonDown(XRHand hand)
        {
            InputBool(hand, CommonUsages.menuButton, out bool result);
            return result;
        }

        public static Vector2 ThumbStick2DAxis(XRHand hand)
        {
            InputVector2(hand, CommonUsages.primary2DAxis, out Vector2 value);
            return value;
        }

        public static bool TriggerButtonDown(XRHand hand)
        {
            InputBool(hand, CommonUsages.triggerButton, out bool result);
            return result;
        }

        public static bool TriggerButtonUp(XRHand hand)
        {
            InputBool(hand, CommonUsages.triggerButton, out bool result);
            return !result;
        }

        public static float GripButtonFloat(XRHand hand)
        {
            InputFloat(hand, CommonUsages.grip, out float value);
            return value;
        }

        public static float TriggerButtonFloat(XRHand hand)
        {
            InputFloat(hand, CommonUsages.trigger, out float value);
            return value;
        }

        private static bool InputBool(XRHand hand, InputFeatureUsage<bool> usage, out bool value)
        {
            if (hand == XRHand.Left)
            {
                InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(usage, out value);
                return value;
            }
            else
            {
                InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(usage, out value);
                return value;
            }
        }

        static float InputFloat(XRHand hand, InputFeatureUsage<float> usage, out float value)
        {
            if (hand == XRHand.Left)
            {
                InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(usage, out value);
                return value;
            }
            else
            {
                InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(usage, out value);
                return value;
            }
        }

        static Vector2 InputVector2(XRHand hand, InputFeatureUsage<Vector2> usage, out Vector2 value)
        {
            if (hand == XRHand.Left)
            {
                InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(usage, out value);
                return value;
            }
            else
            {
                InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(usage, out value);
                return value;
            }
        }

        static Vector3 InputVector3(XRHand hand, InputFeatureUsage<Vector3> usage, out Vector3 value)
        {
            if (hand == XRHand.Left)
            {
                InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(usage, out value);
                return value;
            }
            else
            {
                InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(usage, out value);
                return value;
            }
        }
    }
}
