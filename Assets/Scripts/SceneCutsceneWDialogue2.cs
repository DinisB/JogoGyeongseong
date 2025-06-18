using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class SceneCutsceneWDialogue2 : MonoBehaviour
    {
        

        [Serializable]
        public class CustomName
        {
            public string name;
            public string color;
        }

        [SerializeField] private Sprite[] sprites;
        [SerializeField] private int[] spritesStopMotion;
        [SerializeField] private string[] texts;
        [SerializeField] private CustomName[] names;
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

        private IEnumerator BackgroundStopMotion()
        {
            int count = 0;
            while (count < sprites.Length)
            {
                canvasImageObject.sprite = sprites[count];
                count++;
                if (count >= sprites.Length) break;
                yield return new WaitForSeconds(timeEachFrame);
                if (_hasDialoguesFinished) continue;
                if (_dialogueCoro == null && count == spritesStopMotion[0]) _dialogueCoro = StartCoroutine(DialogueTexts());
                if (_dialogueCoro != null && !_hasDialoguesFinished)
                {
                    if (count > spritesStopMotion[^1]) count = spritesStopMotion[0];
                }
            }
            
            StartCoroutine(ChangeScene());
        }

        private string FixText(string text)
        {
            foreach (CustomName customName in names)
            {
                text = text.Replace($"{customName.name}: ", $"<color={customName.color}>{customName.name}: </color>");
            }
            return text;
        }

        private Coroutine _dialogueCoro;
        private bool _hasDialoguesFinished = false;
        private IEnumerator DialogueTexts()
        {
            panelTrans.SetActive(false);
            foreach (string txt in texts)
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
            yield return null;
            panelTrans.SetActive(false);
            _hasDialoguesFinished = true;
        }

        public IEnumerator ChangeScene()
        {
            
            Debug.Log("1");
            //StopCoroutine(coro);
            
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
            Debug.Log("2");
        
            if (!loadNextScene) yield break;
            SceneManager.LoadSceneAsync(nextSceneToLoad);
            Debug.Log("3");
        }
        
        private IEnumerator Cutscene()
        {
            // fade In
            canvasImageObject.sprite = sprites[0];
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

            StartCoroutine(BackgroundStopMotion());
        }
        
    }
}