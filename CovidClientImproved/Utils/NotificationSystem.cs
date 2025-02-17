using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MelonLoader;

namespace CovidClientImproved.Utils
{
    class NotificationSystem
    {
        private static NotificationSystem _instance;
        public static NotificationSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NotificationSystem();
                }
                return _instance;
            }
        }

        public List<Notification> activeNotifications = new List<Notification>();
        public Canvas SharedCanvas;
        public float NotificationDelay = 0.3f;
        public float lastNotificationCreationTime = 0f;
        private float NotificationDuration = 1.5f;

        private NotificationSystem() { }

        /// <summary>
        /// Initializes the shared canvas if it hasn't been initialized yet.
        /// </summary>
        public void InitializeSharedCanvas()
        {
            if (SharedCanvas == null)
            {
                SharedCanvas = new GameObject("SharedCanvas").AddComponent<Canvas>();
                SharedCanvas.renderMode = RenderMode.WorldSpace;
                SharedCanvas.transform.SetParent(GameObject.Find("Main Camera").transform, false);
            }
        }

        /// <summary>
        /// Creates a new notification with the provided message, color, and optional duration.
        /// Notifications will not be created if the cooldown period has not passed since the last notification was created.
        /// </summary>
        /// <param name="message">The text content of the notification.</param>
        /// <param name="color">The color of the notification text.</param>
        /// <param name="duration">The duration for which the notification should be displayed (default is 4 seconds).</param>
        public void CreateNotification(string message, Color color, float duration = 4f)
        {
            if (Time.time - lastNotificationCreationTime < NotificationDelay)
            {
                return;
            }

            var notification = new Notification(SharedCanvas, message, color, duration)
            {
                Lifetime = 0
            };
            activeNotifications.Add(notification);
            lastNotificationCreationTime = Time.time;

            MelonCoroutines.Start(AnimateNotificationsIntoPosition());
        }

        /// <summary>
        /// Manages the lifecycle of notifications, updating their lifetime and removing any that have expired or exceed the maximum count.
        /// </summary>
        public void ManageNotifications()
        {
            if (activeNotifications.Count != 0)
            {
                foreach (var notification in activeNotifications.ToList())
                {
                    notification.Lifetime += Time.deltaTime;

                    if (notification.Lifetime > notification.Duration)
                    {
                        RemoveNotification(notification);
                    }
                }
            }

            while (activeNotifications.Count > 5)
            {
                var oldestNotification = activeNotifications.First();
                RemoveNotification(oldestNotification);
            }
        }

        /// <summary>
        /// Removes a specific notification from the system, triggering an animation for the removal and updating the positions of remaining notifications.
        /// </summary>
        /// <param name="notification">The notification to remove.</param>
        private void RemoveNotification(Notification notification)
        {
            MelonCoroutines.Start(notification.AnimateFall(notification.NotificationObject, 0.5f));
            activeNotifications.Remove(notification);
            UpdateNotificationPositions();
        }

        /// <summary>
        /// Updates the positions of all active notifications to ensure they are spaced out correctly on the screen.
        /// </summary>
        private void UpdateNotificationPositions()
        {
            float offsetY = -0.1f;
            foreach (var notification in activeNotifications)
            {
                Vector3 targetPosition = new Vector3(-0.16f, offsetY, 0.4f);

                MelonCoroutines.Start(MoveNotificationOverTime(notification.TextObject.GetComponent<RectTransform>(), targetPosition));

                offsetY += 0.028f;
            }
        }

        /// <summary>
        /// Moves a notification's UI element to a target position over time using linear interpolation.
        /// </summary>
        /// <param name="rectTransform">The RectTransform component of the notification's UI element.</param>
        /// <param name="targetPosition">The target position to move the notification to.</param>
        private IEnumerator MoveNotificationOverTime(RectTransform rectTransform, Vector3 targetPosition)
        {
            float duration = NotificationDuration;
            float elapsedTime = 0f;

            Vector3 startingPosition = rectTransform.localPosition;

            while (elapsedTime < duration)
            {
                rectTransform.localPosition = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            rectTransform.localPosition = targetPosition;
        }

        /// <summary>
        /// Animates all active notifications into their final positions on the screen.
        /// </summary>
        private IEnumerator AnimateNotificationsIntoPosition()
        {
            float offsetY = -0.1f;
            foreach (var notification in activeNotifications)
            {
                Vector3 targetPosition = new Vector3(-0.16f, offsetY, 0.4f);
                yield return MoveNotificationOverTime(notification.TextObject.GetComponent<RectTransform>(), targetPosition);

                offsetY += 0.03f;
            }
        }
    }

    class Notification
    {
        public GameObject CanvasObject;
        public GameObject TextObject;
        public GameObject NotificationObject;
        public Canvas SharedCanvas;
        public float Duration;
        public string Message;
        public float Lifetime;

        public Notification(Canvas canvas, string initialMessage, Color defaultColor, float duration = 10f)
        {
            Message = initialMessage;
            Lifetime = 0f;
            SharedCanvas = canvas;
            NotificationObject = CreateNotification(defaultColor);
            Duration = duration;
        }

        /// <summary>
        /// Animates the notification object to fall off the screen after a specified duration.
        /// </summary>
        /// <param name="notificationObject">The GameObject representing the notification.</param>
        /// <param name="fallDuration">The duration of the fall animation.</param>
        public IEnumerator AnimateFall(GameObject notificationObject, float fallDuration)
        {
            float elapsedTime = 0;
            float endOffset = 0.15f;
            Vector3 startPosition = TextObject.GetComponent<RectTransform>().localPosition;
            Vector3 endPosition = new Vector3(startPosition.x, startPosition.y - endOffset, startPosition.z);

            while (elapsedTime < fallDuration)
            {
                TextObject.GetComponent<RectTransform>().localPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / fallDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Object.Destroy(notificationObject);
        }

        /// <summary>
        /// Creates a notification object with the specified color.
        /// </summary>
        /// <param name="color">The color of the notification text.</param>
        /// <returns>The GameObject representing the notification text UI element.</returns>
        public GameObject CreateNotification(Color color)
        {
            CanvasObject = SharedCanvas.gameObject;
            TextObject = new GameObject($"NotificationText_{NotificationSystem.Instance.activeNotifications.Count}");
            TextObject.AddComponent<Text>();
            TextObject.AddComponent<ContentSizeFitter>();
            TextObject.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            TextObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            TextObject.GetComponent<Text>().fontSize = 60;
            TextObject.GetComponent<Text>().color = color;
            TextObject.GetComponent<Text>().alignment = TextAnchor.UpperLeft;
            TextObject.GetComponent<Text>().font = GameObject.Find("COC Text").GetComponent<Text>().font;

            MelonCoroutines.Start(TypeText("[CC] " + Message));

            RectTransform rectTransform = TextObject.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(-0.16f, 0f, 0.4f);
            rectTransform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
            rectTransform.sizeDelta = new Vector2(rectTransform.rect.width * 1.5f, rectTransform.rect.height * 1.5f);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            TextObject.transform.SetParent(CanvasObject.transform, false);
            int index = NotificationSystem.Instance.activeNotifications.Count;
            float offsetY = -0.05f + (index * 0.03f);
            rectTransform.transform.localPosition = new Vector3(-0.16f, offsetY, 0.4f);
            return TextObject;
        }

        private IEnumerator TypeText(string targetText)
        {
            string currentText = TextObject.GetComponent<Text>().text;
            int currentIndex = 0;

            while (currentIndex < targetText.Length)
            {
                currentText = targetText.Substring(0, currentIndex + 1);
                TextObject.GetComponent<Text>().text = currentText;

                yield return new WaitForSeconds(0.08f);

                currentIndex++;
            }
        }
    }
}