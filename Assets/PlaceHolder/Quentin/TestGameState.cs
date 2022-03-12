using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestGameState : MonoBehaviour {
    void Update() {
        if (Keyboard.current.spaceKey.wasPressedThisFrame) {
            GameState currentState = GameManager.Instance.CurrentState;
            GameState newState = currentState == GameState.play ? GameState.pause : GameState.play;
            GameManager.Instance.SetState(newState);
        }
    }
    void Start() {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }
    void OnGameStateChanged(GameState newState) {
        if (newState == GameState.play) Debug.Log("le gamestate est en play");
        else Debug.Log("le gamestate est en pause");
    }
}
