using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Enum representing the different states of a battle.
/// </summary>
public enum BattleState
{
    START,      
    PLAYERTURN, 
    ENEMYTURN,  
    WON,        
    LOST        
}

/// <summary>
/// The battle manager handles everything that happens during a battle.
/// </summary>
public class BattleManager : MonoBehaviour
{
    // Class fields and serialized properties for the battle system

    // State of the current battle, defined by the BattleState enum
    [SerializeField] private BattleState state;

    // Prefabs for player and enemy characters
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;

    // UI panel for rhythm-based attack mechanics
    [SerializeField] private GameObject rhythmAttackPanel;

    // Holder for in-game pellets used in rhythm attacks
    [SerializeField] private GameObject pelletHolder;

    // Script to control the movement and behavior of pellets
    private PelletScroller _pelletScroller;

    // Unit scripts attached to the player and enemy, public for access by PelletScroller
    public Unit playerUnit;
    public Unit enemyUnit;

    // UI text elements for displaying dialogue and character information
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private TextMeshProUGUI playerLevelText;
    [SerializeField] private TextMeshProUGUI enemyLevelText;
    [SerializeField] private TextMeshProUGUI playerHpStatus;
    [SerializeField] private TextMeshProUGUI enemyHpStatus;

    // Sliders for visual representation of player's and enemy's HP
    [SerializeField] private Slider playerHpSlider;
    [SerializeField] private Slider enemyHpSlider;

    // Panel containing the in-battle action buttons
    [SerializeField] private GameObject buttonsPanel;

    // Canvas for displaying the game over screen
    [SerializeField] private Canvas gameOverCanvas;

    // Text element on the game over screen
    [SerializeField] private TextMeshProUGUI gameOverText;

    // AudioSource component for playing audio
    [SerializeField] private AudioSource audioSource;

    // Chance for the player to successfully flee from battle
    private float _fleeChance;
    
    
    /// <summary>
    /// Start method called on the frame when the script is enabled.
    /// Initializes the battle setup and begins the battle sequence.
    /// </summary>
    void Start()
    {
        // Retrieve the PelletScroller component from the pelletHolder GameObject
        // This component controls the behavior of pellets in the rhythm attack game
        _pelletScroller = pelletHolder.GetComponent<PelletScroller>();

        // Initially hide the battle action buttons panel
        buttonsPanel.SetActive(false);

        // Set the initial state of the battle to START
        state = BattleState.START;

        // Begin the battle sequence using a coroutine
        // This coroutine manages the setup and progression of the battle
        StartCoroutine(StartBattle());
    }
    
