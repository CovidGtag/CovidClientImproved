using MelonLoader;
using CovidClientImproved.GUI.Logic;
using CovidClientImproved.CC.Input;
using CovidClientImproved.Utils;
using System.Collections.Generic;
using CovidClientImproved.CC.Mods;
using CovidClientImproved.GUI.UIElements;

namespace CovidClientImproved
{
    public class Main : MelonMod
    {
        public static bool navigation = true;
        public static UILogic logic = null;
        
        private UnityEngine.Vector2 currentPos = new UnityEngine.Vector2(0, 0);

        public override void OnInitializeMelon()
        {
            base.OnInitializeMelon();
            try
            {
                NotificationSystem.Instance.InitializeSharedCanvas();
                logic = new UILogic();
                var modPageType = new PageType("Mods");
                modPageType.AddElementFactory((page) => new ToggleButton(logic) {
                    ModName = "BACK",
                    Callback = (value) =>
                    {
                        if ((bool)value)
                        {
                            logic.ChangePage(true);
                        }
                    },
                    IsSinglePressMode = true,
                    RenderState = false
                });
                modPageType.AddElementFactory((page) => new ToggleButton(logic) { ModName = "FORWARDS",
                    Callback = (value) =>
                    {
                        if ((bool)value)
                        {
                            logic.ChangePage(false);
                        }
                    },
                    IsSinglePressMode = true,
                    RenderState = false
                });
                modPageType.AddElementFactory((page) => new ToggleButton(logic) {
                    ModName = "HOME",
                    Callback = (value) =>
                    {
                        if ((bool)value)
                        {
                            logic.NavigateToPage(0);
                        }
                    },
                    IsSinglePressMode = true,
                    RenderState = false});

                logic.RegisterPageType(modPageType);

                var pages = new List<Page> {
                    new Page
                    {
                        PageName = "HOME",
                        PageType = new PageType("Home"),
                        PageId = 0,
                        Elements = new List<UIElement>()
                        {
                            new ToggleButton(logic) { ModName = "MODS", Callback = (value) => {
                                if ((bool)value)
                                    logic.NavigateToPage(1);
                            }, IsSinglePressMode = true, RenderState = false},
                            new OptionElement(logic, 0, new string[] { "MADE", "BY", "COVID_GTAG" }) { ModName = "CREDITS" }
                        }
                    },

                    new Page
                    {
                        PageName = "MODS [P1]",
                        PageType = modPageType,
                        PageId = 1,
                        Elements = new List<UIElement>()
                        {
                            new ToggleButton(logic) { ModName = "LONGARMS", Callback = (value) => {
                                CovidClientMods.UseLongarms = (bool)value;
                            }},
                            new SliderElement("LONGARMS SCALE", 1, 2, logic) { Callback = (value) => CovidClientMods.RescalePlayer((float)value) },
                            new ToggleButton(logic) { ModName = "FLY", EnabledState = CovidClientMods.Fly },
                            new OptionElement(logic, 1, new string[] { "DEFAULT", "IRON MONKE" }) { ModName = "FLY OPTIONS", OptionSelected = (value) => CovidClientMods.FlyModeChanged(value) },
                            new SliderElement("FLY SPEED", 1, 100, logic) { Callback = (value) => CovidClientMods.FlySpeedChanged((float)value) },
                            new ToggleButton(logic) { ModName = "SPEEDBOOST", EnabledState = () => CovidClientMods.SpeedBoost() }
                        }
                    }
                };

                logic.AddPages(pages);
            }
            catch (System.Exception e)
            {
                NotificationSystem.Instance.CreateNotification(e.Message, UnityEngine.Color.red);
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            NotificationSystem.Instance.ManageNotifications();
            if (logic != null)
            {
                Input();
                logic.HandleStateMethods();
            }
        }

        private void Input()
        {
            AssignInputValues();

            navigation = logic.renderer.state;
            bool IsRightThumbstickUp = InputValue.RightThumbstickAxis.y > 0.4f;
            bool IsRightThumbstickDown = InputValue.RightThumbstickAxis.y < -0.4f;
            
            if (InputValue.Menu && Cooldown.CheckCooldown("ToggleGUI", 0.5f))
            {
                logic.renderer.ToggleState();
            }

            if (navigation)
            {
                if (IsRightThumbstickUp && Cooldown.CheckCooldown("NavigateItems", 0.4f))
                {
                    logic.ChangeSelectionIndex(true);
                }
                if (IsRightThumbstickDown && Cooldown.CheckCooldown("NavigateItems", 0.4f))
                {
                    logic.ChangeSelectionIndex(false);
                }
                logic.HandleInput();
            }
        }

        private void AssignInputValues()
        {
            InputValue.RightGrip = InputPoller.GripButtonDown(InputPoller.XRHand.Right);
            InputValue.LeftGrip = InputPoller.GripButtonDown(InputPoller.XRHand.Left);
            InputValue.RightPrimary = InputPoller.PrimaryButtonDown(InputPoller.XRHand.Right);
            InputValue.LeftPrimary = InputPoller.PrimaryButtonDown(InputPoller.XRHand.Left);
            InputValue.RightSecondary = InputPoller.SecondaryButtonDown(InputPoller.XRHand.Right);
            InputValue.LeftSecondary = InputPoller.SecondaryButtonDown(InputPoller.XRHand.Left);
            InputValue.Menu = InputPoller.MenuButtonDown(InputPoller.XRHand.Left);
            InputValue.RightTrigger = InputPoller.TriggerButtonDown(InputPoller.XRHand.Right);
            InputValue.LeftTrigger = InputPoller.TriggerButtonDown(InputPoller.XRHand.Left);
            InputValue.RightThumbstick = InputPoller.ThumbStickDown(InputPoller.XRHand.Right);
            InputValue.LeftThumbstick = InputPoller.ThumbStickDown(InputPoller.XRHand.Left);
            InputValue.RightThumbstickAxis = InputPoller.ThumbStick2DAxis(InputPoller.XRHand.Right);
            InputValue.LeftThumbstickAxis = InputPoller.ThumbStick2DAxis(InputPoller.XRHand.Left);
            InputValue.RightGripValue = InputPoller.GripButtonFloat(InputPoller.XRHand.Right);
            InputValue.LeftGripValue = InputPoller.GripButtonFloat(InputPoller.XRHand.Left);
            InputValue.RightTriggerValue = InputPoller.TriggerButtonFloat(InputPoller.XRHand.Right);
            InputValue.LeftTriggerValue = InputPoller.TriggerButtonFloat(InputPoller.XRHand.Left);
        }
    }
}
