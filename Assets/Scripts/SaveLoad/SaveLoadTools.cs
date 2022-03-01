using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveLoadTools 
{

    public static void SaveGameData(GameSaveData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = $"{Application.persistentDataPath}/autosave.save";
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GameSaveData LoadGameData()
    {
        string path = $"{Application.persistentDataPath}/autosave.save";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            GameSaveData data = formatter.Deserialize(stream) as GameSaveData;
            stream.Close();
            return data;
        }
        else { return null; }
        
    }
}
