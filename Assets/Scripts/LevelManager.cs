using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour, ISerializationCallbackReceiver {



    [Header("Data")]
    [SerializeField]
    PlayerHandler playerHandler;
    [ListToPopup(typeof(LevelManager), "TMPList")]
    public string Level;
    int levelInt => int.Parse(Level.Remove(0, Level.Length - 1));

    [Header("Checkpoints")]
    [SerializeField]
    List<Checkpoint> checkpoints;
    

    int checkpointProgression = 0;


    public static LevelManager Instance { get;private set; }

    public static List<string> TMPList;
    [HideInInspector] public List<string> PopupList;
    public List<string> GetAllScenesInBuild() {
        List<string> AllScenes = new List<string>();

        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++) {
            string scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            AllScenes.Add(sceneName);
        }
        return AllScenes;
    }
    private void Awake() {
        if (!Instance) Instance = this;
    }
    public void OnBeforeSerialize() {
        PopupList = GetAllScenesInBuild();
        TMPList = PopupList;
    }

    public void OnAfterDeserialize() { }

    private void Start() {
        checkpoints.Sort();
    }

    private void Update() {
        if (Keyboard.current.rKey.wasPressedThisFrame) {
            playerHandler.GetCurrentPlayer().Respawn(checkpoints[checkpointProgression].transform.position);
        }
    }
    [ContextMenu("Refresh")]
    private void OnValidate() {
        checkpoints.Sort();
    }
    /// <summary>
    /// Function called at the end of the level
    /// </summary>
    void LevelEnds() {
        GameManager.Instance.UpdateProgression(levelInt);
    }
    /// <summary>
    /// Progression of checkpoints
    /// </summary>
    /// <param name="prog"></param>
    public void UpdateCPProgression(int prog) {
        if (prog < checkpointProgression) return;
        checkpointProgression = prog;
    }
}

