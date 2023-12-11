using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    private Unit _playerUnit;
    private Unit _enemyUnit;

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private TextMeshProUGUI playerLevelText;
    [SerializeField] private TextMeshProUGUI enemyLevelText;
    [SerializeField] private TextMeshProUGUI playerHPStatus;
    [SerializeField] private TextMeshProUGUI enemyHPStatus;
    [SerializeField] private Slider playerHPSlider;
    [SerializeField] private Slider enemyHPSlider;
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
        _playerUnit = playerGameObject.GetComponent<Unit>();
        playerNameText.text = _playerUnit.unitName;
        playerLevelText.text = "Level " + _playerUnit.unitLevel;
        playerHPSlider.maxValue = _playerUnit.maxHP;
        playerHPSlider.value = _playerUnit.currentHP;
        playerHPStatus.text = _playerUnit.currentHP + " / " + _playerUnit.maxHP;

        GameObject enemyGameObject = Instantiate(enemyPrefab);
        _enemyUnit = enemyGameObject.GetComponent<Unit>();
        enemyNameText.text = _enemyUnit.unitName;
        enemyLevelText.text = "Level " + _enemyUnit.unitLevel;
        enemyHPSlider.maxValue = _enemyUnit.maxHP;
        enemyHPSlider.value = _enemyUnit.currentHP;
        enemyHPSlider.value = _enemyUnit.currentHP;
        enemyHPStatus.text = _enemyUnit.currentHP + " / " + _enemyUnit.maxHP;

        dialogueText.text = "An enemy " + _enemyUnit.unitName + " challenges " + _playerUnit.unitName + " to battle!";

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
            dialogueText.text = _playerUnit.unitName + " won the battle!";
            // wait 3 seconds
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = _playerUnit.unitName + " was defeated.";
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
        Damage(_enemyUnit, enemyHPSlider, enemyHPStatus, damageDealt);
        rhythmAttackPanel.SetActive(false);
        _pelletScroller.damage = 0;
        _pelletScroller.pelletCount = 0;
        StartCoroutine(EndPlayerTurn(damageDealt));
    }
    
    IEnumerator EndPlayerTurn(int damageDealt)
    {
        yield return StartCoroutine(DisplayDamageDialogue(damageDealt, _playerUnit, _enemyUnit));
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
        if (_playerUnit.currentHP <= 0)
        {
            state = BattleState.LOST;
            EndBattle(state);
        }
        else if (_enemyUnit.currentHP <= 0)
        {
            state = BattleState.WON;
            EndBattle(state);
        }
        else
        {
            buttonsPanel.SetActive(true);
            dialogueText.text = "What will " + _playerUnit.unitName + " do?";
        }
    }
    
    void EnemyTurn()
    {
        if (_playerUnit.currentHP <= 0)
        {
            state = BattleState.LOST;
            EndBattle(state);
        }
        else if (_enemyUnit.currentHP <= 0)
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
        int damageDealt = Random.Range(1, 4) * _enemyUnit.unitLevel;
        
        Damage(_playerUnit, playerHPSlider, playerHPStatus, damageDealt);
        StartCoroutine(EndEnemyTurn(damageDealt));
    }
    
    IEnumerator EndEnemyTurn(int damageDealt)
    {
        yield return StartCoroutine(DisplayDamageDialogue(damageDealt, _enemyUnit, _playerUnit));
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }
}