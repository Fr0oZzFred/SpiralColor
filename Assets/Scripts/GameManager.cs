using UnityEngine;
using System.Collections.Generic;
public enum GameState {Boot, MainMenu, InHUB, InLevel, Pause, Score, Loading, Cutscene, Credits, Options, ControllerDisconnected }

[System.Serializable]
public class GameManagerData{
    public int progression;
    public Dictionary<string, bool> pieces;
    public GameManagerData(GameManager data) {
        progression = data.Progression;
        pieces = data.pieces;
    }
}
public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }
    public GameState OldState { get; private set; }
    public delegate void GameStateChangeHandler(GameState newState);
    public event GameStateChangeHandler OnGameStateChanged;
    public int Progression { get; private set; }
    public Dictionary<string, bool> pieces { get; private set; }
    private void Awake(){
        if (Instance == null) Instance = this;
        Init();
    }
    private void Update() {
        HandlePause();
    }
    private void Init() {
        Progression = 1;
        pieces = new Dictionary<string, bool>();
        for (int level = 1; level < 16; level++) {
            for (int starIndex = 1; starIndex < 4; starIndex++) {
                pieces.Add("Star " + level + "-" + starIndex, false);
            }
        }
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
    public void SaveGameManager() {
        SaveSystem.SaveGameManager(this);
    }

    public void LoadGameManager() {
        GameManagerData data = SaveSystem.LoadGameManager();
        Progression = data.progression;
        pieces = data.pieces;
    }
    /// <summary>
    /// Update the Progression of the storyline
    /// </summary>
    /// <param name="prog"></param>
    public void UpdateProgression(int prog) {
        if (prog < Progression) return;
        Progression = prog;
    }
    public void CollectStar(Star star) {
        pieces["Star " + LevelManager.Instance.LevelInt + "-" + star.StarIndex] = true;
    }

    public bool CheckStar(Star star) {
        return pieces["Star " + LevelManager.Instance.LevelInt + "-" + star.StarIndex];
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
    public void QuitApplication() {
        Application.Quit();
    }
    private void OnApplicationQuit() {
        Debug.LogWarning("Save have to be done here");
    }
}
