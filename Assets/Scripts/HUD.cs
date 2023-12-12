using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private TextMeshProUGUI xpForNextLevelText;
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUI()
    {
        hpText.text = "HP: " + GameManager.Instance.playerCurrentHP;
        levelText.text = "Level: " + GameManager.Instance.playerLevel;
        xpText.text = "XP in level: " + GameManager.Instance.playerXP;
        xpForNextLevelText.text = "XP for next level: " + GameManager.Instance.playerXPForNextLevel;
    }
}
