using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState{ START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleManager : MonoBehaviour
{

    [SerializeField] private BattleState state;

    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private GameObject enemyPrefab;

    private Unit _playerUnit;
    private Unit _enemyUnit;

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private TextMeshProUGUI playerLevelText;
    [SerializeField] private TextMeshProUGUI enemyLevelText;
    
    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartBattle();
    }

    void StartBattle()
    {
        GameObject playerGameObject = Instantiate(playerPrefab);
        _playerUnit = playerGameObject.GetComponent<Unit>();
        playerNameText.text = _playerUnit.unitName;
        playerLevelText.text = "Level " + _playerUnit.unitLevel;
        
        GameObject enemyGameObject = Instantiate(enemyPrefab);
        _enemyUnit = enemyGameObject.GetComponent<Unit>();
        enemyNameText.text = _enemyUnit.unitName;
        enemyLevelText.text = "Level " + _enemyUnit.unitLevel;

        dialogueText.text = "An enemy " + _enemyUnit.unitName + " challenges " + _playerUnit.unitName + " to battle!";
    }
}
