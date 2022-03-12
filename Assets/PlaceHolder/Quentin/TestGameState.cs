using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestGameState : MonoBehaviour {
    void Update() {
        if (Keyboard.current.spaceKey.wasPressedThisFrame) {
            GameState currentState = GameManager.Instance.CurrentState;
            GameState newState = currentState == GameState.InGame ? GameState.Pause : GameState.InGame;
            GameManager.Instance.SetState(newState);
        }
    }
    void Start() {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }
    void OnGameStateChanged(GameState newState) {
        if (newState == GameState.InGame) Debug.Log("le gamestate est en play");
        else Debug.Log("le gamestate est en pause");
    }
}
