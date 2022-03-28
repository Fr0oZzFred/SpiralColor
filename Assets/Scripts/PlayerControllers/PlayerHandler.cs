using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour {
    
    public List<Controller> listController;
    public int startingIndex = 0;
    int index = 0;

    public OrbitCamera cam;
    private void Start() {
        index = startingIndex > listController.Count -1 ? 0 : startingIndex;
        for (int i = 0; i < listController.Count; i++) {
            listController[i].RegisterInputs(index == i);
        }
        cam.SetFocus(listController[index].transform);
        listController[index].SetControllerLED();

        if(GameManager.Instance)
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void Update() {
        if (InputHandler.Controller == null) return;
        if (listController.Count > 0) {
            if (InputHandler.Controller.rightShoulder.wasPressedThisFrame) {
                int oldIndex = index;
                index++;
                ChangePlayer(oldIndex);
            } else if (InputHandler.Controller.leftShoulder.wasPressedThisFrame) {
                int oldIndex = index;
                if (index > 0) index--;
                else index = listController.Count - 1;
                ChangePlayer(oldIndex);
            }
        }
    }

    private void OnGameStateChanged(GameState newState) {
        switch (newState) {
            case GameState.InLevel:
            case GameState.InHUB:
                CurrentPlayer.RegisterInputs(true);
                break;
            case GameState.Pause:
            case GameState.Score:
            case GameState.Cutscene:
                CurrentPlayer.RegisterInputs(false);
                break;
        }
    }

    /// <summary>
    /// Change the player who is controlled by the player
    /// </summary>
    /// <param name="oldIndex"></param>
    void ChangePlayer(int oldIndex) {
        index %= listController.Count;
        listController[oldIndex].RegisterInputs(false);
        listController[index].RegisterInputs(true);
        listController[index].SetControllerLED();
        cam.SetFocus(listController[index].transform);
    }

    public Controller CurrentPlayer {
        get {
            return listController[index];
        }
    }
    private void OnApplicationQuit() {
        InputHandler.SetControllerLED(Color.black);
    }
}
