using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private RectTransform canvas;
    
    // values to maybe change later??
    [SerializeField] private float inventoryStartDisappearTime = 2f;
    [SerializeField] private float inventoryDisappearTime = 1f;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private Coroutine _disappearInventory;
    public void PopInventory()
    {
        if (_disappearInventory != null) StopCoroutine(_disappearInventory);
        if (!canvas.Find("Inventory").TryGetComponent(out CanvasGroup canvasGroup))
        {
            // In case can't find CanvasGroup?? should be impossible but we never know...
            Debug.Log("Couldn't find CanvasGroup on Inventory[Canvas child], creating one...");
            canvasGroup = canvas.Find("Inventory").gameObject.AddComponent<CanvasGroup>();
        }
        _disappearInventory = StartCoroutine(DisappearingInventory(canvasGroup));
    }

    private IEnumerator DisappearingInventory(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(inventoryStartDisappearTime);
        float elapsed = 0f;
        while (elapsed < inventoryDisappearTime)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, (elapsed / inventoryDisappearTime));
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
