using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MelonLoader;
using CovidClientImproved.CC.Input;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using Player = GorillaLocomotion.Player;
using CovidClientImproved.Utils;

namespace CovidClientImproved.CC.Mods
{
    public class CovidClientMods
    {
        public static Vector3 PlayerScale = new Vector3(1f, 1f, 1f);
        public static float FlySpeed = 5f;
        public static bool UseLongarms = false;

        public static string FlyOption = "DEFAULT";

        public static void RescalePlayer(float value)
        {
            if (!UseLongarms)
                return;

            PlayerScale = new Vector3(value, value, value);
            SubmitScale();
        }

        private static void SubmitScale()
        {
            GameObject.Find("GorillaPlayer").transform.localScale = PlayerScale;
        }

        public static void FlySpeedChanged(float value)
        {
            FlySpeed = value;
        }

        public static void FlyModeChanged(string value)
        {
            FlyOption = value;
        }

        public static void Fly()
        {
            if (!InputValue.RightPrimary && FlyOption != "IRON MONKE")
                return;

            Vector3 currentVelocity;
            Vector3 smoothedVelocity;

            switch (FlyOption)
            {
                case "DEFAULT":
                    if (!InputValue.RightPrimary)
                        return;

                    Vector3 localForward = Player.Instance.headCollider.transform.forward;
                    Vector3 targetMovement = localForward * FlySpeed * Time.deltaTime;

                    Player.Instance.transform.position += targetMovement;
                    break;

                case "IRON MONKE":
                    float leftTriggerValue = InputValue.LeftTriggerValue;
                    float rightTriggerValue = InputValue.RightTriggerValue;
                    
                    if (leftTriggerValue <= 0f && rightTriggerValue <= 0f)
                        return;

                    Vector3 leftThrust = Vector3.zero;
                    Vector3 rightThrust = Vector3.zero;
                    
                    if (leftTriggerValue > 0f)
                        leftThrust = Player.Instance.leftHandTransform.forward * FlySpeed;

                    if (rightTriggerValue > 0f)
                        rightThrust = Player.Instance.rightHandTransform.forward * FlySpeed;
                    
                    Vector3 totalThrust = leftThrust + rightThrust;
                    
                    if (totalThrust == Vector3.zero)
                        return;

                    currentVelocity = Player.Instance.playerRigidBody.velocity;
                    smoothedVelocity = Vector3.Lerp(currentVelocity, totalThrust, Time.deltaTime * 10f);

                    Player.Instance.playerRigidBody.velocity = smoothedVelocity;
                    GorillaTagger.Instance.StartVibration(((leftTriggerValue > 0f) ? true : false), 0.12f, 0.2f);
                    break;
                default:
                    break;
            }
        }

        public static void SpeedBoost()
        {
            Player.Instance.maxJumpSpeed = 8.6f;
            Player.Instance.jumpMultiplier = 1.4f;
        }
    }
}
