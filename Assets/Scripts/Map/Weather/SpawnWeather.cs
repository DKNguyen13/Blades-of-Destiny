using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWeather : MonoBehaviour
{
    [Header("Weather")]
    [SerializeField] private GameObject thunderPrefab;

    // Reference to the main camera
    private Camera mainCamera;

    // Maximum number of thunders allowed at a time
    [SerializeField] private int maxThunders = 10;
    private int currentThunderCount = 0;

    void Start()
    {
        // Get the main camera
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (currentThunderCount < maxThunders)
        {
            StartCoroutine(SpawnThunder());
        }
    }

    // Method to spawn thunder at a random position within the camera's view
    IEnumerator SpawnThunder()
    {
        // Increment the current thunder count
        currentThunderCount++;

        // Generate random viewport coordinates
        float randomX = Random.Range(0f, 1f);
        float randomY = Random.Range(0.3f, 1f);
        Vector3 randomViewportPosition = new Vector3(randomX, randomY, mainCamera.nearClipPlane + 1f);

        // Convert viewport position to world position
        Vector3 spawnPosition = mainCamera.ViewportToWorldPoint(randomViewportPosition);

        // Instantiate the thunder prefab at the calculated position
        GameObject thunderInstance = Instantiate(thunderPrefab, spawnPosition, Quaternion.identity);

        // Wait for a certain amount of time before destroying the thunder instance
        yield return new WaitForSeconds(1.2f);

        // Destroy the thunder instance
        Destroy(thunderInstance);

        // Decrement the current thunder count after the thunder instance is destroyed
        currentThunderCount--;
    }
}
