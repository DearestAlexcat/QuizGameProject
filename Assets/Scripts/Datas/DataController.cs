using UnityEngine;
using System.IO;

public static class DataController
{
    public static GameData LoadData(string filePath = "")
    {
        if (string.IsNullOrEmpty(filePath)) // Путь по умолчанию
        {
            filePath = Path.Combine(Application.persistentDataPath, "data.json");
        }

        GameData gameData;

        string dataText = default(string);

        if (File.Exists(filePath))
        {
            dataText = File.ReadAllText(filePath);
        }

        if(string.IsNullOrEmpty(dataText))
        {
            TextAsset data = Resources.Load<TextAsset>("data");
            if (data != null)
            {
                dataText = data.text;
            }
        }

        gameData = JsonUtility.FromJson<GameData>(dataText);
        return gameData;
    }

    public static void SaveData(GameData gameData, string filePath = "")
    {
        if(string.IsNullOrEmpty(filePath))
        {
            filePath = Path.Combine(Application.persistentDataPath, "data.json");
        }

        string dataAsJson = JsonUtility.ToJson(gameData);
        
        File.WriteAllText(filePath, dataAsJson);
    }
}
