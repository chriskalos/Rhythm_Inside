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
    
    [SerializeField] private GameObject attackPrefab;
    private Attack _attackScript;

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

    IEnumerator PlayerAttack()
    {
        // todo OnFightButtonPressed();
        // bool isDead = // todo: take damage with EndAttack()
        // SetHP(_enemyUnit, enemyHPSlider, enemyHPStatus, damage);

        int damage = EndAttack();
        bool isDead = _enemyUnit.currentHP - damage <= 0;
        dialogueText.text = _enemyUnit.name + " took " + damage + " damage!";
        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            dialogueText.text = _enemyUnit.unitName + " was defeated!";
            yield return new WaitForSeconds(3f);
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            // todo: StartCoroutine(EnemyTurn());
        }
    }

    void EndBattle()
    {
        
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
        buttonsPanel.SetActive(false);
        GameObject attack = Instantiate(attackPrefab);
        Damage(_enemyUnit, enemyHPSlider, enemyHPStatus, _attackScript.CalculateDamage());
        _attackScript = attack.GetComponent<Attack>();
    }

    public int EndAttack()
    {
        int damage = _attackScript.CalculateDamage();
        _enemyUnit.TakeDamage(damage);
        Destroy(_attackScript.gameObject);
        buttonsPanel.SetActive(true);
        return damage;
    }
}