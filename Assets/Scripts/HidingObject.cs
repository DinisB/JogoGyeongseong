using System;
using UnityEngine;

public class HidingObject : MonoBehaviour
{

    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite outlinedSprite;
    [SerializeField] private Sprite hidingSprite;

    private SpriteRenderer _spriteRenderer;
    private GameObject _canvasObject;

    private Vector3 _playerLocation;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _canvasObject = transform.Find("ObjectCanvas").gameObject;
        _canvasObject.SetActive(false);
    }

    /// <summary>
    /// Called when the player is in bounds of the trigger.
    /// </summary>
    public void PlayerInBounds()
    {
        _spriteRenderer.sprite = outlinedSprite;
        _canvasObject.SetActive(true);
    }

    /// <summary>
    /// Called when the player is out of bounds of the trigger.
    /// </summary>
    public void PlayerOutOfBounds()
    {
        _spriteRenderer.sprite = defaultSprite;
        _canvasObject.SetActive(false);
    }

    /// <summary>
    /// Called when the player wants to hide in this object.
    /// </summary>
    public void PlayerHideHere(Vector3 playerLocation)
    {
        _spriteRenderer.sprite = hidingSprite;
        _playerLocation = playerLocation;
    }

    /// <summary>
    /// Called when the player exists this object.
    /// </summary>
    /// <returns>Returns the location the player was before hiding.</returns>
    public Vector3 PlayerExitHideSpot()
    {
        _spriteRenderer.sprite = outlinedSprite;
        return _playerLocation;
    }
}
