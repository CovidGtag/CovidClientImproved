using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using CovidClientImproved.Utils;

namespace CovidClientImproved.GUI.UIElements
{
    public class Canvas
    {
        public enum AnchorPoint
        {
            TopLeft = 0,
            TopMiddle = 1,
            TopRight = 2,
            BottomLeft = 3,
            BottomMiddle = 4,
            BottomRight = 5,
        }

        public GameObject gameObject;
        public GameObject textObject;
        public UnityEngine.Canvas canvas;
        public RectTransform rectTransform;
        public AnchorPoint anchorPoint;
        public Text renderText;
        public float scaleMultiplier = 0.001f;
        public bool autoResize = true;
        public Color textColor = Color.white;
        public bool state = true;

        private object currentEffect;
        private float effectDuration = 1.0f;
        private float effectDelay = 0.0f;
        private Vector3 baseOffset = new Vector3(0f, 0f, 0.4f);

        public Canvas(AnchorPoint AnchorPoint)
        {
            try
            {
                InitializeComponents();
                ConfigureBaseOffset();
                ConfigureComponents();
            }
            catch (System.Exception e)
            {
                CMLog.Exception(e);
            }
        }

        private void InitializeComponents()
        {
            gameObject = new GameObject("CanvasObject");
            textObject = new GameObject("TextObject");

            canvas = gameObject.AddComponent<UnityEngine.Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            gameObject.transform.SetParent(GameObject.Find("Main Camera").transform, false);

            textObject.transform.SetParent(gameObject.transform, false);

            renderText = textObject.AddComponent<Text>();
            rectTransform = textObject.GetComponent<RectTransform>();
        }

        private void ConfigureBaseOffset()
        {
            switch (anchorPoint)
            {
                case AnchorPoint.TopLeft:
                    baseOffset = new Vector3(0.16f, 0.08f, 0.4f);
                    break;
                case AnchorPoint.TopMiddle:
                    baseOffset = new Vector3(0f, 0.08f, 0.4f);
                    break;
                case AnchorPoint.TopRight:
                    baseOffset = new Vector3(-0.16f, 0.08f, 0.4f);
                    break;
                case AnchorPoint.BottomLeft:
                    baseOffset = new Vector3(0.16f, -0.08f, 0.4f);
                    break;
                case AnchorPoint.BottomMiddle:
                    baseOffset = new Vector3(0f, -0.08f, 0.4f);
                    break;
                case AnchorPoint.BottomRight:
                    baseOffset = new Vector3(-0.16f, -0.08f, 0.4f);
                    break;
            }
        }

        private void ConfigureComponents()
        {
            textObject.AddComponent<ContentSizeFitter>();
            textObject.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            textObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            renderText.text = "-LOADING-";
            renderText.fontSize = 24;
            renderText.color = Color.white;
            renderText.alignment = TextAnchor.UpperLeft;
            renderText.font = GameObject.Find("COC Text").GetComponent<Text>().font;

            rectTransform.localPosition = baseOffset;
            rectTransform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
            rectTransform.sizeDelta = new Vector2(rectTransform.rect.width * 1.5f, rectTransform.rect.height * 1.5f);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.transform.localPosition = baseOffset;
        }

        public void ToggleState()
        {
            gameObject.SetActive(!state);
            state = !state;
        }

        public void UpdateText(string newText)
        {
            if (renderText)
            {
                renderText.text = newText;
            }
        }

        public void SetFontSize(int size)
        {
            if (renderText)
                renderText.fontSize = size;
        }

        public void TransitionText(string newText)
        {
            if (string.IsNullOrEmpty(newText))
            {
                UpdateText(newText);
                return;
            }

            MelonLoader.MelonCoroutines.Start(TypeText(newText));
        }

        private IEnumerator TypeText(string targetText)
        {
            string currentText = renderText.text;
            int currentIndex = 0;

            while (currentIndex < targetText.Length)
            {
                currentText = targetText.Substring(0, currentIndex + 1);
                renderText.text = currentText;

                yield return new WaitForSeconds(0.08f);

                currentIndex++;
            }
        }

        public void FadeText(float duration, float delay = 0.0f)
        {
            if (currentEffect != null)
                StopCurrentEffect();

            effectDuration = duration;
            effectDelay = delay;
            currentEffect = MelonLoader.MelonCoroutines.Start(FadeEffect());
        }

        public void PulseText(float duration, float delay = 0.0f)
        {
            if (currentEffect != null)
                StopCurrentEffect();

            effectDuration = duration;
            effectDelay = delay;
            currentEffect = MelonLoader.MelonCoroutines.Start(PulseEffect());
        }

        public void ShakeText(float duration, float delay = 0.0f)
        {
            if (currentEffect != null)
                StopCurrentEffect();

            effectDuration = duration;
            effectDelay = delay;
            currentEffect = MelonLoader.MelonCoroutines.Start(ShakeEffect());
        }

        private IEnumerator FadeEffect()
        {
            if (effectDelay > 0)
                yield return new WaitForSeconds(effectDelay);

            Color originalColor = renderText.color;
            float elapsedTime = 0;

            while (elapsedTime < effectDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / effectDuration);
                renderText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            renderText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        }

        private IEnumerator PulseEffect()
        {
            if (effectDelay > 0)
                yield return new WaitForSeconds(effectDelay);

            Vector3 originalScale = rectTransform.localScale;
            float elapsedTime = 0;

            while (elapsedTime < effectDuration)
            {
                float scale = Mathf.Lerp(1f, 1.2f, Mathf.Sin(elapsedTime * Mathf.PI / effectDuration));
                rectTransform.localScale = new Vector3(
                    originalScale.x * scale,
                    originalScale.y * scale,
                    originalScale.z * scale
                );
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            rectTransform.localScale = originalScale;
        }

        private IEnumerator ShakeEffect()
        {
            if (effectDelay > 0)
                yield return new WaitForSeconds(effectDelay);

            Vector3 originalPosition = rectTransform.localPosition;
            float elapsedTime = 0;

            while (elapsedTime < effectDuration)
            {
                float xOffset = Random.Range(-0.01f, 0.01f);
                float yOffset = Random.Range(-0.01f, 0.01f);
                rectTransform.localPosition = originalPosition + new Vector3(xOffset, yOffset, 0);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            rectTransform.localPosition = originalPosition;
        }

        public void StopCurrentEffect()
        {
            if (currentEffect != null)
            {
                MelonLoader.MelonCoroutines.Stop(currentEffect);
                currentEffect = null;
            }
        }

        public void ResetText()
        {
            StopCurrentEffect();
            renderText.color = textColor;
            rectTransform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier);
            rectTransform.localPosition = baseOffset;
        }
    }
}
