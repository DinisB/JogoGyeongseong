using System;
using UnityEngine;

public class PlayerManager2 : MonoBehaviour
{

    private Camera _cam;
    
    void Start()
    {
        _cam = Camera.main;
        FixCameraPos(null);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("CameraPos")) FixCameraPos(other);
    }

    private Collider2D _lastRoom;
    private void FixCameraPos(Collider2D other)
    {
        if (other == null)
        {
            transform.position = new Vector3(0f, 0f, 0f);
            _cam.transform.position = new Vector3(0f, 0f, -10f);
            return;
        }
        if (_lastRoom != null)
        {
            Vector3 lastPos = _lastRoom.transform.position;
            Vector3 nowPos = other.transform.position;
            float x = nowPos.x - lastPos.x;
            float y = nowPos.y - lastPos.y;
            if (y > 0) transform.position += new Vector3(0f, 32f, 0f);
            if (y < 0) transform.position -= new Vector3(0f, 32f, 0f);
            if (x > 0) transform.position += new Vector3(22f, 0, 0f);
            if (x < 0) transform.position -= new Vector3(22f, 0, 0f);
        }
        _cam.transform.position = other.gameObject.transform.position - new Vector3(0f, 0f, 10f);
        _lastRoom = other;
    }
}
