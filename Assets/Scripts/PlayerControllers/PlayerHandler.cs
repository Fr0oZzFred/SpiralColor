using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour {

    [SerializeField] PlayerController player;

    [SerializeField] List<Controller> listController;
    private List<Controller> listControllerInRange;

    [SerializeField] float rangeDetection = 2f;

    [SerializeField] Vector3 offset = new Vector3(0, 2, 0);

    public bool CoroutineIsRunning { get; private set; }
    public Controller CurrentPlayer {
        get {
            return current == null ? player : current;
        }
    }
    Controller current;

    private void Start() {
        if (GameManager.Instance) {
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        }

        listControllerInRange = new List<Controller>();

        player.RegisterInputs(true);
        player.SetControllerLED();

        if (listController.Count == 0) return;
        for (int i = 0; i < listController.Count; i++) {
            listController[i].RegisterInputs(false);
        }
        CoroutineIsRunning = false;
    }

    private void Update() {
        if (InputHandler.Controller == null) return;
        if (GameManager.Instance && GameManager.Instance.CurrentState == GameState.Pause) return;
        CheckForControllerInRange();
        CheckForControllerOutRange();
        CheckForChange();
    }

    void CheckForControllerInRange() {
        if (listController.Count <= 0) return;
        if (!player.IsCurrentlyPlayed) return;

        foreach (var item in listController) {
            Vector3 p = item.transform.position - player.transform.position;
            if (p.magnitude < rangeDetection && !listControllerInRange.Contains(item)) {
                listControllerInRange.Add(item);
                item.DisplayATH(true);
            }
        }
    }

    void CheckForControllerOutRange() {
        if (listControllerInRange.Count <= 0) return;
        if (!player.IsCurrentlyPlayed) return;

        List<Controller> outOfRangeControllers = new List<Controller>();
        foreach (var item in listControllerInRange) {
            Vector3 p = item.transform.position - player.transform.position;
            if (p.magnitude > rangeDetection) {
                outOfRangeControllers.Add(item);
            }
        }
        foreach (var item in outOfRangeControllers) {
            listControllerInRange.Remove(item);
            item.DisplayATH(false);
        }
    }

    void CheckForChange() {
        if (CoroutineIsRunning) return;

        if (CurrentPlayer == player) {
            foreach (var item in listControllerInRange) {
                if (item is SpherePlayerController) {
                    if (InputHandler.Controller.buttonEast.wasPressedThisFrame) {
                        ChangePlayer(item);
                        listControllerInRange.Clear();
                        return;
                    }
                } else if (item is TrianglePlayerController) {
                    if (InputHandler.Controller.buttonNorth.wasPressedThisFrame) {
                        ChangePlayer(item);
                        listControllerInRange.Clear();
                        return;
                    }
                } else if (item is SquarePlayerController && !item.GetComponent<SquarePlayerController>().IsOnButton) {
                    if (InputHandler.Controller.buttonWest.wasPressedThisFrame) {
                        ChangePlayer(item);
                        listControllerInRange.Clear();
                        return;
                    }
                } else if (item is CrossPlayerController && !item.GetComponent<CrossPlayerController>().IsOnButton) {
                    if (InputHandler.Controller.buttonSouth.wasPressedThisFrame) {
                        ChangePlayer(item);
                        listControllerInRange.Clear();
                        return;
                    }
                }
            }
        } else if (CurrentPlayer != player) {
            if (CurrentPlayer.GetComponent<SquarePlayerController>() &&
                CurrentPlayer.GetComponent<SquarePlayerController>().IsOnButton) {
                return;
            } else if (CurrentPlayer.GetComponent<CrossPlayerController>() &&
                  CurrentPlayer.GetComponent<CrossPlayerController>().IsOnButton) {
                return;

            }
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
                if (CurrentPlayer)
                    CurrentPlayer.RegisterInputs(true);
                break;
            case GameState.Pause:
            case GameState.Score:
            case GameState.Cutscene:
            case GameState.ControllerDisconnected:
                if (CurrentPlayer)
                    CurrentPlayer.RegisterInputs(false);
                break;
        }
    }

    /// <summary>
    /// Change the player who is controlled by the player
    /// </summary>
    /// <param name="oldIndex"></param>
    void ChangePlayer(Controller newController) {
        foreach (var item in listControllerInRange) {
            item.DisplayATH(false);
        }
        if (player == newController) {
            StartCoroutine(SpawnPlayerQTE());
        } else {
            StartCoroutine(PlayerIntoControllerQTE(newController));
        }
    }
    IEnumerator SpawnPlayerQTE() {
        CoroutineIsRunning = true;

        current.RegisterInputs(false);
        yield return new WaitForSeconds(5f);
        player.Respawn(current.transform.position + offset, current.GetCamRotation());
        current = null;
        player.RegisterInputs(true);
        player.SetControllerLED();


        CoroutineIsRunning = false;
    }
    IEnumerator PlayerIntoControllerQTE(Controller newController) {
        CoroutineIsRunning = true;

        player.RegisterInputs(false);
        yield return new WaitForSeconds(5f);
        newController.SetCamRotation(player.GetCamRotation());
        if (newController is CrossPlayerController) {
            newController.RegisterInputs(!newController.GetComponent<CrossPlayerController>().IsOnButton);
        } else {
            newController.RegisterInputs(true);
        }
        newController.SetControllerLED();
        current = newController;

        CoroutineIsRunning = false;
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
