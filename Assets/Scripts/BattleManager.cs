using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum BattleState
{
    START,
    PLAYERTURN,
    ENEMYTURN,
    WON,
    LOST
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleState state;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private GameObject rhythmAttackPanel;
    [SerializeField] private GameObject pelletHolder;
    private PelletScroller _pelletScroller;
    
    // These are public because they are accessed by the PelletScroller
    public Unit playerUnit;
    public Unit enemyUnit;

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private TextMeshProUGUI playerLevelText;
    [SerializeField] private TextMeshProUGUI enemyLevelText;
    [SerializeField] private TextMeshProUGUI playerHpStatus;
    [SerializeField] private TextMeshProUGUI enemyHpStatus;
    [SerializeField] private Slider playerHpSlider;
    [SerializeField] private Slider enemyHpSlider;
    [SerializeField] private GameObject buttonsPanel;

    private float _fleeChance;

    // Start is called before the first frame update
    void Start()
    {
        _pelletScroller = pelletHolder.GetComponent<PelletScroller>();
        buttonsPanel.SetActive(false);
        state = BattleState.START;
        StartCoroutine(StartBattle());
    }

    IEnumerator StartBattle()
    {
        GameObject playerGameObject = Instantiate(playerPrefab);
        playerUnit = playerGameObject.GetComponent<Unit>();

        playerUnit.unitLevel = GameManager.Instance.playerLevel;
        playerUnit.xp = GameManager.Instance.playerXP;
        playerUnit.currentHP = GameManager.Instance.playerCurrentHP;

        GameObject enemyGameObject = Instantiate(enemyPrefab);
        enemyUnit = enemyGameObject.GetComponent<Unit>();
        
        // Randomize enemy level based on player level
        int enemyLevel = playerUnit.unitLevel + Random.Range(2, 6); // Enemy level is 2-5 levels higher than player level
        enemyLevel = Mathf.Clamp(enemyLevel, 1, enemyUnit.unitMaxLevel); // Ensure enemy level doesn't go below 1 or above a maximum level
        
        enemyUnit.unitLevel = enemyLevel;
        
        enemyUnit.UpdateStats();
        enemyUnit.currentHP = enemyUnit.maxHP;
        UpdateStats();

        dialogueText.text = "An enemy " + enemyUnit.unitName + " challenges " + playerUnit.unitName + " to battle!";
        
        _fleeChance = Mathf.Clamp(0.5f + 0.05f * (playerUnit.unitLevel - enemyUnit.unitLevel), 0, 1);

        yield return new WaitForSeconds(3f);
        
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

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

    void EndBattle(BattleState state)
    {
        // todo: if player loses, game over
        
        if (state == BattleState.WON)
        {
            // todo: return to overworld
            
            int previousLevel = playerUnit.unitLevel;
            
            playerUnit.GainXP(enemyUnit.givenXP);
            
            // Start a coroutine to handle the end of the battle
            StartCoroutine(HandleEndOfBattle(playerUnit.unitLevel > previousLevel));
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = playerUnit.unitName + " was defeated.";
            // todo: game over
        }
    }
    
    IEnumerator HandleEndOfBattle(bool leveledUp)
    {
        UpdateStats();
        yield return StartCoroutine(WaitForMessage(playerUnit.unitName + " gained " + enemyUnit.givenXP + " XP."));
        if (leveledUp)
        {
            playerLevelText.text = "Level " + playerUnit.unitLevel;
            yield return StartCoroutine(WaitForMessage(playerUnit.unitName + " has leveled up! " + playerUnit.unitName + " is now level " + playerUnit.unitLevel + "."));
        }

        yield return StartCoroutine(WaitForMessage(playerUnit.unitName + " won the battle!"));

        GameManager.Instance.UpdatePlayerState(playerUnit.unitLevel, playerUnit.xp, playerUnit.currentHP);
        GameManager.Instance.EndBattle();
    }
    
    IEnumerator DisplayMessage(string message)
    {
        dialogueText.text = message;
        yield return new WaitForSeconds(3f);
    }
    
    IEnumerator WaitForMessage(string message)
    {
        yield return StartCoroutine(DisplayMessage(message));
    }
    
    void Damage(Unit unit, Slider hpSlider, TextMeshProUGUI hpText, int damage)
    {
        if (damage <= unit.currentHP)
        {
            hpSlider.value = unit.currentHP - damage;
        }
        else
        {
            hpSlider.value = 0;
            damage = unit.currentHP;
        }
        hpText.text = (unit.currentHP - damage) + " / " + unit.maxHP;
        unit.currentHP -= damage;
    }

    public void OnFightButtonPressed()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        dialogueText.text = "Press the spacebar on the beat to attack!";
        buttonsPanel.SetActive(false);
        rhythmAttackPanel.SetActive(true);
    }

    public void OnCheckButtonPressed()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        string message = "The enemy can deal damage between " + enemyUnit.unitLevel + " and " + 4 * enemyUnit.unitLevel + ".\n";
        message += "Your chance of fleeing is " + (_fleeChance * 100) + "%.";
        StartCoroutine(WaitForMessage(message));
    }

    public void OnRunButtonPressed()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        buttonsPanel.SetActive(false);
        
        if (Random.Range(1, 100) < _fleeChance * 100)
        {
            StartCoroutine(HandleRunAway(true));
        }
        else
        {
            StartCoroutine(HandleRunAway(false));
        }
    }

    IEnumerator HandleRunAway(bool success)
    {
        string message = success ? "You ran away successfully." : "You failed to run away!";
        yield return StartCoroutine(WaitForMessage(message));

        if (success)
        {
            GameManager.Instance.EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            EnemyTurn();
            buttonsPanel.SetActive(true);
        }
    }
    
    public void EndAttack()
    {
        int damageDealt = _pelletScroller.damage;
        Damage(enemyUnit, enemyHpSlider, enemyHpStatus, damageDealt);
        rhythmAttackPanel.SetActive(false);
        _pelletScroller.damage = 0;
        _pelletScroller.pelletCount = 0;
        StartCoroutine(EndPlayerTurn(damageDealt));
    }
    
    IEnumerator EndPlayerTurn(int damageDealt)
    {
        yield return StartCoroutine(DisplayDamageDialogue(damageDealt, playerUnit, enemyUnit));
        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }
    
    IEnumerator DisplayDamageDialogue(int damageDealt, Unit damageDealer, Unit damageReceiver)
    {
        dialogueText.text = damageDealer.unitName + " dealt " + damageDealt + " damage to " + damageReceiver.unitName + "!";
        yield return new WaitForSeconds(3f);
    }
    
    void PlayerTurn()
    {
        if (playerUnit.currentHP <= 0)
        {
            state = BattleState.LOST;
            EndBattle(state);
        }
        else if (enemyUnit.currentHP <= 0)
        {
            state = BattleState.WON;
            EndBattle(state);
        }
        else
        {
            buttonsPanel.SetActive(true);
            dialogueText.text = "What will " + playerUnit.unitName + " do?";
        }
    }
    
    void EnemyTurn()
    {
        if (playerUnit.currentHP <= 0)
        {
            state = BattleState.LOST;
            EndBattle(state);
        }
        else if (enemyUnit.currentHP <= 0)
        {
            Debug.Log(state);
            state = BattleState.WON;
            Debug.Log(state);
            Debug.Log("Calling EndBattle(state);");
            EndBattle(state);
        }
        else
        {
            EnemyAttack();
        }
    }
    
    void EnemyAttack()
    {
        // Scale attack damage with the level of the enemy realistically
        int damageDealt = Random.Range(1, 4) * enemyUnit.unitLevel;
        
        Damage(playerUnit, playerHpSlider, playerHpStatus, damageDealt);
        StartCoroutine(EndEnemyTurn(damageDealt));
    }
    
    IEnumerator EndEnemyTurn(int damageDealt)
    {
        yield return StartCoroutine(DisplayDamageDialogue(damageDealt, enemyUnit, playerUnit));
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }
}