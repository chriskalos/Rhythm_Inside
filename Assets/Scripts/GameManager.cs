using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GameManager is a singleton class that manages game states and player data across different scenes.
/// </summary>
public class GameManager : MonoBehaviour
{
    // Singleton instance of GameManager
    public static GameManager Instance { get; private set; }

    // Player stats
    public int playerLevel { get; private set; }
    public int playerXP { get; private set; }
    public int playerCurrentHP { get; private set; }
    
    // XP required for the player to reach the next level
    public int playerXPForNextLevel { get; set; }

    // Reference to the HUD (Heads-Up Display) for UI updates
    public HUD hud;

    private void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene changes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    void Start()
    {
        // Initialize player state
        playerLevel = 5;
        UpdatePlayerState(playerLevel, XPForNextLevel(playerLevel-1), Mathf.RoundToInt(30 * Mathf.Pow(1.15f, playerLevel - 1)));
        hud.UpdateUI(); // Update HUD with the player's initial state
    }

    /// <summary>
    /// Loads the battle scene.
    /// </summary>
    public void StartBattle()
    {
        SceneManager.LoadScene("Battle", LoadSceneMode.Single);
    }

    /// <summary>
    /// Exits the battle scene and returns to the overworld.
    /// </summary>
    public void EndBattle()
    {
        SceneManager.LoadScene("Overworld", LoadSceneMode.Single);
    }

    /// <summary>
    /// Updates the player's state including level, XP, and current HP.
    /// </summary>
    /// <param name="level">Player's level.</param>
    /// <param name="xp">Player's current XP.</param>
    /// <param name="currentHP">Player's current HP.</param>
    public void UpdatePlayerState(int level, int xp, int currentHP)
    {
        playerLevel = level;
        playerXP = xp;
        playerCurrentHP = currentHP;
        playerXPForNextLevel = XPForNextLevel(playerLevel);
    }
    
    /// <summary>
    /// Calculates the XP required for the player to reach the next level.
    /// </summary>
    /// <param name="level">Current level of the player.</param>
    /// <returns>The XP needed for the next level.</returns>
    private int XPForNextLevel(int level) 
    {
        if (playerLevel < 100) // Ensure level does not exceed max level (100)
        {
            if (level != 0)
            {
                return Mathf.RoundToInt(100 * Mathf.Pow(1.2f, level + 1)); // XP calculation formula
            }
        }
        return 0; // Return 0 if max level is reached
    }
}