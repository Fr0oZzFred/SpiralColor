using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Scenes : ISerializationCallbackReceiver {

    public ScenesEnum destination;

    [ListToPopup(typeof(Scenes), "TMPList")]
    public string TargetScene;

    [ListToPopup(typeof(Scenes), "TMPList")]
    public string AdditiveScene;

    public static List<string> TMPList;
    [HideInInspector] public List<string> sceneList;

    public List<string> GetAllScenesInBuild() {
        List<string> AllScenes = new List<string>();

        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++) {
            string scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            AllScenes.Add(sceneName);
        }
        return AllScenes;
    }

    public void OnBeforeSerialize() {
        sceneList = GetAllScenesInBuild();
        TMPList = sceneList;
    }

    public void OnAfterDeserialize() { }
}
