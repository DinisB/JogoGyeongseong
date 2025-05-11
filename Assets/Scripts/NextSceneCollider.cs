using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneCollider : MonoBehaviour
{
    [SerializeField] private string nameScene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player") {
            SceneManager.LoadSceneAsync(nameScene);
        }
    }
}
