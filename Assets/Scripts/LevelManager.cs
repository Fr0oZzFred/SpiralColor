using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using System.Collections;

public class LevelManager : MonoBehaviour {

    #region Fields

    [Header("General")]
    [SerializeField] float skyboxRotationSpeed;
    [SerializeField] Scenes scene;
    [SerializeField] PlayerHandler playerHandler;
    [SerializeField] PlayableDirector timeline;

    Material skybox;

    public int LevelInt => int.Parse(scene.TargetScene.Remove(0, scene.TargetScene.Length - 2));

    [Header("Musics")]
    [SerializeField] string music;
    [SerializeField] string scoreMusic;

    [Header("Checkpoints")]
    [SerializeField] Checkpoint checkpointPrefab;
    [SerializeField]
    List<Checkpoint> checkpoints;
    int checkpointProgression = 0;

    [Header("Portal")]
    [SerializeField] float portalDelay;
    [SerializeField] Transform portal; //Gardez comme base au cas où on change d'avis sur le material des led du portal
    VisualEffect vfx;
    [SerializeField] [ColorUsage(true, true)] List<Color> colors;
    public Controller CurrentController {
        get {
            return playerHandler.CurrentPlayer;
        }
    }
    public bool CoroutineIsRunning {
        get {
            return playerHandler.CoroutineIsRunning;
        }
    }

    public static LevelManager Instance { get; private set; }
    #endregion

    #region Editor
    [ContextMenu("Refresh")]
    private void OnValidate() {
        checkpoints.Sort();
    }
    public Checkpoint GetCheckpointPrefab() {
        return checkpointPrefab;
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
    #endregion Editor


    private void Awake() {
        if (!Instance) Instance = this;
    }

    private void Start() {
        if (GameManager.Instance)
            GameManager.Instance.SetState(GameState.InLevel);

        checkpoints.Sort();
        if (SoundsManager.Instance) {
            SoundsManager.Instance.StopCurrentMusic();
            SoundsManager.Instance.Play(music);
        }
        if (UIManager.Instance) {
            UIManager.Instance.DisplayGems(false);
        }
        timeline.gameObject.SetActive(false);
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        skybox = RenderSettings.skybox;
        vfx = portal.GetChild(0).GetComponentInChildren<VisualEffect>(true);
        vfx.gameObject.SetActive(false);
    }

    private void Update() {
        if (Keyboard.current.rKey.wasPressedThisFrame) {
            if(PlayTestData.Instance)
                PlayTestData.Instance.Dead();
            Respawn(playerHandler.CurrentPlayer);
        }

        skybox.SetFloat("_Rotation", skybox.GetFloat("_Rotation") + skyboxRotationSpeed * Time.deltaTime);
    }

    public void Respawn(Controller controller) {
        int index = controller.GetClosestAllowedCheckpoint(checkpointProgression);
        controller.Respawn(checkpoints[index].transform.position, checkpoints[index].CamRotation);
    }

    /// <summary>
    /// Function called at the end of the level
    /// </summary>
    public void TriggerLevelEnd() {
        timeline.gameObject.SetActive(true);
        GameManager.Instance.UpdateProgression(LevelInt + 1);
        SoundsManager.Instance.StopCurrentMusic();
        SoundsManager.Instance.Play(scoreMusic);
        GameManager.Instance.SetState(GameState.Score);
        //StartCoroutine(ChangePortalColor());
    }

    //Gardez en cas de changement de dernière minute
    IEnumerator ChangePortalColor() {
        yield return new WaitForSeconds(portalDelay);
        for (int i = 1; i < colors.Count; i++) {
            vfx.SetVector4("Portal Edge Color " + i, colors[i]);
            vfx.SetVector4("Portal Color " + i, colors[i]);
        }
    }
    /// <summary>
    /// Progression of checkpoints
    /// </summary>
    /// <param name="prog"></param>
    public void UpdateCPProgression(int prog) {
        if (prog < checkpointProgression) return;
        checkpointProgression = prog;
    }
    public bool PlayerProgressionIsGreaterThan(int prog) {
        return prog <= checkpointProgression;
    }

    public void ReloadLevel() {
        SceneManagement.Instance.LoadingRendering(scene.TargetScene, scene.AdditiveScene);
    }
    void OnGameStateChanged(GameState gameState) {
        switch (gameState) {
            case GameState.Pause:
                SoundsManager.Instance.PauseCurrentMusic();
                break;
            case GameState.InLevel:
                SoundsManager.Instance.UnPauseCurrentMusic();
                UIManager.Instance.DisplayGems(false);
                break;
        }
    }
}

