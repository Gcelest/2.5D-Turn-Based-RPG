using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; // Only if you're using TextMeshPro

public class ChangeBackground : MonoBehaviour
{
    [Header("Scene Backgrounds")]
    public GameObject[] sceneObjects; // Your background prefabs

    [Header("UI")]
    public TextMeshProUGUI backgroundNameText; // Or use UnityEngine.UI.Text
    public Button leftButton;
    public Button rightButton;

    public int currentIndex;

    public void SetBackground(int index)
    {
        foreach (GameObject bg in sceneObjects)
        {
            bg.SetActive(false);
        }

        if (index >= 0 && index < sceneObjects.Length)
        {
            sceneObjects[index].SetActive(true);
            currentIndex = index;
        }
    }


    void Start()
    {
        foreach (GameObject bg in sceneObjects)
        {
            bg.SetActive(false); // Fully disables entire background object and all children
        }

        // Try to load saved background index (default to 0)
        int savedIndex = PlayerPrefs.GetInt("BackgroundIndex", 0);
        currentIndex = Mathf.Clamp(savedIndex, 0, sceneObjects.Length - 1);
        sceneObjects[currentIndex].SetActive(true);

        // Set background name in UI
        if (backgroundNameText != null)
            backgroundNameText.text = sceneObjects[currentIndex].name;

        // Connect button events
        leftButton.onClick.AddListener(() => UpdateBackground(-1));
        rightButton.onClick.AddListener(() => UpdateBackground(1));
    }

    public int GetCurrentBackgroundIndex()
    {
        return currentIndex;
    }



    void UpdateBackground(int direction)
    {
        // Deactivate current
        sceneObjects[currentIndex].SetActive(false);

        // Update index
        currentIndex += direction;
        if (currentIndex < 0)
        {
            currentIndex = sceneObjects.Length - 1;
        }
        else if (currentIndex >= sceneObjects.Length)
        {
            currentIndex = 0;
        }

        // Activate new
        sceneObjects[currentIndex].SetActive(true);

        // Save selected background index
        PlayerPrefs.SetInt("BackgroundIndex", currentIndex);
        PlayerPrefs.Save();
        Debug.Log("Saved BackgroundIndex = " + currentIndex);

        // Update UI text
        backgroundNameText.text = sceneObjects[currentIndex].name;
    }
}
