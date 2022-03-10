using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public enum GameState { MainMenu, InGame, Pause, Credits}
public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }
    public delegate void GameStateChangeHandler(GameState newState);
    public event GameStateChangeHandler OnGameStateChanged;
    void Awake(){
        if (Instance == null) Instance = this;
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
}
