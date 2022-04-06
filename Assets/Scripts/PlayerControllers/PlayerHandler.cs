using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour {
    
    [SerializeField] PlayerController player;

    [SerializeField] List<Controller> listController;
    private List<Controller> listControllerInRange;

    [SerializeField] float rangeDetection = 2f;


    public Controller CurrentPlayer {
        get {
            return current == null ? player : current;
        }
    }
    Controller current;

    private void Start() {
        if (GameManager.Instance)
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;

        if (listController.Count == 0) return;
        listControllerInRange = new List<Controller>();
        for (int i = 0; i < listController.Count; i++) {
            listController[i].RegisterInputs(false);
        }
        player.RegisterInputs(true);
        player.SetControllerLED();
    }

    private void Update() {
        if (InputHandler.Controller == null) return;
        CheckForControllerInRange();
        CheckForChange();
    }

    void CheckForControllerInRange() {
        if (listController.Count > 0) {
            if (player.isCurrentlyPlayed) {
                foreach (var item in listController) {
                    Vector3 p = item.transform.position - player.transform.position;
                    if (p.magnitude < rangeDetection) {
                        listControllerInRange.Add(item);
                    } else if (listControllerInRange.Contains(item)) {
                        listControllerInRange.Remove(item);
                    }
                }
            }
        }
    }

    void CheckForChange() {
        if(CurrentPlayer == player && listControllerInRange.Count > 0) {
            foreach (var item in listControllerInRange) {
                if (item is SpherePlayerController) {
                    if (InputHandler.Controller.buttonEast.wasPressedThisFrame) {
                        ChangePlayer(item);
                    }
                }
                else if (item is TrianglePlayerController) {
                    if (InputHandler.Controller.buttonNorth.wasPressedThisFrame) {
                        ChangePlayer(item);
                    }
                }
                else if (item is SquarePlayerController) {
                    if (InputHandler.Controller.buttonWest.wasPressedThisFrame) {
                        ChangePlayer(item);
                    }
                }
                else if (item is CrossPlayerController) {
                    if (InputHandler.Controller.buttonSouth.wasPressedThisFrame) {
                        ChangePlayer(item);
                    }
                }
            }
        } else if(CurrentPlayer != player) {
            if (InputHandler.Controller.leftShoulder.wasPressedThisFrame ||
                    InputHandler.Controller.rightShoulder.wasPressedThisFrame) {
                ChangePlayer(player);
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
            case GameState.ControllerDisconnected:
                CurrentPlayer.RegisterInputs(false);
                break;
        }
    }

    /// <summary>
    /// Change the player who is controlled by the player
    /// </summary>
    /// <param name="oldIndex"></param>
    void ChangePlayer(Controller newController) {
        if(player == newController) {
            current.RegisterInputs(false);
            player.Respawn(current.transform.position + new Vector3(0, 2, 0));
            current = null;
            player.RegisterInputs(true);
            player.SetControllerLED();
        } else {
            player.RegisterInputs(false);
            if (newController is CrossPlayerController) {
                newController.RegisterInputs(!newController.GetComponent<CrossPlayerController>().IsOnButton);
            } else {
                newController.RegisterInputs(true);
            }
            newController.SetControllerLED();
            current = newController;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(player.transform.position, rangeDetection);
        if (listController.Count == 0) return;
        foreach (var item in listController) {
            Gizmos.DrawWireSphere(item.transform.position, rangeDetection);
        }
    }

    private void OnApplicationQuit() {
        InputHandler.SetControllerLED(Color.black);
    }
}
