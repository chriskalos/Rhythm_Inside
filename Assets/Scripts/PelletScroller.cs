using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PelletScroller : MonoBehaviour
{
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
        GameObject instance = Instantiate(pelletPrefab, new Vector3(600f, 0f, -10f), Quaternion.identity);
        
        instance.transform.SetParent(this.transform, false);
    }

    public void MoveToTheBeat()
    {
        foreach (BeatPellet pellet in GetComponentsInChildren<BeatPellet>())
        {
            pellet.transform.localPosition += new Vector3(-5f, 0f, 0f);
        }
    }
}
