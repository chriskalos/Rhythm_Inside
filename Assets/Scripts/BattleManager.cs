using System.Collections;
using System.Collections.Generic;
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

    public Text dialogueText;
    
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
        
        GameObject enemyGameObject = Instantiate(enemyPrefab);
        _enemyUnit = enemyGameObject.GetComponent<Unit>();

        dialogueText.text = "An enemy " + _enemyUnit.unitName + "\nchallenges " + _playerUnit.unitName + " to battle!";
    }
}
