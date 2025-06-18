using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePauseMan : MonoBehaviour
{

   [SerializeField] private Button unpauseButton;
   [SerializeField] private Button quitButton;
   
   private Canvas _canvas;

   private void Awake()
   {
      _canvas = GetComponent<Canvas>();
   }


   private void Start()
   {
      _canvas.enabled = false;
      unpauseButton.onClick.AddListener(() => UnpauseGame());
      quitButton.onClick.AddListener(() => BackMainMenu());
   }

   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.Escape))
      {
         bool willEnable = !_canvas.enabled;
         _canvas.enabled = willEnable;
         Time.timeScale = willEnable ? 0f : 1f;
      }
   }

   private void UnpauseGame()
   {
      _canvas.enabled = false;
      Time.timeScale = 1f;
   }

   private void BackMainMenu()
   {
      Time.timeScale = 1f;
      SceneManager.LoadSceneAsync("MainMenu");
   }

}
