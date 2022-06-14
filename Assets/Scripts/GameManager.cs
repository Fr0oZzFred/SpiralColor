using UnityEngine;
using System.Collections.Generic;
public enum GameState {Boot, MainMenu, InHUB, InLevel, Pause, Score, Loading, Cutscene, Credits, Options, ControllerDisconnected, Keyboard }

[System.Serializable]
public class GameManagerData{
    public int progression;
    public List<List<bool>> gemsList;
    public bool gameDone;
    public bool creditSeenOnce;
    public int[] gemsTypesIndex;
    public GameManagerData(GameManager data) {
        progression = data.Progression;
        gemsList = data.gemsList;
        gameDone = data.GameDone;
        creditSeenOnce = data.CreditSeenOnce;
        for(int i = 0; i < data.GemsTypesIndex.Count; i++) gemsTypesIndex[i] = data.GemsTypesIndex[i];
    }
}
public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }
    public GameState OldState { get; private set; }
    public delegate void GameStateChangeHandler(GameState newState);
    public event GameStateChangeHandler OnGameStateChanged;
    public int Progression { get; private set; }
    public List<List<bool>> gemsList { get; private set; }
    public List<int> GemsTypesIndex { get; private set; }
    public int GemsCount { get { return GemsTypesIndex.Count; } }
    public string Username { get; private set; }
    public bool GameDone { get; private set; }
    public bool CreditSeenOnce { get; set; }
    private void Awake() {
        if (Instance == null) Instance = this;
        Username = "";
        gemsList = new List<List<bool>>();
        GemsTypesIndex = new List<int>();
        for (int level = 0; level < 16; level++) {
            gemsList.Add(new List<bool>());
        }
        Init();
    }
    private void Update() {
        if (InputHandler.Controller == null) return;
        if (InputHandler.Controller.startButton.wasPressedThisFrame) {
            HandlePause();
        }
        if (InputHandler.Controller.buttonEast.wasPressedThisFrame) HandleBack();
    }
    private void Init() {
        Progression = 1;
        GameDone = false;
    }
    public void SetState (GameState newState){
        if (newState == CurrentState) return;
        OldState = CurrentState;
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }

    public void SetState(string newState) {
        switch (newState) {
            case "Boot":
                SetState(GameState.Boot);
                break;
            case "MainMenu":
                SetState(GameState.MainMenu);
                break;
            case "InHUB":
                SetState(GameState.InHUB);
                break;
            case "InLevel":
                SetState(GameState.InLevel);
                break;
            case "Pause":
                SetState(GameState.Pause);
                break;
            case "Score":
                SetState(GameState.Score);
                break;
            case "Loading":
                SetState(GameState.Loading);
                break;
            case "Cutscene":
                SetState(GameState.Cutscene);
                break;
            case "Credits":
                SetState(GameState.Credits);
                break;
            case "Options":
                SetState(GameState.Options);
                break;
            default:
                Debug.LogWarning(newState + "does not exist as a GameState");
                break;
        }
    }
    public void SetOldState() {
        SetState(OldState);
    }
    public void SaveGameManager() {
        SaveSystem.SaveGameManager(this);
    }

    public void LoadGameManager() {
        GameManagerData data = SaveSystem.LoadGameManager();
        Progression = data.progression;
        if (data.gemsList != null) gemsList = data.gemsList;
        GameDone = data.gameDone;
        CreditSeenOnce = data.creditSeenOnce;
        for (int i = 0; i < data.gemsTypesIndex.Length; i++) GemsTypesIndex.Add(data.gemsTypesIndex[i]);
    }
    /// <summary>
    /// Update the Progression of the storyline
    /// </summary>
    /// <param name="prog"></param>
    public void UpdateProgression(int prog) {
        if (prog < Progression) return;
        if (prog > 15) {
            GameDone = true;
            return;
        }
        Progression = prog;
    }
    public void AddGem() {
        gemsList[LevelManager.Instance.LevelInt].Add(false);
    }
    public void CollectGem(int index, GemsTypes type) {
        gemsList[LevelManager.Instance.LevelInt][index] = true;
        GemsTypesIndex.Add((int)type);
    }
    public bool CheckGem(int index) {
        return gemsList[LevelManager.Instance.LevelInt][index];
    }
    public void GetCollectedGemsOfLevel(int level, out int collected, out int max ) {
        collected = 0;
        max = 0;
        if (gemsList[level].Count == 0) return;
        foreach (var isCollected in gemsList[level]) {
            if (isCollected) collected ++;
            max++;
        }
    }
    public void HandlePause() {
        if (CurrentState == GameState.Boot) return;
        if (CurrentState == GameState.MainMenu) return;
        if (CurrentState == GameState.Score) return;
        if (CurrentState == GameState.Loading) return;
        if (CurrentState == GameState.Options) return;
        if (CurrentState == GameState.Credits) return;
        if (CurrentState == GameState.Pause) {
            if(OldState == GameState.Options) {
                if (HUBManager.Instance) {
                    SetState(GameState.InHUB);
                    return;
                } else if (LevelManager.Instance) {
                    SetState(GameState.InLevel);
                    return;
                }
            }
            SetOldState();
            SoundsManager.Instance.Play("ButtonClick");
        } else {
            SetState(GameState.Pause);
            SoundsManager.Instance.Play("ButtonClick");
        }
    }
    public void HandleBack() {
        if (CurrentState == GameState.Options) {
            SetOldState();
            SoundsManager.Instance.Play("ButtonClick");
        }
        if (CurrentState == GameState.Credits) {
            SceneManagement.Instance.LoadMainMenuWithRendering();
            SetState(GameState.MainMenu);
        }
    }
    public void ApplyUsername(string name) {
        Username = name;
    }
    public void QuitApplication() {
        Application.Quit();
    }
    private void OnApplicationQuit() {
        SaveGameManager();
    }
}
