using System;
using System.Collections;
using System.Collections.Generic;
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
    private Animator anim;

    private Transform _shootPos;
    private ParticleSystem _shootingParticles;
    private Rigidbody2D _rb;
    private RectTransform _staminaBar;
    private Vector2 _lookingDirection = Vector2.right;
    private float _currentStamina;
    private bool _isStaminaRegenerating = false;
    private Coroutine _staminaRegenCoroutine;
    private List<GameObject> _hidingSpotsInReach = new List<GameObject>();
    private GameObject _hidingObject = null;
    
    // this property can be public if needed elsewhere.
    private float StaminaPercent => _currentStamina / maxStamina;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    
    void Start()
    {
        anim = GetComponent<Animator>();
        _staminaBar = canvas.Find("Stamina").Find("Bar").GetComponent<RectTransform>();
        _shootPos = transform.Find("Gun").Find("ShootPos");
        _shootingParticles = _shootPos.Find("ShootingParticles").GetComponent<ParticleSystem>();

        _currentStamina = maxStamina;
        UpdateStaminaBar();
    }

    void Update()
    {
        if (_hidingObject == null) { // These only work when the player is NOT HIDING.
            HandleMovement();
            HandleShooting();
            HandleStamina();
        }
        
        if (Input.GetKeyDown(KeyCode.Tab)) gameManager.PopInventory();

        if (Input.GetKeyDown(KeyCode.Space)) HandleHiding();
    }
    
    /// <summary>
    /// Find exactly which direction the player is facing, used mostly for shooting accurately in the right direction.
    /// </summary>
    /// <param name="horizontalInput">Input.GetAxisRaw("Horizontal")</param>
    /// <param name="verticalInput">Input.GetAxisRaw("Vertical")</param>
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
    
    /// <summary>
    /// Handle any movement the player does, from normal WASD and running/sprinting.
    /// </summary>
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
            anim.SetFloat("X", horizontalInput);
            anim.SetFloat("Y", verticalInput);
            anim.SetBool("Walking", true);
        }
        else {
            anim.SetBool("Walking", false);
        }
        
        // Calculate movement speed with sprint if possible
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && _currentStamina > 0;
        float speedMultiplier = isRunning ? runningMultiplier : 1f;
        
        // Apply movement
        Vector2 movement = new Vector2((horizontalInput * moveSpeed) * speedMultiplier, (verticalInput * moveSpeed) * speedMultiplier);
        _rb.linearVelocity = movement;
        
        // Handle running stamina consumption
        if (isRunning) ConsumeStamina(Time.deltaTime);
        else if (Input.GetKeyUp(KeyCode.LeftShift)) TryStartStaminaRegen();
    }
    
    /// <summary>
    /// Start the coroutine for the stamina regeneration and cancel any previous one in case of having.
    /// </summary>
    private void TryStartStaminaRegen()
    {
        if (_staminaRegenCoroutine != null) StopCoroutine(_staminaRegenCoroutine);
        _staminaRegenCoroutine = StartCoroutine(RegenerateStamina());
    }
    
    /// <summary>
    /// Handles the stamina regeneration.
    /// </summary>
    private void HandleStamina()
    {
        if (_isStaminaRegenerating && _currentStamina < maxStamina)
        {
            _currentStamina += Time.deltaTime * staminaRegenRate;
            _currentStamina = Mathf.Min(_currentStamina, maxStamina);
            UpdateStaminaBar();
        }
    }
    
    /// <summary>
    /// Consume stamina upon use.
    /// </summary>
    /// <param name="amount">the amount to be consumed. (for this game should be Time.deltatime always since it will reduce while using it)</param>
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

    /// <summary>
    /// Called when player wants to shoot the weapon, verification if player can shoot should be done before this being called.
    /// </summary>
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

    /// <summary>
    /// This will return the angle the particle system for shooting should be aimed at.
    /// </summary>
    /// <returns>float value to use on particleSystem.main.startRotation</returns>
    private float GetParticleSystemRotation()
    {
        float angle = 0f;
        if (_lookingDirection == Vector2.left) angle = 180f;
        else if (_lookingDirection == Vector2.down) angle = 90f;
        else if (_lookingDirection == Vector2.up) angle = 270f;
        return angle * Mathf.Deg2Rad;
    }
    
    /// <summary>
    /// Update the Stamina bar size shown on Canvas(screen)
    /// </summary>
    private void UpdateStaminaBar()
    {
        _staminaBar.localScale = new Vector3(StaminaPercent, 1f, 1f);
    }
    
    /// <summary>
    /// Timer to start the stamina regeneration, cancel it if player consumes again any.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RegenerateStamina()
    {
        yield return new WaitForSeconds(timeBeforeStaminaRegen);
        _isStaminaRegenerating = true;
    }

    /// <summary>
    /// Handle hiding logic, if if it's to enter hiding or exit hiding.
    /// </summary>
    private void HandleHiding()
    {
        if (_hidingObject == null)
        {
            TryToHideInObject();
        }
        else
        {
            TryToExitHidingObject();
        }
    }

    /// <summary>
    /// Called when the player presses SPACE key and is inside a hiding object,
    /// then will try to come out and enable everything it should.
    /// </summary>
    private void TryToExitHidingObject()
    {
        if (_hidingObject.TryGetComponent(out HidingObject hidingScript))
        {
            transform.position = hidingScript.PlayerExitHideSpot();
        }

        transform.GetComponent<SpriteRenderer>().enabled = true;
        _shootPos.parent.gameObject.SetActive(true);
        _hidingObject = null;
    }
    
    /// <summary>
    /// Called when player presses SPACE key, it will try to find the closest hiding
    /// that the player reaches, hide whatever it has to hide,
    /// and if it can't find any, nothing will happen.
    /// </summary>
    private void TryToHideInObject()
    {
        if (_hidingSpotsInReach.Count == 0) return; // No spots to hide close by.
        Vector3 playerLocation = transform.position;
        GameObject closest = _hidingSpotsInReach[0];
        // If the player is near more than 1 hiding object, this will pick the closest one.
        if (_hidingSpotsInReach.Count > 1)
        {
            float closestDist = Mathf.Infinity;
            foreach (GameObject hidingObject in _hidingSpotsInReach)
            {
                if (closest == hidingObject) continue;
                float distance = Vector3.Distance(playerLocation, hidingObject.transform.position);
                if (distance < closestDist)
                {
                    closestDist = distance;
                    closest = hidingObject;
                }
            }
        }
        TryStartStaminaRegen();
        if (closest.CompareTag("HidingObject") && closest.TryGetComponent(out HidingObject hidingScript))
        {
            hidingScript.PlayerHideHere(playerLocation);
            _hidingObject = closest;
            transform.position = closest.transform.position;
            _rb.linearVelocity = Vector2.zero;
            _shootPos.parent.gameObject.SetActive(false);
            transform.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    
    /// <summary>
    /// When player enters any 2D Trigger type collider
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // to add later: player's health reduction
        if (other.CompareTag("Bullet")) Destroy(other.gameObject);

        if (other.CompareTag("HidingObject") && other.TryGetComponent(out HidingObject hidingObject))
        {
            hidingObject.PlayerInBounds();
            _hidingSpotsInReach.Add(other.gameObject);
        }
        
    }

    /// <summary>
    /// When player exists any 2D Trigger type collider.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("HidingObject") && other.TryGetComponent(out HidingObject hidingObject))
        {
            hidingObject.PlayerOutOfBounds();
            _hidingSpotsInReach.Remove(other.gameObject);
        }
    }
}