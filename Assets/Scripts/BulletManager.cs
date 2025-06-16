using UnityEngine;

public class BulletManager : MonoBehaviour
{

    private float _bulletSpeed = 250f;
    private Rigidbody2D _rb;
    private Vector2 _direction = Vector2.right;
    public Vector2 SetDirection(Vector2 direction) => _direction = direction;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 3f);
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _direction * _bulletSpeed;
    }
}
