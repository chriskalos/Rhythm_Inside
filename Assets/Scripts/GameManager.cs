using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int playerLevel { get; private set; }
    public int playerXP { get; private set; }
    public int playerCurrentHP { get; private set; }

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
        UpdatePlayerState(1, 0, 30);
        Debug.Log("GameManager instantiated with:\n" + "playerLevel: " + playerLevel + ", playerXP: " + playerXP + ", playerCurrentHP: " + playerCurrentHP);
    }

    public void StartBattle()
    {
        // Save current state, if necessary
        SavePlayerState();

        // Load the battle scene
        SceneManager.LoadScene("Battle", LoadSceneMode.Single);
    }

    public void EndBattle()
    {
        // Restore the player's state, if necessary
        RestorePlayerState();

        // Unload the battle scene
        SceneManager.LoadScene("Overworld", LoadSceneMode.Single);
    }

    public void UpdatePlayerState(int level, int xp, int currentHP)
    {
        playerLevel = level;
        playerXP = xp;
        playerCurrentHP = currentHP;
    }

    private void SavePlayerState()
    {
        // Implement logic to save player's current state
    }

    private void RestorePlayerState()
    {
        // Implement logic to restore player's state
    }
}