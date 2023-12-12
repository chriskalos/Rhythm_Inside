using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// HUD class for managing the display of player stats on the game's UI.
/// </summary>
public class HUD : MonoBehaviour
{
    // UI elements for displaying player stats
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private TextMeshProUGUI xpForNextLevelText;
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateUI(); // Update the UI elements with current player stats on start
    }
    
    /// <summary>
    /// Updates the HUD with the latest player stats.
    /// </summary>
    public void UpdateUI()
    {
        // Retrieve player stats from GameManager and update each UI text element
        // Again I know this is horrible practice but Unity forces me into spaghetti code
        hpText.text = "HP: " + GameManager.Instance.playerCurrentHP;
        levelText.text = "Level: " + GameManager.Instance.playerLevel;
        xpText.text = "XP in level: " + GameManager.Instance.playerXP;
        xpForNextLevelText.text = "XP for next level: " + GameManager.Instance.playerXPForNextLevel;
    }
}
