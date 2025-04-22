using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float playTime;
    public int currentSlot;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Optional if you want it to persist
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        playTime += Time.deltaTime;
    }

    public void NewGame(int slot)
    {
        currentSlot = slot;
        playTime = 0;
        // Set other new game data...
        SceneManager.LoadScene("OverworldScene"); // Replace with your actual scene
    }

    public void LoadGame(int slot)
    {
        currentSlot = slot;
        SaveSlotData data = SaveSystem.Load(slot);

        // Load scene and apply data
        playTime = data.playTime;
        SceneManager.LoadScene(data.sceneName);
        // After scene loads, set player position via coroutine or event
        StartCoroutine(ApplyLoadedData(data));
    }

    public void SaveGame(Vector3 playerPosition, int backgroundIndex)
    {
        SaveSlotData data = new SaveSlotData
        {
            playerName = "Player", // Add player naming later
            playTime = playTime,
            playerPosition = playerPosition,
            sceneName = SceneManager.GetActiveScene().name,
            lastSaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
            currentBackgroundIndex = backgroundIndex // 👈 Save background style here

        };

        SaveSystem.Save(data, currentSlot);
    }

    private IEnumerator ApplyLoadedData(SaveSlotData data)
    {
        yield return new WaitForSeconds(0.1f); // wait for scene to fully load

        // Find your background handler (e.g., ChangeBackground)

        ChangeBackground bgManager = FindFirstObjectByType<ChangeBackground>();
        if (bgManager != null)
        {
            bgManager.SetBackground(data.currentBackgroundIndex);
        }

        // Set player position, etc. if needed
    }
}
