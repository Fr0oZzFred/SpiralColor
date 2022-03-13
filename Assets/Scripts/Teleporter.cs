using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Teleporter : MonoBehaviour, ISerializationCallbackReceiver {

    [Header("Text")]
    [SerializeField]
    TMP_Text textGO;
    [SerializeField]
    string text;
    [SerializeField]
    Color colorLocked, colorUnlocked;

    [Header("Teleporter")]
    [ListToPopup(typeof(Teleporter), "TMPList")]
    public string targetScene;

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
    private void OnValidate() {
        if (textGO) {
            ChangeTextColor(colorLocked);
            ChangeTextContent(text);
        }
    }
    public void ChangeTextColor(Color c) {
        textGO.color = c;
    }
    public void ChangeTextContent(string text) {
        textGO.SetText(text);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<IControllable>() != null) SceneManagement.Instance.LoadLevel(targetScene);
    }
}
