using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CameraTracking is responsible for smoothly following the player in the game.
/// </summary>
public class CameraTracking : MonoBehaviour
{ // I adapted this from the wizard game!
    [SerializeField] private Transform playerTransform; 
    [SerializeField] private float smoothTime = 0.25f;

    private Vector3 _offset;
    private Vector3 _velocity;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the offset and velocity
        _offset = new Vector3(0f, 1f, -4f); // Sets the camera offset relative to the player
        _velocity = Vector3.zero; // Initialize camera velocity to zero
    }

    void LateUpdate()
    {
        // Calculate the target position based on the player's position and the offset
        var position = playerTransform.position;
        Vector3 targetPosition = new Vector3(position.x + _offset.x, position.y + _offset.y, _offset.z);

        // Smoothly move the camera towards the target position
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, smoothTime);
        
        // Update the camera's position
        transform.position = smoothedPosition;
    }
}
