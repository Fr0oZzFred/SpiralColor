using UnityEngine;
using UnityEngine.InputSystem;
public enum GameState {Boot, MainMenu, InGame, Pause, Score, Credits }

[System.Serializable]
public class GameManagerData{
    public int progression;
    public GameManagerData(GameManager data) {
        progression = data.progression;
    }
}
public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }
    public delegate void GameStateChangeHandler(GameState newState);
    public event GameStateChangeHandler OnGameStateChanged;
    public int progression { get; private set; }
    void Awake(){
        if (Instance == null) Instance = this;
        progression = 0; // Temporaire
    }
    void Update() {
        if (Keyboard.current.spaceKey.wasPressedThisFrame) {
            GameState newState = CurrentState == GameState.InGame ? GameState.Pause : GameState.InGame;
            SetState(newState);
        }
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
    }
    /// <summary>
    /// Update the Progression of the storyline
    /// </summary>
    /// <param name="prog"></param>
    public void UpdateProgression(int prog) {
        if (prog < progression) return;
        progression = prog;
    }
}
