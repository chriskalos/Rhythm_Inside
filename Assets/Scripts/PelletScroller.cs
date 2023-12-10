using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PelletScroller : MonoBehaviour
{
    [SerializeField] private float bpm = 125f; // Beats per minute
    [SerializeField] private float pelletSpeed = 30f;
    [SerializeField] private bool hasStarted;
    private float _bpmTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        _bpmTransform = bpm / pelletSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasStarted)
        {
            if (Input.anyKeyDown)
            {
                hasStarted = true;
            }
        }
        else
        {
            transform.position -= new Vector3(_bpmTransform * Time.deltaTime, 0f, 0f);
        }
    }
}
