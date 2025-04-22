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

    public int currentIndex = 0;

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

        currentIndex = 0;
        sceneObjects[currentIndex].SetActive(true);

        // Disable all backgrounds first
        for (int i = 0; i < sceneObjects.Length; i++)
        {
            sceneObjects[i].SetActive(false);
        }

        // Show the first background
        currentIndex = 0;
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
        if (currentIndex < 0) currentIndex = sceneObjects.Length - 1;
        else if (currentIndex >= sceneObjects.Length) currentIndex = 0;

        // Activate new
        sceneObjects[currentIndex].SetActive(true);

        // Update UI text
        backgroundNameText.text = sceneObjects[currentIndex].name;
    }
}
