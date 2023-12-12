using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a unit in the game, handling its stats like health points, experience, and level.
/// </summary>
public class Unit : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer; // Renderer for the unit's sprite
    public string unitName; // Name of the unit

    public int unitLevel; // Current level of the unit
    public int unitMaxLevel = 100; // Maximum level the unit can achieve

    public int baseHP = 30; // Base health points of the unit
    public int maxHP; // Maximum health points of the unit
    public float hpGrowthRate = 1.15f; // Rate at which health points increase per level
    public int currentHP; // Current health points of the unit

    public int xp; // Current experience points of the unit
    public int totalXp; // Total accumulated experience points (not used in this script)

    // Experience required for the first level up
    public int baseXP = 100;

    // Exponential growth rate for XP requirements
    public float xpGrowthRate = 1.5f;

    // Stats to calculate how much XP will be gained when this unit is defeated
    public float givenXPGrowthRate = 1.2f; // 20% increase per level
    public int givenXP; // XP given to the opponent when this unit is defeated

    // Awake is called before Start
    void Awake()
    {
        UpdateStats(); // Update the stats of the unit based on its level
        currentHP = maxHP; // Set current HP to maximum at start
    }

    void Start()
    {
        // Ensure the unit has health points at the start
        if (currentHP <= 0)
        {
            ResetHP();
        }
    }

    /// <summary>
    /// Updates the unit's stats based on its current level.
    /// </summary>
    public void UpdateStats()
    {
        maxHP = Mathf.RoundToInt(baseHP * Mathf.Pow(hpGrowthRate, unitLevel - 1)); // Calculate max HP based on level
        givenXP = Mathf.RoundToInt(baseXP * Mathf.Pow(xpGrowthRate, unitLevel - 1)); // Calculate XP given to opponent
    }

    /// <summary>
    /// Adds experience points to the unit and handles level up if necessary.
    /// </summary>
    /// <param name="amount">Amount of XP to add.</param>
    public void GainXP(int amount)
    {
        if (unitLevel < unitMaxLevel)
        {
            xp += amount; // Add the XP
            // Check for level up
            while (xp >= XPForNextLevel(unitLevel) && unitLevel < unitMaxLevel)
            {
                LevelUp(); // Level up the unit if enough XP is gained
            }
        }
    }

    /// <summary>
    /// Handles the unit's level up process.
    /// </summary>
    private void LevelUp()
    {
        unitLevel++; // Increase the unit's level
        xp -= XPForNextLevel(unitLevel - 1); // Subtract XP required for the level-up
        UpdateStats(); // Update the unit's stats based on new level
        currentHP = maxHP; // Fully heal the unit on level up
    }

    /// <summary>
    /// Calculates the XP required for the next level.
    /// </summary>
    /// <param name="level">Current level of the unit.</param>
    /// <returns>XP required for the next level.</returns>
    private int XPForNextLevel(int level)
    { // I know this is really bad because I have one in Unit but it's 2 AM I'm too tired to think of a better way
        if (unitLevel < unitMaxLevel)
        {
            // Calculate the XP required for the next level using the growth rate
            int xpNeeded = Mathf.RoundToInt(100 * Mathf.Pow(1.2f, level + 1));
            return xpNeeded;
        }
        return 0; // Return 0 if max level is reached or exceeded
    }

    /// <summary>
    /// Resets the unit's current HP to its maximum HP.
    /// </summary>
    public void ResetHP()
    {
        currentHP = maxHP;
    }
}
