using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour
{
    [SerializeField] private GameObject pelletPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform hitArea;
    [SerializeField] private float pelletSpeed = 10f;
    [SerializeField] private float beatInterval; // Time between beats, calculated from BPM

    private void Start()
    {
        // Calculate interval between beats (60 seconds divided by BPM)
        beatInterval = 60f / 125f; // For 125 BPM
        StartCoroutine(SpawnPellets());
    }

    IEnumerator SpawnPellets()
    {
        while (true) // You'll want to replace this with a condition to stop spawning when the rhythm section ends
        {
            SpawnPellet();
            yield return new WaitForSeconds(beatInterval);
        }
    }

    void SpawnPellet()
    {
        // Instantiate the pellet and set it moving towards the hit area
        GameObject pellet = Instantiate(pelletPrefab, spawnPoint.position, Quaternion.identity);
        pellet.GetComponent<Rigidbody2D>().velocity = new Vector2(-pelletSpeed, 0); // Assuming the pellet moves along the x-axis
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Replace with your input
        {
            // todo
            // Check for pellets in the hit area
            // Calculate hit or miss
            // You might want to use Physics2D.OverlapCircle here to detect pellets
        }
    }

    // Call this method to calculate the damage based on performance
    public int CalculateDamage()
    {
        // Your damage calculation logic based on how many beats were hit
        return 0; // Replace with actual damage
    }
}