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
        // Disappear inventory on start
        PopInventory(true);
    }

    private Coroutine _disappearInventory;
    public void PopInventory(bool firstTime = false)
    {
        if (_disappearInventory != null) StopCoroutine(_disappearInventory);
        if (!canvas.Find("Inventory").TryGetComponent(out CanvasGroup canvasGroup))
        {
            // In case can't find CanvasGroup?? should be impossible but we never know...
            Debug.Log("Couldn't find CanvasGroup on Inventory[Canvas child], creating one...");
            canvasGroup = canvas.Find("Inventory").gameObject.AddComponent<CanvasGroup>();
        }
        _disappearInventory = StartCoroutine(DisappearingInventory(canvasGroup, firstTime));
    }

    /// <summary>
    /// Called to ensure the items displayed on the inventory are correct.
    /// </summary>
    private void UpdateInventoryItems()
    {
        
    }
    
    /// <summary>
    /// Coroutine that will make the inventory appear and disappear after the time set on the script.
    /// </summary>
    /// <param name="canvasGroup"></param>
    /// <param name="firstTime"></param>
    /// <returns></returns>
    private IEnumerator DisappearingInventory(CanvasGroup canvasGroup, bool firstTime = false)
    {
        if (firstTime)
        {
            canvasGroup.alpha = 0f;
            yield break;
        }
        UpdateInventoryItems();
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
