using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneCutsceneWDialogue : MonoBehaviour
{

    [Serializable]
    public class FramePoint
    {
        public Sprite sprite;
        public string[] text;
    }

    [Serializable]
    public class CustomName
    {
        public string name;
        public string color;
    }
    
    [SerializeField] private CustomName[] names;
    [SerializeField] private FramePoint[] framePoints;
    [SerializeField] private float timeEachFrame = 0.2f;
    [SerializeField] private Image canvasImageObject;
    [SerializeField] private bool loadNextScene = false;
    [SerializeField] private string nextSceneToLoad;

    [SerializeField] private float fadeIn;
    [SerializeField] private float fadeOut;
    [SerializeField] private CanvasGroup fadeCg;
    
    [SerializeField] private float pauseBetweenTexts;
    [SerializeField] private GameObject panelTrans;
    [SerializeField] private TMP_Text dialogueTmp;

    private void Start()
    {
        StartCoroutine(Cutscene());
    }

    private string FixText(string text)
    {
        foreach (CustomName customName in names)
        {
            text = text.Replace($"{customName.name}: ", $"<color={customName.color}>{customName.name}: </color>");
        }
        return text;
    }
    
    private IEnumerator Cutscene()
    {
        canvasImageObject.sprite = framePoints[0].sprite;
        // fade In
        if (fadeIn > 0f)
        {
            fadeCg.alpha = 1f;
            float elapsed = 0f;
            while (elapsed < fadeIn)
            {
                elapsed += Time.deltaTime;
                fadeCg.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeIn);
                yield return null;
            }
            fadeCg.alpha = 0f;
        }
        
        int count = 0;
        while (true)
        {
            FramePoint framePoint = framePoints[count];
            canvasImageObject.sprite = framePoint.sprite;
            panelTrans.SetActive(false);
            foreach (string txt in framePoint.text)
            {
                if (!string.IsNullOrEmpty(txt))
                {
                    panelTrans.SetActive(true);
                    for (int i = 0; i < txt.Length; i++)
                    {
                        string beforeText = FixText(txt.Substring(0, i));
                        string newText = beforeText + "<color=#00000000>" + txt.Substring(i);
                        dialogueTmp.text = newText;
                        yield return new WaitForSeconds(0.03f);
                    }
                    dialogueTmp.text = FixText(txt);
                    // load text
                }

                yield return new WaitForSeconds(pauseBetweenTexts);
            }
            
            count++;
            if (count >= framePoints.Length) break;
            yield return new WaitForSeconds(timeEachFrame);
        }

        // fade Out
        if (fadeOut > 0f)
        {
            fadeCg.alpha = 0f;
            float elapsed = 0f;
            while (elapsed < fadeOut)
            {
                elapsed += Time.deltaTime;
                fadeCg.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeOut);
                yield return null;
            }
            fadeCg.alpha = 1f;
        }
        
        if (!loadNextScene) yield break;
        SceneManager.LoadSceneAsync(nextSceneToLoad);
    }
    
}
