using System;
using UnityEngine;

public class PlayerManager2 : MonoBehaviour
{

    private Camera _cam;
    private int _camPos = 0;
    
    void Start()
    {
        _cam = Camera.main;
        _cam.transform.position = GetNextCameraPos(-1);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("CameraPos"))
        {
            int colliderID = int.Parse(other.gameObject.name.Substring("Collider".Length));
            _cam.transform.position = GetNextCameraPos(colliderID);
        }
    }

    private Vector3 GetNextCameraPos(int colliderID)
    {
        Vector3[] camPos = new Vector3[] { new Vector3(0f, 0f, -10f), new Vector3(288f, 288f, -10f) };
        if (colliderID == 0 && _camPos == 0)
        {
            // going up
            _camPos = 1;
            return camPos[1];
        }
        // starting point
        _camPos = 0;
        return camPos[0];
    }
}
