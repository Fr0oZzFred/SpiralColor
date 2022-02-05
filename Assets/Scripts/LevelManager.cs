using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour, ISerializationCallbackReceiver {
    public static List<string> TMPList;
    [HideInInspector]public List<string> PopupList;
    [ListToPopup(typeof(LevelManager), "TMPList")]
    public string Popup;

    public List<string> GetAllScenesInBuild() {
        List<string> AllScenes = new List<string>();

        for(int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            AllScenes.Add(sceneName);
        }
        return AllScenes;
    }

    public void OnBeforeSerialize() {
        PopupList = GetAllScenesInBuild();
        TMPList = PopupList;
    }

    public void OnAfterDeserialize() {}
}

