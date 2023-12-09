using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public string unitName;

    public int unitLevel;

    public int damage;

    public int maxHP;

    public int currentHP;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
