using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int playerLevel { get; private set; }
    public int playerXP { get; private set; }
    public int playerCurrentHP { get; private set; }
    
    public int playerXPForNextLevel { get; set; }

    public HUD hud;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        playerLevel = 5;
        UpdatePlayerState(playerLevel, XPForNextLevel(playerLevel-1), Mathf.RoundToInt(30 * Mathf.Pow(1.15f, playerLevel - 1)));
        hud.UpdateUI();
        Debug.Log("GameManager instantiated with:\n" + "playerLevel: " + playerLevel + ", playerXP: " + playerXP + ", playerCurrentHP: " + playerCurrentHP);
    }

    public void StartBattle()
    {
        // Load the battle scene
        SceneManager.LoadScene("Battle", LoadSceneMode.Single);
    }

    public void EndBattle()
    {
        // Unload the battle scene
        SceneManager.LoadScene("Overworld", LoadSceneMode.Single);
    }

    public void UpdatePlayerState(int level, int xp, int currentHP)
    {
        playerLevel = level;
        playerXP = xp;
        playerCurrentHP = currentHP;
        playerXPForNextLevel = XPForNextLevel(playerLevel);
    }
    
    private int XPForNextLevel(int level) // Pass
    {
        // Debug.Log("### GameManager.Instance.XPForNextLevel ###");
        // Debug.Log("### level passed: " + level + " ###");
        if (playerLevel < 100) // Max level is 100
        {
            if (level != 0)
            {
                // Calculate the XP required for the next level
                int xpNeeded = Mathf.RoundToInt(100 * Mathf.Pow(1.2f, level + 1));
                // Debug.Log("### xpNeeded: " + xpNeeded + " ###");
                return xpNeeded;
            }
        }
        return 0;
    }
}