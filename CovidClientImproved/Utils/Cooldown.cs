using System.Collections.Generic;
using UnityEngine;

namespace CovidClientImproved.Utils
{
    public class Cooldown
    {
        private static readonly Dictionary<string, (float startTime, float duration)> cooldowns = new Dictionary<string, (float startTime, float duration)>();

        public static bool CheckCooldown(string actionName, float duration)
        {
            try
            {
                if (!cooldowns.ContainsKey(actionName))
                {
                    cooldowns[actionName] = (Time.time, duration);
                    return false;
                }

                var (startTime, cdDuration) = cooldowns[actionName];

                if (Time.time < startTime + cdDuration)
                {
                    return false;
                }

                cooldowns[actionName] = (Time.time, duration);
                return true;
            }
            catch (System.Exception ex)
            {
                CMLog.Error($"Error in cooldown: {ex.Message}");
                return false;
            }
        }
    }
}