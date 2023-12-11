using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    public string unitName;

    public int unitLevel;

    public int unitMaxLevel = 100;

    public int baseHP = 30;
    
    public int maxHP;

    public float hpGrowthRate = 1.15f;

    public int currentHP;
    
    public int xp;
    
    // Experience required for the first level up
    public int baseXP = 100;

    // Exponential growth rate for XP requirements
    public float xpGrowthRate = 1.5f;
    
    
    // Stats to calculate how much XP will be gained when this unit is defeated
    public float givenXPGrowthRate = 1.2f; // 20% increase per level
    public int givenXP;
    
    
    // Awake is called before Start
    void Awake()
    {
        UpdateStats();
        currentHP = maxHP;
    }

    void Start()
    {
        if (currentHP <= 0)
        {
            ResetHP();
        }
    }

    public void UpdateStats()
    {
        maxHP = Mathf.RoundToInt(baseHP * Mathf.Pow(hpGrowthRate, unitLevel - 1)); // 15% increase per level
        givenXP = Mathf.RoundToInt(baseXP * Mathf.Pow(xpGrowthRate, unitLevel - 1)); // 20% increase per level
    }
    
    public void GainXP(int amount)
    {
        if (unitLevel < unitMaxLevel)
        {
            xp += amount;
            if (xp >= XPForNextLevel(unitLevel))
            {
                LevelUp();
            }
        }
    }
    
    private void LevelUp()
    {
        unitLevel++;
        xp -= XPForNextLevel(unitLevel - 1); // Subtract XP required for the level-up
        UpdateStats(); // Update stats based on new level
        currentHP = maxHP; // Fully heal the unit
    }
    
    private int XPForNextLevel(int level)
    {
        if (unitLevel < unitMaxLevel)
        {
            // Calculate the XP required for the next level
            return Mathf.RoundToInt(baseXP * Mathf.Pow(xpGrowthRate, level - 1));
        }

        return 0;
    }
    
    public void ResetHP()
    {
        currentHP = maxHP;
    }
}
