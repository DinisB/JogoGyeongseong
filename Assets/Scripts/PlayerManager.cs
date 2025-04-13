using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    
    // Other objects references
    [SerializeField] private Transform canvas;
    
    // Player Stats (not changeable)
    private float _moveSpeed = 50f;
    private float _runningMultiplier = 2.5f;

    // Player Stats affected by player actions.
    private float _stamina = 10, _staminaMax = 10;
        
    private Rigidbody2D _rb;
    private RectTransform _staminaBar;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _staminaBar = canvas.Find("Stamina").Find("Bar").GetComponent<RectTransform>();

        _stamina = _staminaMax;
    }

    private float _holdingShiftTime = 0;
    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput != 0) transform.rotation = horizontalInput > 0 ? Quaternion.Euler(0, 180f, 0f) : Quaternion.identity;
        float verticalInput = Input.GetAxisRaw("Vertical");
        float speedMult = Input.GetKey(KeyCode.LeftShift) && _stamina > 0 ? _runningMultiplier : 1f;
        if (Input.GetKey(KeyCode.LeftShift) && _stamina > 0)
        {
            _holdingShiftTime += Time.deltaTime;
            if (_holdingShiftTime >= 1f)
            {
                _holdingShiftTime = 0f;
                ReduceStamina();
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && _holdingShiftTime > 0f)
        {
            ReduceStamina();
            _holdingShiftTime = 0f;
        }
        _rb.linearVelocity = new Vector2((horizontalInput * _moveSpeed) * speedMult, (verticalInput * _moveSpeed) * speedMult);
    }

    private void ReduceStamina()
    {
        _stamina -= 1;
        UpdateStaminaBar();
    }
    
    private void UpdateStaminaBar()
    {
        _staminaBar.localScale = new Vector3((_stamina / _staminaMax), 1f, 1f);
    }
    
}