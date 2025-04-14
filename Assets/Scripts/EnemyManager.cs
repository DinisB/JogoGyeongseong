using System;
using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    [SerializeField] private GameObject bulletPrefab;
    // WE NEED TO UPDATE LATER SHOOT POS FOR DOWN AND UP DEPENDING ON THE SPRITES USED, NEED OFFSET FOR IT.
    [SerializeField] private Transform shootPos;
    [SerializeField] private Transform pathObject;

    [SerializeField] private float moveSpeed = 30f;
    
    private Rigidbody2D _rb;
    
    // WILL BE USED IN FUTURE
    // 0 = Nothing found, walk
    // 1 = In combat with player, dont walk
    private int _state = 0;

    private Vector3[] _paths;
    private int _nextPath = -1;
    private Vector3 _direction = Vector3.zero;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        // //////////
        // PATH SETUP
        // //////////
        _paths = new Vector3[pathObject.childCount];
        int counter = 0;
        foreach (Transform each in pathObject)
        {
            _paths[counter] = each.position;
            counter++;
        }
        // Delete so we keep the minimum gameObjects in the scene as necessary, were only needed for setup locations in local variable.
        // Kept alive for testing purposes to check the threshhold
        // Destroy(pathObject.gameObject);

        transform.position = _paths[0];
        _nextPath = 1;
        _direction = (_paths[_nextPath] - transform.position).normalized;
        
        InvokeRepeating(nameof(InvokeRayCasts), 1f, 1f);
    }

    private float _elapsedUpdateWalk = 0f;
    private void Update()
    {
        if (_nextPath < 0) return; // not move while it's being setup.
        _elapsedUpdateWalk += Time.deltaTime;
        _rb.linearVelocity = _direction * moveSpeed;
        
        if (_elapsedUpdateWalk >= 0.1f) UpdateWalk();
    }

    private void UpdateWalk()
    {
        float distance = Vector3.Distance(transform.position, _paths[_nextPath]);
        //Debug.Log($"{Vector3.Distance(transform.position, _paths[_nextPath])}");
        if (distance < 5f)
        {
            _nextPath += 1;
            if (_nextPath >= _paths.Length) _nextPath = 0;
            _direction = (_paths[_nextPath] - transform.position).normalized;
        }
    }
    
    private void InvokeRayCasts()
    {
        RaycastHit2D hit = Physics2D.Raycast(shootPos.position, Vector2.right, Mathf.Infinity, LayerMask.GetMask("Default"));
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            Debug.Log("Found player");
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot()
    {
        // Change later to calculate diff between player and enemy, normalize and use that as the direction for the bullet.
        Vector2[] directionsOffSets = new[] { new Vector2(1f, 0f), new Vector2(1f, 0.03f), new Vector2(1f, -0.03f) };
        foreach (Vector2 direction in directionsOffSets)
        {
            GameObject spawnedBullet = Instantiate(bulletPrefab, shootPos.position, Quaternion.identity);
            if (spawnedBullet.TryGetComponent(out BulletManager bulletManager))
            {
                bulletManager.SetDirection(direction);
            }
            yield return new WaitForSeconds(0.05f);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Bullet")) return;
        Destroy(other.gameObject);
        Destroy(gameObject);
    }
}
