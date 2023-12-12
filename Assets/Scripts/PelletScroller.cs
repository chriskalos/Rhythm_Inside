using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PelletScroller : MonoBehaviour
{
    public BattleManager battleManager;
    [SerializeField] private GameObject rhythmAttackPanel;
    [SerializeField] private GameObject pelletPrefab;
    [SerializeField] private HitButton hitButton;

    public int damage;
    public int pelletCount;

    private int _spawnedPellets;
    
    // Start is called before the first frame update
    void Start()
    {
        pelletCount = 0;
        _spawnedPellets = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /// <summary>
    /// Spawns a pellet at the right edge of the screen.
    ///
    /// This function is called by the BeatManager
    /// every beat.
    /// </summary>
    public void SpawnPellet()
    {
        if (rhythmAttackPanel.activeSelf && _spawnedPellets < 8)
        {
            GameObject instance = Instantiate(pelletPrefab, new Vector3(610f, 0f, -10f), Quaternion.identity);
            instance.transform.SetParent(transform, false);
            _spawnedPellets++;
        }
    }

    // This is a crude way of moving the pellets to the left smoothly.
    // It was the only consistent way to keep them synced with the beat.
    
    /// <summary>
    /// Moves all pellets to the left by 5 units.
    /// This function is called by the BeatManager
    /// every 128th of a beat.
    /// </summary>
    public void MoveToTheBeat()
    {
        if (rhythmAttackPanel.activeSelf)
        {
            foreach (BeatPellet pellet in GetComponentsInChildren<BeatPellet>())
            {
                pellet.transform.localPosition += new Vector3(-5f, 0f, 0f);
            }
        }
    }

    /// <summary>
    /// When a pellet is hit, count it and increment damage exponentially.
    /// If the pellet count is 8, end the attack.
    /// </summary>
    public void HitPellet()
    {
        pelletCount++;

        if (pelletCount >= 8)
        {
            EndAttack();
        }
    }
    
    /// <summary>
    /// Ends the attack and destroys all spawned pellets.
    /// Also calculates damage based on the player's level.
    /// </summary>
    public void EndAttack()
    {
        damage = Mathf.RoundToInt(pelletCount * Mathf.Pow(GameManager.Instance.playerLevel, 1.15f)); // Scale damage by player level
        // Debug.Log("Damage dealt by " + battleManager.playerUnit.unitName + ": " + damage);
        battleManager.EndAttack(); // End the attack from the BattleManager
        
        // Destroy all spawned pellets
        foreach (BeatPellet pellet in GetComponentsInChildren<BeatPellet>())
        {
            Destroy(pellet.gameObject);
        }
        
        // Reset spawned pellet count
        _spawnedPellets = 0;
    }
}