    /// <summary>
    /// Start the battle. This coroutine is called in Start().
    /// It instantiates the player and enemy units, and sets their stats.
    /// </summary>
    /// <returns></returns>
    /// <summary>
    /// Coroutine to start the battle.
    /// It initializes player and enemy units, sets their stats, and begins the first turn.
    /// </summary>
    IEnumerator StartBattle()
    {
        // Instantiate the player's GameObject from the prefab and get the Unit component
        GameObject playerGameObject = Instantiate(playerPrefab);
        playerUnit = playerGameObject.GetComponent<Unit>();

        // Set player stats based on data stored in GameManager
        playerUnit.unitLevel = GameManager.Instance.playerLevel;
        playerUnit.xp = GameManager.Instance.playerXP;
        playerUnit.currentHP = GameManager.Instance.playerCurrentHP;

        // Instantiate the enemy's GameObject from the prefab and get the Unit component
        GameObject enemyGameObject = Instantiate(enemyPrefab);
        enemyUnit = enemyGameObject.GetComponent<Unit>();

        // Determine enemy level based on player's level, within a range of +/- 10%
        int enemyLevel = Mathf.FloorToInt(playerUnit.unitLevel + Random.Range(0.8f, 1.2f));
        enemyLevel = Mathf.Clamp(enemyLevel, 1, enemyUnit.unitMaxLevel); // Clamp to ensure it's within valid bounds

        // Set enemy level and update its stats
        enemyUnit.unitLevel = enemyLevel;
        enemyUnit.UpdateStats();
        enemyUnit.currentHP = enemyUnit.maxHP;

        // Update UI and stats for both player and enemy
        UpdateStats();

        // Display the battle start message
        dialogueText.text = "An enemy " + enemyUnit.unitName + " challenges " + playerUnit.unitName + " to battle!";

        // Calculate the player's chance to flee based on level difference
        _fleeChance = Mathf.Clamp(0.5f + 0.05f * (playerUnit.unitLevel - enemyUnit.unitLevel), 0, 1);

        // Wait for a short duration before starting the player's turn
        yield return new WaitForSeconds(3f);

        // Transition to the player's turn
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    /// <summary>
    /// Update the stats of the player and enemy units.
    /// </summary>
    private void UpdateStats()
    {
        // Update player stats & UI
        playerUnit.UpdateStats();
        playerNameText.text = playerUnit.unitName;
        playerLevelText.text = "Level " + playerUnit.unitLevel;
        playerHpSlider.maxValue = playerUnit.maxHP;
        playerHpSlider.value = playerUnit.currentHP;
        playerHpStatus.text = playerUnit.currentHP + " / " + playerUnit.maxHP;

        // Update enemy stats & UI
        enemyUnit.UpdateStats();
        enemyNameText.text = enemyUnit.unitName;
        enemyLevelText.text = "Level " + enemyUnit.unitLevel;
        enemyHpSlider.maxValue = enemyUnit.maxHP;
        enemyHpSlider.value = enemyUnit.currentHP;
        enemyHpSlider.value = enemyUnit.currentHP;
        enemyHpStatus.text = enemyUnit.currentHP + " / " + enemyUnit.maxHP;
    }

    /// <summary>
    /// End the battle. This function is called when the player or enemy unit is defeated.
    /// </summary>
    /// <param name="state"></param>
    void EndBattle(BattleState state)
    {
        if (state == BattleState.WON)
        {
            int previousLevel = playerUnit.unitLevel;

            playerUnit.GainXP(enemyUnit.givenXP);

            // Start a coroutine to handle the end of the battle
            StartCoroutine(HandleEndOfBattle(true, playerUnit.unitLevel > previousLevel));
        }
        else if (state == BattleState.LOST)
        {
            StartCoroutine(HandleEndOfBattle(false, false));
        }
    }

    /// <summary>
    /// Calculate the player's score based on their level and XP.
    /// This is called when the player loses.
    /// </summary>
    /// <param name="level">The player's level</param>
    /// <param name="xp">The player's XP in that level</param>
    /// <param name="xpForNextLevel">The XP the player needed for the next level</param>
    /// <returns></returns>
    public int CalculateScore(int level, int xp, int xpForNextLevel)
    {
        float levelProgress = (float)xp / xpForNextLevel;
        float score = (level - 1 + levelProgress) * 1000f / 99;
        return Mathf.RoundToInt(score);
    }
    
    /// <summary>
    /// Coroutine to handle the end of the battle, displaying appropriate messages and updating game state.
    /// </summary>
    /// <param name="won">Indicates if the player won the battle.</param>
    /// <param name="leveledUp">Indicates if the player leveled up during the battle.</param>
    IEnumerator HandleEndOfBattle(bool won, bool leveledUp)
    {
        if (won)
        {
            // Update stats and display XP gain message
            UpdateStats();
            yield return StartCoroutine(WaitForMessage(playerUnit.unitName + " gained " + enemyUnit.givenXP + " XP."));

            // Display level-up message if the player leveled up
            if (leveledUp)
            {
                playerLevelText.text = "Level " + playerUnit.unitLevel;
                yield return StartCoroutine(WaitForMessage(playerUnit.unitName + " has leveled up! " +
                                                           playerUnit.unitName + " is now level " +
                                                           playerUnit.unitLevel + "."));
            }

            // Display victory message
            yield return StartCoroutine(WaitForMessage(playerUnit.unitName + " won the battle!"));

            // Update the player's state in GameManager and end the battle
            GameManager.Instance.UpdatePlayerState(playerUnit.unitLevel, playerUnit.xp, playerUnit.currentHP);
            GameManager.Instance.EndBattle();
        }
        else
        {
            // Display defeat message
            yield return StartCoroutine(WaitForMessage(playerUnit.unitName + " was defeated."));

            // Fade out the audio smoothly
            float startVolume = audioSource.volume;
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0, t);
                yield return null;
            }

            // Stop the audio and activate the game over canvas
            audioSource.Stop();
            Debug.Log("Activate the canvas");
            gameOverCanvas.gameObject.SetActive(true);

            // Calculate and display the final score
            float score = CalculateScore(GameManager.Instance.playerLevel, GameManager.Instance.playerXP,
                GameManager.Instance.playerXPForNextLevel);
            gameOverText.text = "Game Over\nScore: " + score;
        }
    }

    /// <summary>
    /// Coroutine to display a message for a fixed duration.
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    IEnumerator DisplayMessage(string message)
    {
        dialogueText.text = message;
        yield return new WaitForSeconds(3f); // Wait for 3 seconds before continuing
    }

    /// <summary>
    /// Coroutine to wait for a message to be displayed before continuing.
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    IEnumerator WaitForMessage(string message)
    {
        yield return StartCoroutine(DisplayMessage(message)); // Display the message and wait
    }

    /// <summary>
    /// Apply damage to a unit and update the corresponding UI elements.
    /// </summary>
    /// <param name="unit">The unit receiving the damage.</param>
    /// <param name="hpSlider">The UI slider representing the unit's HP.</param>
    /// <param name="hpText">The UI text element showing the unit's HP.</param>
    /// <param name="damage">The amount of damage to apply.</param>
    void Damage(Unit unit, Slider hpSlider, TextMeshProUGUI hpText, int damage)
    {
        // Calculate the new HP value, ensuring it doesn't fall below 0
        if (damage <= unit.currentHP)
        {
            hpSlider.value = unit.currentHP - damage;
        }
        else
        {
            hpSlider.value = 0;
            damage = unit.currentHP; // Clamp damage to the current HP if it exceeds it
        }

        // Update the HP text and reduce the unit's current HP
        hpText.text = (unit.currentHP - damage) + " / " + unit.maxHP;
        unit.currentHP -= damage;
    }

    /// <summary>
    /// Called when the Fight button is pressed. Initiates the rhythm attack phase.
    /// </summary>
    public void OnFightButtonPressed()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return; // Do nothing if it's not the player's turn
        }

        dialogueText.text = "Press the spacebar on the beat to attack!";
        buttonsPanel.SetActive(false); // Hide the action buttons
        rhythmAttackPanel.SetActive(true); // Display the rhythm attack UI
    }

    /// <summary>
    /// Called when the Check button is pressed. Shows information about the enemy.
    /// </summary>
    public void OnCheckButtonPressed()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return; // Do nothing if it's not the player's turn
        }

        // Construct and display a message with details about the enemy's potential damage and the player's flee chance
        string message = "The enemy can deal damage between " +
                         Mathf.RoundToInt(Mathf.Pow(enemyUnit.unitLevel, 1.15f)) + " and " +
                         8 * Mathf.RoundToInt(Mathf.Pow(enemyUnit.unitLevel, 1.15f)) + ".\n";
        message += "Your chance of fleeing is " + (_fleeChance * 100) + "%.";
        StartCoroutine(WaitForMessage(message));
    }
    
    /// <summary>
    /// Called when the Run button is pressed. Determines if the player successfully flees the battle.
    /// </summary>
    public void OnRunButtonPressed()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return; // Do nothing if it's not the player's turn
        }

        buttonsPanel.SetActive(false); // Hide the action buttons

        // Randomly determine if the flee attempt is successful based on the flee chance
        if (Random.Range(1, 100) < _fleeChance * 100)
        {
            StartCoroutine(HandleRunAway(true)); // Handle successful flee
        }
        else
        {
            StartCoroutine(HandleRunAway(false)); // Handle failed flee
        }
    }

    /// <summary>
    /// Coroutine to handle the result of a flee attempt.
    /// </summary>
    /// <param name="success">Indicates if the flee attempt was successful.</param>
    IEnumerator HandleRunAway(bool success)
    {
        string message = success ? "You ran away successfully." : "You failed to run away!";
        yield return StartCoroutine(WaitForMessage(message)); // Display the result message

        if (success)
        {
            GameManager.Instance.EndBattle(); // End the battle if flee was successful
        }
        else
        {
            state = BattleState.ENEMYTURN; // Proceed to enemy's turn if flee failed
            EnemyTurn();
            buttonsPanel.SetActive(true); // Re-enable the action buttons
        }
    }
    
    /// <summary>
    /// Called to end the player's attack phase and calculate the damage dealt to the enemy.
    /// </summary>
    public void EndAttack()
    {
        int damageDealt = _pelletScroller.damage; // Retrieve the damage dealt from the PelletScroller
        Damage(enemyUnit, enemyHpSlider, enemyHpStatus, damageDealt); // Apply damage to the enemy
        rhythmAttackPanel.SetActive(false); // Hide the rhythm attack UI
        _pelletScroller.damage = 0;
        _pelletScroller.pelletCount = 0;
        StartCoroutine(EndPlayerTurn(damageDealt)); // Proceed to the end of the player's turn
    }
    
    /// <summary>
    /// Coroutine to handle the end of the player's turn and transition to the enemy's turn.
    /// </summary>
    /// <param name="damageDealt">The amount of damage dealt by the player.</param>
    IEnumerator EndPlayerTurn(int damageDealt)
    {
        yield return
            StartCoroutine(DisplayDamageDialogue(damageDealt, playerUnit, enemyUnit)); // Display the damage dealt
        state = BattleState.ENEMYTURN; // Change the state to the enemy's turn
        EnemyTurn(); // Start the enemy's turn
    }
    
    /// <summary>
    /// Coroutine to display a message indicating the amount of damage dealt.
    /// </summary>
    /// <param name="damageDealt">Amount of damage dealt.</param>
    /// <param name="damageDealer">Unit that dealt the damage.</param>
    /// <param name="damageReceiver">Unit that received the damage.</param>
    IEnumerator DisplayDamageDialogue(int damageDealt, Unit damageDealer, Unit damageReceiver)
    {
        // Display the damage message
        dialogueText.text = damageDealer.unitName + " dealt " + damageDealt + " damage to " + damageReceiver.unitName +
                            "!";
        yield return new WaitForSeconds(3f); // Wait for 3 seconds to allow the player to read the message
    }
    
    /// <summary>
    /// Method called to handle the player's turn.
    /// </summary>
    void PlayerTurn()
    {
        // Check for loss condition (player HP is 0)
        // Technically this will never happen, but
        // it's here just in case.
        if (playerUnit.currentHP <= 0)
        {
            state = BattleState.LOST;
            EndBattle(state);
        }
        // Check for win condition (enemy HP is 0)
        else if (enemyUnit.currentHP <= 0)
        {
            state = BattleState.WON;
            EndBattle(state);
        }
        else
        {
            // Enable action buttons and prompt for player action
            buttonsPanel.SetActive(true);
            dialogueText.text = "What will " + playerUnit.unitName + " do?";
        }
    }

    /// <summary>
    /// Method called to handle the enemy's turn.
    /// </summary>
    void EnemyTurn()
    {
        // Check for loss condition (player HP is 0)
        if (playerUnit.currentHP <= 0)
        {
            state = BattleState.LOST;
            EndBattle(state);
        }
        // Check for win condition (enemy HP is 0)
        else if (enemyUnit.currentHP <= 0)
        {
            state = BattleState.WON;
            EndBattle(state);
        }
        else
        {
            // If neither condition is met, proceed with the enemy attack
            EnemyAttack();
        }
    }

    /// <summary>
    /// Method to execute the enemy's attack.
    /// </summary>
    void EnemyAttack()
    {
        // Calculate damage based on enemy level
        int damageDealt = Random.Range(1, 9) * Mathf.RoundToInt(Mathf.Pow(enemyUnit.unitLevel, 1.15f));

        // Apply the calculated damage to the player
        Damage(playerUnit, playerHpSlider, playerHpStatus, damageDealt);

        // Proceed to the end of the enemy's turn
        StartCoroutine(EndEnemyTurn(damageDealt));
    }
    
    /// <summary>
    /// Coroutine to handle the end of the enemy's turn.
    /// </summary>
    /// <param name="damageDealt">Amount of damage dealt by the enemy.</param>
    IEnumerator EndEnemyTurn(int damageDealt)
    {
        // Display the damage dealt by the enemy
        yield return StartCoroutine(DisplayDamageDialogue(damageDealt, enemyUnit, playerUnit));

        // Transition back to the player's turn
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }
}