using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the behavior of the hit button in the rhythm game, detecting player inputs and managing button state.
/// </summary>
public class HitButton : MonoBehaviour
{
    [SerializeField] private PelletScroller pelletScroller;
    
    private Image _hitButtonImage;
    [SerializeField] private Sprite defaultImage;
    [SerializeField] private Sprite pressedImage;
    [SerializeField] private KeyCode keyToPress;
    
    public bool canBePressed;
    public bool isPressed;
    
    // Called when the script instance is being loaded
    void OnEnable()
    {
        // Initialize button state and visuals
        canBePressed = false;
        isPressed = false;
        _hitButtonImage = GetComponent<Image>();
        _hitButtonImage.sprite = defaultImage;
    }

    // Update is called once per frame
    void Update()
    {
        // Detect if the assigned key is pressed down
        if (Input.GetKeyDown(keyToPress))
        {
            _hitButtonImage.sprite = pressedImage;
            isPressed = true;
        }
        
        // Detect if the assigned key is released
        if (Input.GetKeyUp(keyToPress))
        {
            _hitButtonImage.sprite = defaultImage;
            isPressed = false;
        }
        
        // Check for a missed hit (button pressed without a pellet)
        if (!canBePressed && isPressed)
        {
            isPressed = false;
            pelletScroller.EndAttack();
        }
    }

    // Called when a collider enters the trigger area of this button
    private void OnTriggerEnter(Collider other)
    {
        // Enable the button when a pellet enters the trigger area
        if (other.tag == "Pellet")
        {
            canBePressed = true;
        }
    }

    // Called when a collider exits the trigger area of this button
    private void OnTriggerExit(Collider other)
    {
        // Disable the button when a pellet exits the trigger area
        if (other.tag == "Pellet")
        {
            canBePressed = false;
        }
    }
}
