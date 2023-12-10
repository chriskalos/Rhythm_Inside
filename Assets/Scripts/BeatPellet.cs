using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatPellet : MonoBehaviour
{
    [SerializeField] private bool canBePressed;
    public KeyCode keyToPress;
    
    private PelletScroller _pelletScroller;
    
    // Start is called before the first frame update
    void Start()
    {
        _pelletScroller = GetComponentInParent<PelletScroller>();
        // todo: import BattleManager
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(keyToPress) && canBePressed)
        {
            _pelletScroller.HitPellet();
            Destroy(gameObject);
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
            _pelletScroller.battleManager.EndAttack();
        }
    }
}
