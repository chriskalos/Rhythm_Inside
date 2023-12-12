using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the spawning, movement, and interaction of pellets in the rhythm attack phase.
/// </summary>
public class PelletScroller : MonoBehaviour
{
    public BattleManager battleManager; // Reference to the BattleManager for interaction with the battle system
    [SerializeField] private GameObject rhythmAttackPanel; // The panel where the rhythm attack takes place
    [SerializeField] private GameObject pelletPrefab; // Prefab for the pellet objects
    [SerializeField] private HitButton hitButton; // Reference to the HitButton to detect player input

    public int damage; // Accumulated damage based on successful hits
    public int pelletCount; // Count of pellets successfully hit

    private int _spawnedPellets; // Counter for the total number of pellets spawned

    // Start is called before the first frame update
    void Start()
    {
        pelletCount = 0;
        _spawnedPellets = 0;
    }

    /// <summary>
    /// Spawns a pellet at a specified position. Called at regular intervals by the BeatManager.
    /// </summary>
    public void SpawnPellet()
    {
        if (rhythmAttackPanel.activeSelf && _spawnedPellets < 8) // Limit the maximum number of pellets
        {
            // Create a pellet instance and position it within the rhythm attack panel
            GameObject instance = Instantiate(pelletPrefab, new Vector3(610f, 0f, -10f), Quaternion.identity);
            instance.transform.SetParent(transform, false);
            _spawnedPellets++; // Increment the count of spawned pellets
        }
    }

    /// <summary>
    /// Moves all pellets to the left. Called at each fraction of a beat by the BeatManager.
    /// </summary>
    public void MoveToTheBeat()
    { // This was the most effective way I could find to move the pellets on time.
        if (rhythmAttackPanel.activeSelf)
        {
            // Move each pellet to the left
            foreach (BeatPellet pellet in GetComponentsInChildren<BeatPellet>())
            {
                pellet.transform.localPosition += new Vector3(-5f, 0f, 0f);
            }
        }
    }

    /// <summary>
    /// Called when a pellet is successfully hit. Increments the pellet count and ends the attack if necessary.
    /// </summary>
    public void HitPellet()
    {
        pelletCount++; // Increment the count of successfully hit pellets

        if (pelletCount >= 8) // Check if all pellets have been hit
        {
            EndAttack(); // End the attack if all pellets are hit
        }
    }

    /// <summary>
    /// Ends the rhythm attack phase, calculates damage, and cleans up pellets.
    /// </summary>
    public void EndAttack()
    {
        // Calculate damage based on the number of pellets hit and the player's level
        damage = Mathf.RoundToInt(pelletCount * Mathf.Pow(GameManager.Instance.playerLevel, 1.15f));

        battleManager.EndAttack(); // Notify the BattleManager that the attack phase is over

        // Destroy all remaining pellets
        foreach (BeatPellet pellet in GetComponentsInChildren<BeatPellet>())
        {
            Destroy(pellet.gameObject);
        }

        _spawnedPellets = 0; // Reset the counter for spawned pellets
    }
}
