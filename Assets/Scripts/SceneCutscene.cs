using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneCutscene : MonoBehaviour
{

    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float timeEachFrame = 0.2f;
    [SerializeField] private Image canvasImageObject;
    [SerializeField] private bool loadNextScene = false;
    [SerializeField] private string nextSceneToLoad;
    [SerializeField] private bool hasMessage;
    [SerializeField] private GameObject messageBox;

    private void Start()
    {
        StartCoroutine(Cutscene());
    }

    private IEnumerator Cutscene()
    {
        int count = 0;
        while (true)
        {
            canvasImageObject.sprite = sprites[count];
            count++;
            if (count >= sprites.Length) break;
            yield return new WaitForSeconds(timeEachFrame);
        }
        yield return null;
        if (!hasMessage) yield break;
        messageBox.SetActive(true);
        if (!SceneManager.GetSceneByName(nextSceneToLoad).IsValid() || !loadNextScene) yield break;
        SceneManager.LoadSceneAsync(nextSceneToLoad);
    }
    
}
