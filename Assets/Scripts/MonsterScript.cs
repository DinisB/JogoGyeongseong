using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MonsterScript : MonoBehaviour
{

    [SerializeField] private Transform pathObject;

    [SerializeField] private float moveSpeed = 30f;

    private Rigidbody2D _rb;

    private Vector3[] _paths;
    private int _nextPath = -1;
    private Vector3 _direction = Vector3.zero;
    private int life = 50;

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Bullet"))
        {
            life -= 10;
            if (life <= 0) Destroy(gameObject);
        }
    }
}
