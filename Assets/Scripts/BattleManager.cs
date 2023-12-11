using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        playerNameText.text = playerUnit.unitName;
        playerLevelText.text = "Level " + playerUnit.unitLevel;
        playerHpSlider.maxValue = playerUnit.maxHP;
        playerHpSlider.value = playerUnit.currentHP;
        playerHpStatus.text = playerUnit.currentHP + " / " + playerUnit.maxHP;

        GameObject enemyGameObject = Instantiate(enemyPrefab);
        enemyUnit = enemyGameObject.GetComponent<Unit>();
        enemyUnit.unitLevel = 15;
        enemyUnit.UpdateStats();
        enemyNameText.text = enemyUnit.unitName;
        enemyLevelText.text = "Level " + enemyUnit.unitLevel;
        enemyHpSlider.maxValue = enemyUnit.maxHP;
        enemyHpSlider.value = enemyUnit.currentHP;
        enemyHpSlider.value = enemyUnit.currentHP;
        enemyHpStatus.text = enemyUnit.currentHP + " / " + enemyUnit.maxHP;

        dialogueText.text = "An enemy " + enemyUnit.unitName + " challenges " + playerUnit.unitName + " to battle!";

        yield return new WaitForSeconds(3f);
        
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void EndBattle(BattleState state)
    {
        // todo: end battle
        // todo: if player wins, give player exp, return to overworld
        // todo: if player loses, game over
        
        if (state == BattleState.WON)
        {
            dialogueText.text = playerUnit.unitName + " won the battle!";
            // todo: give player exp
            // todo: return to overworld
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = playerUnit.unitName + " was defeated.";
            // todo: game over
        }
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
        dialogueText.text = "Press the spacebar on the beat to attack!";
        buttonsPanel.SetActive(false);
        rhythmAttackPanel.SetActive(true);
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