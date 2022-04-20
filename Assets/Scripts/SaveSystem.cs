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
    public static void SaveSoundsSettings(SoundsManager sounds) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/SoundsSettings.test";
        FileStream stream = new FileStream(path, FileMode.Create);
        SoundsSettingsData data = new SoundsSettingsData(sounds);
        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static SoundsSettingsData LoadSoundsSettings() {
        string path = Application.persistentDataPath + "/SoundsSettings.test";
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            SoundsSettingsData data = formatter.Deserialize(stream) as SoundsSettingsData;
            stream.Close();
            return data;
        } else Debug.Log("File don't exist");
        return null;
    }
    public static void SaveGemmes(GemsPool gems) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/GemsPool.test";
        FileStream stream = new FileStream(path, FileMode.Create);
        GemsData data = new GemsData(gems);
        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static GemsData LoadGemmes() {
        string path = Application.persistentDataPath + "/GemsPool.test";
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            GemsData data = formatter.Deserialize(stream) as GemsData;
            stream.Close();
            return data;
        } else Debug.Log("File don't exist");
        return null;
    }
}
