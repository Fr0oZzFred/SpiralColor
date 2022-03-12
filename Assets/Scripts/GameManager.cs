using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GameState { play, pause}
public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }
    public delegate void GameStateChangeHandler(GameState newState);
    public event GameStateChangeHandler OnGameStateChanged;
    void Awake(){
        if (Instance == null) Instance = this;
    }
    public void SetState (GameState newState){
        if (newState == CurrentState) return;
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
        switch (CurrentState) {
            case GameState.play:
                break;
            case GameState.pause:
                UIManager.Instance.pauseMenu.SetActive(true);
                break;
        }
    }
}
