using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class Managers : MonoBehaviour, ISerializationCallbackReceiver  {

    [SerializeField]
    GameState baseGameState;

    [ListToPopup(typeof(Managers), "TMPList")]
    public string TargetScene, RenderingScene;

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
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        Cursor.visible = true;
#else
        Cursor.visible = false;
#endif
        DontDestroyOnLoad(this);
        if (File.Exists(Application.persistentDataPath + "/GameManager.data")) {
            GameManager.Instance.LoadGameManager();
        } else {
            GameManager.Instance.SaveGameManager();
        }
        SceneManagement.Instance.LoadingRendering(TargetScene, RenderingScene);
        GameManager.Instance.SetState(baseGameState);
    }
}
