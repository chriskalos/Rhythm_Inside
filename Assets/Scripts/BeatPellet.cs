using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an individual pellet in the rhythm attack system.
/// Handles player input and interaction with the activator.
/// </summary>
public class BeatPellet : MonoBehaviour
{
    [SerializeField] private bool canBePressed;
    public KeyCode keyToPress;

    private PelletScroller _pelletScroller;

    // Start is called before the first frame update
    void Start()
    {
        _pelletScroller = GetComponentInParent<PelletScroller>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the correct key is pressed and the pellet can be pressed
        if (Input.GetKeyDown(keyToPress) && canBePressed)
        {
            _pelletScroller.HitPellet();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called when the pellet enters the HitButton's trigger area.
    /// </summary>
    /// <param name="other">The collider that this pellet has entered.</param>
    private void OnTriggerEnter(Collider other)
    {
        // If the pellet enters the activator, it can be pressed
        if (other.tag == "Activator")
        {
            canBePressed = true;
        }
    }

    /// <summary>
    /// Called when the pellet exits the HitButton's trigger area.
    /// </summary>
    /// <param name="other">The collider that this pellet has exited.</param>
    private void OnTriggerExit(Collider other)
    {
        // If the pellet exits the activator, it can no longer be pressed
        if (other.tag == "Activator")
        {
            canBePressed = false;
            _pelletScroller.EndAttack(); // End the attack phase if a pellet is missed
        }
    }
}