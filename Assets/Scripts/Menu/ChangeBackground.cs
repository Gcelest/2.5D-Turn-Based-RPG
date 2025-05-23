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
        if (index == currentIndex && sceneObjects[index].activeSelf)
        {
            // Same background and already active — skip to avoid resetting animation
            return;
        }

        for (int i = 0; i < sceneObjects.Length; i++)
        {
            if (i != index && sceneObjects[i].activeSelf)
            {
                sceneObjects[i].SetActive(false); // Only deactivate others
            }
        }

        if (index >= 0 && index < sceneObjects.Length)
        {
            sceneObjects[index].SetActive(true);
            currentIndex = index;
        }
    }

    void Start()
    {
        int savedIndex = PlayerPrefs.GetInt("BackgroundIndex", 0);
        currentIndex = Mathf.Clamp(savedIndex, 0, sceneObjects.Length - 1);

        for (int i = 0; i < sceneObjects.Length; i++)
        {
            if (i == currentIndex)
            {
                sceneObjects[i].SetActive(true); // Only activate the selected one
            }
            else
            {
                sceneObjects[i].SetActive(false); // Deactivate the rest
            }
        }

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

    public Camera GetActiveBackgroundCamera()
    {
        if (currentIndex >= 0 && currentIndex < sceneObjects.Length)
            return sceneObjects[currentIndex].GetComponentInChildren<Camera>(true); // include inactive
        return null;
    }


}
