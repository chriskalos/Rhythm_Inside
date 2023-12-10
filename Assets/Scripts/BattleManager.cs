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

        buttonsPanel.SetActive(true);
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void EndBattle()
    {
        // todo: end battle
        // Take stats from battle and pass them to GameManager
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

    void PlayerTurn()
    {
        dialogueText.text = "What will " + _playerUnit.unitName + " do?";
    }

    public void OnFightButtonPressed()
    {
        dialogueText.text = "Press the spacebar to the beat to attack!";
        buttonsPanel.SetActive(false);
        rhythmAttackPanel.SetActive(true);
    }

    public void EndAttack()
    {
        int _damageDealt = _pelletScroller.damage;
        Damage(_enemyUnit, enemyHPSlider, enemyHPStatus, _damageDealt);
        dialogueText.text = _playerUnit.unitName + " dealt " + _damageDealt + " damage to " + _enemyUnit.unitName + "!";
        rhythmAttackPanel.SetActive(false);
        _pelletScroller.damage = 0;
        _pelletScroller.pelletCount = 0;
        state = BattleState.ENEMYTURN;
    }
}