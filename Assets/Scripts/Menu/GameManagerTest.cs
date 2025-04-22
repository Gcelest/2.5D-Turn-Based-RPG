using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerTest : MonoBehaviour
{
    public static GameManagerTest instance;
    private Vector3 spawnPosition = new Vector3(131.0f, 13.47f, -1.0f); // Initial spawn position.


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSpawnPoint(Vector3 position)
    {
        spawnPosition = position; // Update the spawn position.
    }

    public Vector3 GetSpawnPoint()
    {
        return spawnPosition; // Get the spawn position.
    }
}
