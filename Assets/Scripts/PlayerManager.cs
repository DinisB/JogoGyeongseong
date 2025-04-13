using System;
using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    
    // Other objects references
    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject bulletPrefab;
    private Transform _shootPos;
    
    // Player Stats (not changeable)
    private float _timeBeforeStaminaRegen = 2f;
    private float _moveSpeed = 50f;
    private float _runningMultiplier = 2.5f;

    // Player Stats affected by player actions.
    private bool _staminaRegen = false;
    private float _stamina = 10, _staminaMax = 10;
        
    private Rigidbody2D _rb;
    private RectTransform _staminaBar;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _staminaBar = canvas.Find("Stamina").Find("Bar").GetComponent<RectTransform>();
        _shootPos = transform.Find("Gun").Find("ShootPos");

        _stamina = _staminaMax;
    }

    private Vector2 _lookingDirection = Vector2.right;
    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput != 0) transform.rotation = horizontalInput > 0 ? Quaternion.identity : Quaternion.Euler(0, 180f, 0f);
        float verticalInput = Input.GetAxisRaw("Vertical");
        // Only update looking direction when player is actually moving
        if (horizontalInput != 0 || verticalInput != 0)
        {
            // update direction the player is facing for shooting.
            if (horizontalInput > 0 && horizontalInput > verticalInput) _lookingDirection = Vector2.right;
            if (horizontalInput < 0 && horizontalInput < verticalInput) _lookingDirection = Vector2.left;
            if (verticalInput > 0 && verticalInput > horizontalInput) _lookingDirection = Vector2.up;
            if (verticalInput < 0 && verticalInput < horizontalInput) _lookingDirection = Vector2.down;
        }
        float speedMult = Input.GetKey(KeyCode.LeftShift) && _stamina > 0 ? _runningMultiplier : 1f;
        if (Input.GetKey(KeyCode.LeftShift) && _stamina > 0)
        {
            if (_RegenCoroutine != null)
            {
                StopCoroutine(_RegenCoroutine);
                _staminaRegen = false;
            }
            _stamina -= Time.deltaTime;
            UpdateStaminaBar();
        }
        _rb.linearVelocity = new Vector2((horizontalInput * _moveSpeed) * speedMult, (verticalInput * _moveSpeed) * speedMult);
        
        if (Input.GetMouseButtonDown(0)) ShootGun();

        if (_staminaRegen && _stamina < 10)
        {
            _stamina += Time.deltaTime * 2;
            UpdateStaminaBar();
        }
        
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (_RegenCoroutine != null) StopCoroutine(_RegenCoroutine);
            _RegenCoroutine = StartCoroutine(RegenStamina());
        }
    }

    private void ShootGun()
    {
        GameObject spawnedBullet = Instantiate(bulletPrefab, _shootPos.position, Quaternion.identity);
        if (spawnedBullet.TryGetComponent(out BulletManager bulletManager))
        {
            bulletManager.SetDirection(_lookingDirection);
        }
    }
    
    private void UpdateStaminaBar()
    {
        if (_stamina < 0) _stamina = 0;
        if (_stamina > 10) _stamina = 10;
        _staminaBar.localScale = new Vector3((_stamina / _staminaMax), 1f, 1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Bullet")) return;
        Destroy(other.gameObject);
    }

    private Coroutine _RegenCoroutine;
    private IEnumerator RegenStamina()
    {
        yield return new WaitForSeconds(_timeBeforeStaminaRegen);
        _staminaRegen = true;
    }
}