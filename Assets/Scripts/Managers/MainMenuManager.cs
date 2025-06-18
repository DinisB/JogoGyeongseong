using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DefaultNamespace.Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        
        
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;

        [SerializeField] private float timeClicked = 0.1f;
        [SerializeField] private float increaseSizeClick = 0.1f;
        [SerializeField] private float decreaseSize = 0.5f;
        [SerializeField] private float waitClicked = 1f;
        [SerializeField] private AudioSource audioSource;
        
        private void Start()
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(() => ClickPlayGameButton());
            quitButton.onClick.AddListener(() => ClickQuitGameButton());
        }

        public void ClickPlayGameButton()
        {
            playButton.interactable = false;
            if (playButton.TryGetComponent(out HoverButton hoverButton))
            {
                hoverButton.Enabled = false;
            }

            StartCoroutine(PlayButtonAnimation(playButton, () => SceneManager.LoadSceneAsync("Cutscene1")));
        }

        public void ClickQuitGameButton()
        {
            StartCoroutine(PlayButtonAnimation(quitButton, () => Application.Quit()));
        }

        private IEnumerator PlayButtonAnimation(Button button, UnityAction unityAction)
        {
            Vector3 baseSize = button.transform.localScale;
            button.transform.localScale = baseSize + (baseSize * increaseSizeClick);
            yield return new WaitForSeconds(timeClicked);
            float elapsed = 0f;
            Vector3 endSize = baseSize * decreaseSize;
            CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
            float startSound = audioSource.volume;
            while (elapsed < waitClicked)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / waitClicked;
                button.transform.localScale = Vector3.Lerp(baseSize, endSize, t);
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                audioSource.volume = Mathf.Lerp(startSound, 0f, t);
                yield return null;
            }
            audioSource.volume = 0f;
            button.transform.localScale = endSize;
            unityAction.Invoke();
        }
        
    }
}