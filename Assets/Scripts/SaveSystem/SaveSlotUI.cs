using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;


public class SaveSlotUI: MonoBehaviour
{
    public FadeManager fadeManager;
    public List<SaveSlot> slots;

    void Start()
    {
        foreach (var slot in slots)
        {
            int index = slot.slotIndex;

            slot.playButton.onClick.AddListener(() => OnPlayPressed(index, GetFadeManager()));
            slot.deleteButton.onClick.AddListener(() => OnDeletePressed(index));

            SetupSlot(slot);
        }
    }

    void SetupSlot(SaveSlot slot)
    {
        if (SaveSystem.SaveExists(slot.slotIndex))
        {
            SaveSlotData data = SaveSystem.Load(slot.slotIndex);
            slot.infoText.text = $"Player: {data.playerName}\nTime: {FormatTime(data.playTime)}\nSaved: {data.lastSaveTime}";
            slot.playButton.GetComponentInChildren<TextMeshProUGUI>().text = "Continue";
            slot.deleteButton.gameObject.SetActive(true);
        }
        else
        {
            slot.infoText.text = "Empty Slot";
            slot.playButton.GetComponentInChildren<TextMeshProUGUI>().text = "New Game";
            slot.deleteButton.gameObject.SetActive(false);
        }
    }

    public FadeManager GetFadeManager()
    {
        return fadeManager;
    }

    public void OnPlayPressed(int index, FadeManager fadeManager)
    {
        Debug.Log("Play slot " + index);
        fadeManager.gameObject.SetActive(true);
        fadeManager.PlayWithFade(() =>
        {
            if (SaveSystem.SaveExists(index))
                GameManager.Instance.LoadGame(index);
            else
                GameManager.Instance.NewGame(index);
        });
    }

    public void OnDeletePressed(int index)
    {
        SaveSystem.Delete(index);

        // Refresh slot info after deletion
        SetupSlot(slots[index]);
    }

    string FormatTime(float seconds)
    {
        int hrs = Mathf.FloorToInt(seconds / 3600);
        int mins = Mathf.FloorToInt((seconds % 3600) / 60);
        return $"{hrs}h {mins}m";
    }
}


[System.Serializable]
public class SaveSlot
{
    public int slotIndex;
    public Button playButton;
    public Button deleteButton;
    public TextMeshProUGUI infoText;
}
