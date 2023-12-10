using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PelletScroller : MonoBehaviour
{
    public BattleManager battleManager;
    [SerializeField] private GameObject rhythmAttackPanel;
    [SerializeField] private GameObject pelletPrefab;

    private int damage { get; set; }
    private int pelletCount { get; set; }

    private int _spawnedPellets;
    
    // Start is called before the first frame update
    void Start()
    {
        damage = 0;
        pelletCount = 0;
        _spawnedPellets = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnPellet()
    {
        if (rhythmAttackPanel.activeSelf && _spawnedPellets < 8)
        {
            GameObject instance = Instantiate(pelletPrefab, new Vector3(610f, 0f, -10f), Quaternion.identity);
            instance.transform.SetParent(transform, false);
            _spawnedPellets++;
        }
    }

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

    public void HitPellet()
    {
        pelletCount++;
        damage += pelletCount;

        if (pelletCount == 8)
        {
            battleManager.EndAttack();
        }
    }
}
