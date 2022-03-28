using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour, ISerializationCallbackReceiver {

    #region Fields

    [Header("Data")]
    [SerializeField]
    PlayerHandler playerHandler;
    [ListToPopup(typeof(LevelManager), "TMPList")]
    public string Level;
    public int LevelInt => int.Parse(Level.Remove(0, Level.Length - 1));

    [Header("Musics")]
    [SerializeField]
    string music;

    [Header("Checkpoints")]
    [SerializeField]
    List<Checkpoint> checkpoints;
    int checkpointProgression = 0;

    public static LevelManager Instance { get; private set; }
    #endregion

    #region Editor
    [ContextMenu("Refresh")]
    private void OnValidate() {
        checkpoints.Sort();
    }
    public bool HasMissingCheckPoints() {
        foreach (Checkpoint ck in checkpoints) {
            if (!ck) return true;
        }
        return false;
    }
    public void RemoveMissingCheckpoints() {
        if (Application.isPlaying) {
            Debug.LogError("Do not invoke in play mode!");
            return;
        }
        foreach (Checkpoint ck in checkpoints) {
            if (!ck) checkpoints.Remove(ck);
        }
    }
    public Checkpoint GetLastCheckpoint {
        get {
            if(checkpoints.Count == 0) {
                return null;
            }
            if(!checkpoints[checkpoints.Count - 1]) {
                Debug.LogError("Please remove missing checkpoints before adding new ones.");
            }
            return checkpoints[checkpoints.Count - 1];
        }
    }

    public void AddCheckpoint(Checkpoint ck) {
        for (int i = 0; i < checkpoints.Count; i++) {
            if (!checkpoints[i]) {
                checkpoints[i] = ck;
                return;
            }
        }
        checkpoints.Add(ck);
    }

    public (int, int) HasCheckpointsWithSameProgression() {
        for (int i = 1; i < checkpoints.Count; i++) {
            if (checkpoints[i - 1].Progression == checkpoints[i].Progression) {
                return (i - 1, i);
            }
        }
        return (0, 2);
    }

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
    public void OnBeforeSerialize() {
        PopupList = GetAllScenesInBuild();
        TMPList = PopupList;
    }

    public void OnAfterDeserialize() { }

    #endregion Editor


    private void Awake() {
        if (!Instance) Instance = this;
        if (GameManager.Instance)
            GameManager.Instance.SetState(GameState.InLevel);
    }

    private void Start() {
        checkpoints.Sort();
        if(SoundsManager.Instance)
            SoundsManager.Instance.Play(music);
    }

    private void Update() {
        if (Keyboard.current.rKey.wasPressedThisFrame) {
            playerHandler.CurrentPlayer.Respawn(checkpoints[checkpointProgression].transform.position);
        }
    }

    /// <summary>
    /// Function called at the end of the level
    /// </summary>
    public void TriggerLevelEnd() {
        GameManager.Instance.UpdateProgression(GameManager.Instance.Progression + 1);
        GameManager.Instance.SetState(GameState.Score);
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

