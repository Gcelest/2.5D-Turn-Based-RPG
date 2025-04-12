using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemyInfo[] allEnemies;
    [SerializeField] private List<Enemy> currentEnemies;

    private static GameObject instance;

    private const float LEVEL_MODIFIER = 0.5f;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this.gameObject;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void GenerateEnemyByEncounter(Encounter[] _encounters, int _maxNumEnemies)
    {
        currentEnemies.Clear();
        int numEnemies = Random.Range(1, _maxNumEnemies + 1);

        for (int i = 0; i < numEnemies; i++)
        {
            Encounter tempEncounter = _encounters[Random.Range(0, _encounters.Length)];
            int level = Random.Range(tempEncounter.LevelMin, tempEncounter.LevelMax + 1);
            GenerateEnemyByName(tempEncounter.Enemy.EnemyName, level);
        }
    }

    private void GenerateEnemyByName(string _enemyName, int _level)
{
    for (int i = 0; i < allEnemies.Length; i++)
    {
        if (_enemyName == allEnemies[i].EnemyName)
        {
            Enemy newEnemy = new Enemy();

            newEnemy.EnemyName = allEnemies[i].EnemyName;
            newEnemy.Level = _level;
            float levelModifier = (LEVEL_MODIFIER * newEnemy.Level);

            // Calculate stats based on level and base values
            newEnemy.MaxHealth = Mathf.RoundToInt(allEnemies[i].BaseHealth + (allEnemies[i].BaseHealth * levelModifier));
            newEnemy.CurrHealth = newEnemy.MaxHealth;
            newEnemy.Strength = Mathf.RoundToInt(allEnemies[i].BaseStr + (allEnemies[i].BaseStr * levelModifier));

            // Here’s where we adjust initiative for higher-level enemies
            newEnemy.Initiative = Mathf.RoundToInt(allEnemies[i].BaseInitiative + (allEnemies[i].BaseInitiative * levelModifier));

            newEnemy.EnemyVisualPrefab = allEnemies[i].EnemyVisualPrefab;

            // If the enemy level is above a certain threshold, boost initiative further
            if (newEnemy.Level > 5)  // Adjust this level threshold as needed
            {
                newEnemy.Initiative += Mathf.RoundToInt(newEnemy.Level * 1.5f);  // Boost initiative for higher-level enemies
            }

            currentEnemies.Add(newEnemy);
        }
    }
}

    public List<Enemy> GetCurrentEnemies()
    {
        return currentEnemies;
    }
}

[System.Serializable]
public class Enemy
{
    public string EnemyName;
    public int Level;
    public int CurrHealth;
    public int MaxHealth;
    public int Strength;
    public int Initiative;
    public GameObject EnemyVisualPrefab;
}

