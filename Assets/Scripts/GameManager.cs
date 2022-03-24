using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
public enum GameState {Boot, Loading, Cutscene, MainMenu, InHUB, InLevel, Pause, Score, Credits }

[System.Serializable]
public class GameManagerData{
    public int progression;
    public Dictionary<string, bool> pieces;
    public GameManagerData(GameManager data) {
        progression = data.progression;
        pieces = data.pieces;
    }
}
public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }
    public delegate void GameStateChangeHandler(GameState newState);
    public event GameStateChangeHandler OnGameStateChanged;
    public int progression { get; private set; }
    public Dictionary<string, bool> pieces { get; private set; }
    public Piece piece1, piece2, piece3;
    void Awake(){
        if (Instance == null) Instance = this;
        progression = 0; // Temporaire
        pieces = new Dictionary<string, bool>();
        for (int i = 1; i < 16; i++) for (int j = 1; j < 4; j++) pieces.Add("Star " + i + "-" + j, false);
    }
    public void SetState (GameState newState){
        if (newState == CurrentState) return;
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }
    public void SaveGameManager() {
        SaveSystem.SaveGameManager(this);
    }

    public void LoadGameManager() {
        GameManagerData data = SaveSystem.LoadGameManager();
        progression = data.progression;
        pieces = data.pieces;
    }
    /// <summary>
    /// Update the Progression of the storyline
    /// </summary>
    /// <param name="prog"></param>
    public void UpdateProgression(int prog) {
        if (prog < progression) return;
        progression = prog;
    }
    public void CheckPiece(Piece piece) {
        pieces["Star " + LevelManager.Instance.LevelInt + "-" + piece.index] = true;
    }
}
