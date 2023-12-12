using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages the timing of beats in the rhythm game.
/// </summary>
public class BeatManager : MonoBehaviour
{
    [SerializeField] private float bpm; // Beats per minute for the rhythm
    [SerializeField] private AudioSource audioSource; // Audio source for the rhythm music
    [SerializeField] private Intervals[] intervals; // Array of intervals for triggering events
    
    // Start is called before the first frame update
    void Start()
    {
        // Initialization can be added here if necessary
    }

    // Update is called once per frame
    void Update()
    {
        // Iterate through each interval to check if a new beat interval is reached
        foreach (Intervals interval in intervals)
        {
            // Calculate the current time position within the interval
            float sampledTime = (audioSource.timeSamples / (audioSource.clip.frequency * interval.GetIntervalLength(bpm)));
            
            // Check and trigger events if a new interval is reached
            interval.CheckForNewInterval(sampledTime);
        }
    }
}

/// <summary>
/// Represents a beat interval in the rhythm game.
/// </summary>
[System.Serializable]
public class Intervals
{
    [SerializeField] private float steps; // Number of steps within each beat
    [SerializeField] private UnityEvent trigger; // Event to trigger at each interval
    private int _lastInterval; // Keeps track of the last interval that triggered an event

    /// <summary>
    /// Calculates the length of the interval based on the BPM and steps.
    /// </summary>
    /// <param name="bpm">Beats per minute of the rhythm.</param>
    /// <returns>The length of the interval.</returns>
    public float GetIntervalLength(float bpm)
    {
        return 60f / (bpm * steps); // Calculate interval length
    }

    /// <summary>
    /// Checks if the current interval has changed and triggers an event if so.
    /// </summary>
    /// <param name="interval">The current interval position.</param>
    public void CheckForNewInterval(float interval)
    {
        // Check if the interval has changed
        if (Mathf.FloorToInt(interval) != _lastInterval)
        {
            _lastInterval = Mathf.FloorToInt(interval); // Update the last interval
            trigger.Invoke(); // Trigger the event
        }
    }
}
