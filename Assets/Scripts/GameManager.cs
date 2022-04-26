using UnityEngine;
using System.Collections.Generic;
public enum GameState {Boot, MainMenu, InHUB, InLevel, Pause, Score, Loading, Cutscene, Credits, Options, ControllerDisconnected, Keyboard }

[System.Serializable]
public class GameManagerData{
    public int progression;
    public List<List<bool>> gemsList;
    public int gemsCount;
    public GameManagerData(GameManager data) {
        progression = data.Progression;
        gemsList = data.gemsList;
        gemsCount = data.gemsCount;
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
    public int gemsCount = 0;
    public string Username { get; private set; }
    private void Awake(){
        if (Instance == null) Instance = this;
        Username = "";
        gemsList = new List<List<bool>>();
        for (int level = 0; level < 15; level++) {
            gemsList.Add(new List<bool>());
        }
        Init();
    }
    private void Update() {
        HandlePause();
    }
    private void Init() {
        Progression = 0;
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
        gemsCount = data.gemsCount;
    }
    /// <summary>
    /// Update the Progression of the storyline
    /// </summary>
    /// <param name="prog"></param>
    public void UpdateProgression(int prog) {
        if (prog < Progression) return;
        Progression = prog;
    }
    public void AddGem() {
        gemsList[LevelManager.Instance.LevelInt].Add(false);
    }
    public void CollectGem(int index) {
        gemsList[LevelManager.Instance.LevelInt][index] = true;
        gemsCount++;
    }
    public int GemHub() {
        return gemsCount;
    }
    public bool CheckGem(int index) {
        return gemsList[LevelManager.Instance.LevelInt][index];
    }
    public void HandlePause() {
        if (InputHandler.Controller == null) return;
        if (CurrentState == GameState.Boot) return;
        if (CurrentState == GameState.MainMenu) return;
        if (CurrentState == GameState.Score) return;
        if (CurrentState == GameState.Loading) return;
        if (InputHandler.Controller.startButton.wasPressedThisFrame) {
            if (CurrentState == GameState.Pause) {
                SetState(OldState);
            } else {
                SetState(GameState.Pause);
            }
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
        Debug.LogWarning("Save have to be done here");
    }
}
