using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class Managers : MonoBehaviour, ISerializationCallbackReceiver  {

    [SerializeField]
    GameState baseGameState;

    [ListToPopup(typeof(Managers), "TMPList")]
    public string TargetScene;

    public static List<string> TMPList;
    [HideInInspector] public List<string> PopupList;
    public List<string> GetAllScenesInBuild() {
        List<string> AllScenes = new List<string>();

        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++) {
            string scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
            AllScenes.Add(sceneName);
        }
        return AllScenes;
    }
    public void OnBeforeSerialize() {
        PopupList = GetAllScenesInBuild();
        TMPList = PopupList;
    }

    public void OnAfterDeserialize() { } 

    private void Start() {
        DontDestroyOnLoad(this);
        if (File.Exists(Application.persistentDataPath + "/gameManager.test"))
            Debug.Log("Load GM");
        else
            Debug.Log("Save GM");
        GameManager.Instance.SetState(baseGameState);
        SceneManagement.Instance.LoadLevel(TargetScene);
    }
}
