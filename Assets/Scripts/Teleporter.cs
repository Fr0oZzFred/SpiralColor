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
    [SerializeField]
    int requiredProgression;
    [ListToPopup(typeof(Teleporter), "TMPList")]
    public string TargetScene;
    [ListToPopup(typeof(Teleporter), "TMPList")]
    public string AdditiveScene;

    #region Editor
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
    #endregion

    private void OnValidate() {
        if (textGO) {
            ChangeTextColor(colorLocked);
            ChangeTextContent(text);
        }
    }
    private void Start() {
        if (requiredProgression <= GameManager.Instance.Progression)
            ChangeTextColor(colorUnlocked);
    }
    void ChangeTextColor(Color c) {
        if(textGO) textGO.color = c;
    }
    void ChangeTextContent(string text) {
        if(textGO)  textGO.SetText(text);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Controller>() != null) {
            if (requiredProgression <= GameManager.Instance.Progression) {
                PlayTestData.Instance.Restart();
                if (AdditiveScene != null) SceneManagement.Instance.LoadingRendering(TargetScene, AdditiveScene);
                else SceneManagement.Instance.LoadLevel(TargetScene);
            }
        }
    }
}
