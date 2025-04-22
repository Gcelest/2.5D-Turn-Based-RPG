using System;
using UnityEngine;

[System.Serializable]
public class SaveSlotData
{
    public string playerName;
    public float playTime;
    public Vector3 playerPosition;
    public string sceneName;
    public string lastSaveTime; // Stored as string for JSON compatibility
    public int currentBackgroundIndex; // The index or ID of the background used

}
