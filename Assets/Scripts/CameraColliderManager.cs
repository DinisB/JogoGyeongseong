using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class CameraColliderManager : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Bullet"))
            {
                Destroy(other.gameObject);
            }
        }
    }
}