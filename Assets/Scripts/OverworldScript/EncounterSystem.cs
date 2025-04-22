using UnityEngine;

public class EncounterSystem : MonoBehaviour
{

    [SerializeField] private Encounter[] enemiesInScene;
    [SerializeField] private int maxNumEnemies;

    private EnemyManager enemyManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();
        enemyManager.GenerateEnemyByEncounter(enemiesInScene, maxNumEnemies);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]

public class Encounter
{
    public EnemyInfo Enemy;
    public int LevelMin;
    public int LevelMax;
}
