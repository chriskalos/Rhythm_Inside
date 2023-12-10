using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PelletScroller : MonoBehaviour
{
    [SerializeField] private GameObject rhythmAttackPanel;
    [SerializeField] private GameObject pelletPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnPellet()
    {
        if (rhythmAttackPanel.activeSelf)
        {
            GameObject instance = Instantiate(pelletPrefab, new Vector3(610f, 0f, -10f), Quaternion.identity);
            instance.transform.SetParent(this.transform, false);
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
}
