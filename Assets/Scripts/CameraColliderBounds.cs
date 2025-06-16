using System;
using DefaultNamespace;
using UnityEngine;

public class CameraColliderBounds : MonoBehaviour
{

    private float _colliderThickness = 16f;
    private Camera _cam;
    private BoxCollider cameraCollider;

    private GameObject topCollider, bottomCollider, leftCollider, rightCollider;
    private BoxCollider2D topBox, bottomBox, leftBox, rightBox;

    private void Awake()
    {
        _cam = Camera.main;
    }

    void Start()
    {
        CreateBoundaryColliders();
        UpdateColliderPositions();
    }

    private void CreateBoundaryColliders()
    {
        GameObject boundaryParent = new GameObject("Camera Boundaries");
        boundaryParent.transform.SetParent(transform);

        // Top collider
        topCollider = new GameObject("Top Boundary");
        topCollider.AddComponent<CameraColliderManager>();
        topCollider.transform.SetParent(boundaryParent.transform);
        topBox = topCollider.AddComponent<BoxCollider2D>();
        topBox.excludeLayers = LayerMask.GetMask("Default");

        // Bottom collider
        bottomCollider = new GameObject("Bottom Boundary");
        bottomCollider.AddComponent<CameraColliderManager>();
        bottomCollider.transform.SetParent(boundaryParent.transform);
        bottomBox = bottomCollider.AddComponent<BoxCollider2D>();
        bottomBox.excludeLayers = LayerMask.GetMask("Default");

        // Left collider
        leftCollider = new GameObject("Left Boundary");
        leftCollider.AddComponent<CameraColliderManager>();
        leftCollider.transform.SetParent(boundaryParent.transform);
        leftBox = leftCollider.AddComponent<BoxCollider2D>();
        leftBox.excludeLayers = LayerMask.GetMask("Default");

        // Right collider
        rightCollider = new GameObject("Right Boundary");
        rightCollider.AddComponent<CameraColliderManager>();
        rightCollider.transform.SetParent(boundaryParent.transform);
        rightBox = rightCollider.AddComponent<BoxCollider2D>();
        rightBox.excludeLayers = LayerMask.GetMask("Default");
    }

    public void UpdateColliderPositions()
    {
        float cameraHeight = _cam.orthographicSize * 2f;
        float cameraWidth = cameraHeight * _cam.aspect;

        Vector3 cameraPos = _cam.transform.position;

        // Calculate half dimensions
        float halfWidth = cameraWidth * 0.5f;
        float halfHeight = cameraHeight * 0.5f;
        float halfThickness = _colliderThickness * 0.5f;

        // Top collider
        topBox.size = new Vector2(cameraWidth + _colliderThickness * 2, _colliderThickness);
        topCollider.transform.position =
            new Vector3(cameraPos.x, cameraPos.y + halfHeight + halfThickness, cameraPos.z);

        // Bottom collider
        bottomBox.size = new Vector2(cameraWidth + _colliderThickness * 2, _colliderThickness);
        bottomCollider.transform.position =
            new Vector3(cameraPos.x, cameraPos.y - halfHeight - halfThickness, cameraPos.z);

        // Left collider
        leftBox.size = new Vector2(_colliderThickness, cameraHeight);
        leftCollider.transform.position =
            new Vector3(cameraPos.x - halfWidth - halfThickness, cameraPos.y, cameraPos.z);

        // Right collider
        rightBox.size = new Vector2(_colliderThickness, cameraHeight);
        rightCollider.transform.position =
            new Vector3(cameraPos.x + halfWidth + halfThickness, cameraPos.y, cameraPos.z);
    }
}