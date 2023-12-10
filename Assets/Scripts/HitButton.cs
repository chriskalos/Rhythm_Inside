using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitButton : MonoBehaviour
{
    private Image _hitButtonImage;
    [SerializeField] private Sprite defaultImage;
    [SerializeField] private Sprite pressedImage;
    
    [SerializeField] private KeyCode keyToPress;
    
    // Start is called before the first frame update
    void Start()
    {
        _hitButtonImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(keyToPress))
        {
            _hitButtonImage.sprite = pressedImage;
        }
        
        if (Input.GetKeyUp(keyToPress))
        {
            _hitButtonImage.sprite = defaultImage;
        }
    }
    
    // todo: If the player presses the button while the pellet is NOT in the activator,
    // todo: the rhythm attack ends.
}
