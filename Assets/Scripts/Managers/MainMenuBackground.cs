using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBackground : MonoBehaviour
{
    
    [Serializable]
    public class BackgroundPoint
    {
        public Sprite[] sprites;
        public float fadeIn;
        public float time;
        public float fadeOut;
        public float delayAfter;
    }

    private int _backgroundCount = 0;
    [SerializeField] private BackgroundPoint[] backgroundPoints;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image fadeImage;
    
    private void OnEnable()
    {
        _backgroundCount = 0;
        StartCoroutine(Animation(backgroundPoints[_backgroundCount]));
    }
    
    private void PlayNextAnimation() 
    {
        _backgroundCount++;
        if (_backgroundCount >= backgroundPoints.Length) _backgroundCount = 0;
        StartCoroutine(Animation(backgroundPoints[_backgroundCount]));
    }

    private IEnumerator Animation(BackgroundPoint backgroundPoint)
    {
        // fade in
        backgroundImage.sprite = backgroundPoint.sprites[0];
        if (backgroundPoint.fadeIn > 0f)
        {
            CanvasGroup canvasGroup = fadeImage.GetComponent<CanvasGroup>();
            float elapsed = 0f;
            while (elapsed < backgroundPoint.fadeIn)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / backgroundPoint.fadeIn);
                yield return null;
            }
            canvasGroup.alpha = 0f;
        }
        // cutscene
        int count = 0;
        while (true)
        {
            backgroundImage.sprite = backgroundPoint.sprites[count];
            count++;
            if (count >= backgroundPoint.sprites.Length) break;
            yield return new WaitForSeconds(backgroundPoint.time);
        }
        
        // fade out
        if (backgroundPoint.fadeOut > 0f)
        {
            CanvasGroup canvasGroup = fadeImage.GetComponent<CanvasGroup>();
            float elapsed = 0f;
            while (elapsed < backgroundPoint.fadeOut)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / backgroundPoint.fadeOut);
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }
        
        // delay
        yield return new WaitForSeconds(backgroundPoint.delayAfter);
        Invoke(nameof(PlayNextAnimation), 0.1f);
    }
}
