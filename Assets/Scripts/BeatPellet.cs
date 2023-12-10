using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatPellet : MonoBehaviour
{
    [SerializeField] private bool canBePressed;
    [SerializeField] private KeyCode keyToPress;
    
    // Start is called before the first frame update
    void Start()
    {
        // todo: import BattleManager
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(keyToPress) || Input.GetKeyDown(KeyCode.Space)) && canBePressed)
        {
            gameObject.SetActive(false);
            // Debug.Log("Hit!");
            // todo: BattleManager to keep score
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Activator")
        {
            canBePressed = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Activator")
        {
            canBePressed = false;
        }
    }
}
