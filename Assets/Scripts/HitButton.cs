using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitButton : MonoBehaviour
{
    [SerializeField] private PelletScroller pelletScroller;
    private Image _hitButtonImage;
    [SerializeField] private Sprite defaultImage;
    [SerializeField] private Sprite pressedImage;
    
    [SerializeField] private KeyCode keyToPress;

    public bool canBePressed;
    public bool isPressed;
    
    // Start is called before the first frame update
    void Awake()
    {
        isPressed = false;
        _hitButtonImage = GetComponent<Image>();
        _hitButtonImage.sprite = defaultImage;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(keyToPress))
        {
            _hitButtonImage.sprite = pressedImage;
            isPressed = true;
        }
        
        if (Input.GetKeyUp(keyToPress))
        {
            _hitButtonImage.sprite = defaultImage;
            isPressed = false;
        }
        
        // If the player presses the button while there is no pellet, end the attack.
        if (!canBePressed && isPressed)
        {
            isPressed = false;
            pelletScroller.EndAttack();
        }
        
        Debug.Log(canBePressed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pellet")
        {
            canBePressed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Pellet")
        {
            canBePressed = false;
        }
    }
}
