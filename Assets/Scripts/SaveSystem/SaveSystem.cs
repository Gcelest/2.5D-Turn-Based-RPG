using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string GetPath(int slot) => Application.persistentDataPath + $"/save_slot_{slot}.json";

    public static void Save(SaveSlotData data, int slot)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(GetPath(slot), json);
    }

    public static SaveSlotData Load(int slot)
    {
        string path = GetPath(slot);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveSlotData>(json);
        }
        return null;
    }

    public static void Delete(int slot)
    {
        string path = GetPath(slot);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static bool SaveExists(int slot)
    {
        return File.Exists(GetPath(slot));
    }
}
