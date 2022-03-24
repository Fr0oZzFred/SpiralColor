using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public static class SaveSystem {
    public static void SaveGameManager(GameManager gameManager) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/gameManager.test";
        FileStream stream = new FileStream(path, FileMode.Create);
        GameManagerData data = new GameManagerData(gameManager);
        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static GameManagerData LoadGameManager() {
        string path = Application.persistentDataPath + "/gameManager.test";
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            GameManagerData data = formatter.Deserialize(stream) as GameManagerData;
            stream.Close();
            return data;
        } else Debug.Log("File don't exist");
        return null;
    }
}
