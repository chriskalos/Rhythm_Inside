using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float smoothTime = 0.25f;
    private Vector3 _offset;
    private Vector3 _velocity;
    
    // Start is called before the first frame update
    void Start()
    {
        _offset = new Vector3(0f, 1f, -4f);
        _velocity = Vector3.zero;
    }

    void LateUpdate()
    {
        var position = playerTransform.position;
        Vector3 targetPosition = new Vector3(position.x + _offset.x, position.y + _offset.y, _offset.z);
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, smoothTime);
        transform.position = smoothedPosition;
    }
}
