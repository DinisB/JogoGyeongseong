using System;
using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    
    [Header("Other Objects, in scene or prefab")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject bulletPrefab;
    
    // Player Stats (not changeable ingame)
    [Header("Player Stats")]
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private float runningMultiplier = 2.5f;
    [SerializeField] private float maxStamina = 10f;
    [SerializeField] private float timeBeforeStaminaRegen = 2f;
    [SerializeField] private float staminaRegenRate = 2f;

    private Transform _shootPos;
    private ParticleSystem _shootingParticles;
    private Rigidbody2D _rb;
    private RectTransform _staminaBar;
    private Vector2 _lookingDirection = Vector2.right;
    private float _currentStamina;
    private bool _isStaminaRegenerating = false;
    private Coroutine _staminaRegenCoroutine;
    
    // this property can be public if needed elsewhere.
    private float StaminaPercent => _currentStamina / maxStamina;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    
    void Start()
    {
        _staminaBar = canvas.Find("Stamina").Find("Bar").GetComponent<RectTransform>();
        _shootPos = transform.Find("Gun").Find("ShootPos");
        _shootingParticles = _shootPos.Find("ShootingParticles").GetComponent<ParticleSystem>();

        _currentStamina = maxStamina;
        UpdateStaminaBar();
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
        HandleStamina();
        
        if (Input.GetKeyDown(KeyCode.Tab)) gameManager.PopInventory();
    }
    
    private void UpdateLookingDirection(float horizontalInput, float verticalInput)
    {
        // Determine the primary direction of movement
        if (Mathf.Abs(horizontalInput) > Mathf.Abs(verticalInput))
        {
            _lookingDirection = horizontalInput > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            _lookingDirection = verticalInput > 0 ? Vector2.up : Vector2.down;
        }
    }
    
    private void HandleShooting()
    {
        // add small cooldown later just for the particle system to stop his animation and not overlap if needed.
        // or just add small cooldown cause even in real life weapons are not instant unless it's automatic weapon.
        // if automatic weapon:
        // we change later this GetMouseButton, so it shoots while button is being held, and we add a
        // very small cooldown to it so bullets don't go all out at the same time. thoughts?
        if (Input.GetMouseButtonDown(0)) ShootGun();
    }
    
    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        
        // Update player rotation based on horizontal input
        if (horizontalInput != 0) 
        {
            transform.rotation = horizontalInput > 0 ? 
                Quaternion.identity : Quaternion.Euler(0, 180f, 0f);
        }
        
        // Update looking direction for shooting when player is moving
        if (horizontalInput != 0 || verticalInput != 0)
        {
            UpdateLookingDirection(horizontalInput, verticalInput);
        }
        
        // Calculate movement speed with sprint if possible
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && _currentStamina > 0;
        float speedMultiplier = isRunning ? runningMultiplier : 1f;
        
        // Apply movement
        Vector2 movement = new Vector2((horizontalInput * moveSpeed) * speedMultiplier, (verticalInput * moveSpeed) * speedMultiplier);
        _rb.linearVelocity = movement;
        
        // Handle running stamina consumption
        if (isRunning)
        {
            ConsumeStamina(Time.deltaTime);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            TryStartStaminaRegen();
        }
    }
    
    private void TryStartStaminaRegen()
    {
        if (_staminaRegenCoroutine != null)
        {
            StopCoroutine(_staminaRegenCoroutine);
        }
        _staminaRegenCoroutine = StartCoroutine(RegenerateStamina());
    }
    
    private void HandleStamina()
    {
        if (_isStaminaRegenerating && _currentStamina < maxStamina)
        {
            _currentStamina += Time.deltaTime * staminaRegenRate;
            _currentStamina = Mathf.Min(_currentStamina, maxStamina);
            UpdateStaminaBar();
        }
    }
    
    private void ConsumeStamina(float amount)
    {
        if (_staminaRegenCoroutine != null)
        {
            StopCoroutine(_staminaRegenCoroutine);
            _isStaminaRegenerating = false;
        }
        
        _currentStamina -= amount;
        _currentStamina = Mathf.Max(_currentStamina, 0f);
        UpdateStaminaBar();
    }

    private void ShootGun()
    {
        GameObject spawnedBullet = Instantiate(bulletPrefab, _shootPos.position, Quaternion.identity);
        if (spawnedBullet.TryGetComponent(out BulletManager bulletManager))
        {
            bulletManager.SetDirection(_lookingDirection);
        }

        // I never use vars but in this case it works fine so I will keep it for now.
        var main = _shootingParticles.main;
        
        // Set the start rotation
        main.startRotation = GetParticleSystemRotation();
        _shootingParticles.Play();
    }

    private float GetParticleSystemRotation()
    {
        float angle = 0f;
        if (_lookingDirection == Vector2.left) angle = 180f;
        else if (_lookingDirection == Vector2.down) angle = 90f;
        else if (_lookingDirection == Vector2.up) angle = 270f;
        return angle * Mathf.Deg2Rad;
    }
    
    private void UpdateStaminaBar()
    {
        _staminaBar.localScale = new Vector3(StaminaPercent, 1f, 1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // to add later: player's health reduction
        if (other.CompareTag("Bullet")) Destroy(other.gameObject);
    }
    
    private IEnumerator RegenerateStamina()
    {
        yield return new WaitForSeconds(timeBeforeStaminaRegen);
        _isStaminaRegenerating = true;
    }
    
}