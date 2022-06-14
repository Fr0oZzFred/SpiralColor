using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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
    [Header("VisualEffect")]
    [SerializeField] VisualEffect changePlayerVFX;
    [SerializeField] AnimationCurve curvePlayer;
    [SerializeField] AnimationCurve curveShape;
    [SerializeField] Mesh playerMesh;

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

        changePlayerVFX = Instantiate(changePlayerVFX);
        changePlayerVFX.gameObject.SetActive(false);
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
        player.GetComponent<SphereCollider>().enabled = true;
        current.GetComponent<Rigidbody>().velocity = Vector3.zero;

        changePlayerVFX.SetMesh("Start Mesh", current.GetShape().GetComponent<MeshFilter>().mesh);
        changePlayerVFX.SetMesh("End Mesh", player.GetMesh);
        changePlayerVFX.SetAnimationCurve("Start Mesh Size Over Lifetime", curveShape);
        changePlayerVFX.SetAnimationCurve("End Mesh Size Over Lifetime", curvePlayer);

        Vector3 startPos = current.GetShape().position;
        Vector3 targetPos = current.transform.position + offset - startPos;
        changePlayerVFX.SetVector3("Target Position", targetPos);
        changePlayerVFX.SetVector3("Start Rotation", current.GetShape().rotation.eulerAngles);
        changePlayerVFX.SetVector3("End Rotation", player.GetShape().rotation.eulerAngles);
        changePlayerVFX.gameObject.transform.position = startPos;
        changePlayerVFX.gameObject.SetActive(true);

        yield return new WaitForSeconds(
            changePlayerVFX.GetFloat("Start Sphere Lifetime") +
            changePlayerVFX.GetFloat("Particles Lifetime") +
            changePlayerVFX.GetFloat("End Mesh Lifetime")
        );

        player.Respawn(current.transform.position + offset, current.GetCamRotation());
        current = null;
        player.GetShape().gameObject.SetActive(true);
        player.RegisterInputs(true);
        player.SetControllerLED();


        changePlayerVFX.gameObject.SetActive(false);
        CoroutineIsRunning = false;
    }

    IEnumerator PlayerIntoControllerQTE(Controller newController) {
        CoroutineIsRunning = true;
        player.RegisterInputs(false);
        newController.SetCamRotation(player.GetCamRotation());
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;

        changePlayerVFX.SetMesh("Start Mesh", playerMesh);
        changePlayerVFX.SetMesh("End Mesh", newController.GetShape().GetComponent<MeshFilter>().mesh);
        changePlayerVFX.SetAnimationCurve("Start Mesh Size Over Lifetime", curvePlayer);
        changePlayerVFX.SetAnimationCurve("End Mesh Size Over Lifetime", curveShape);

        Vector3 startPos = player.GetShape().position;
        Vector3 targetPos = newController.GetShape().position - startPos;
        changePlayerVFX.SetVector3("Target Position", targetPos);
        changePlayerVFX.SetVector3("Start Rotation", player.GetShape().rotation.eulerAngles);
        changePlayerVFX.SetVector3("End Rotation", newController.GetShape().rotation.eulerAngles);
        changePlayerVFX.gameObject.transform.position = startPos;
        changePlayerVFX.gameObject.SetActive(true);

        yield return new WaitForSeconds(changePlayerVFX.GetFloat("Start Sphere Lifetime") * 0.5f);

        player.GetComponent<SphereCollider>().enabled = false;
        player.GetShape().gameObject.SetActive(false);

        yield return new WaitForSeconds(
            changePlayerVFX.GetFloat("Start Sphere Lifetime") * 0.5f +
            changePlayerVFX.GetFloat("Particles Lifetime") +
            changePlayerVFX.GetFloat("End Mesh Lifetime")
        );

        if (newController is CrossPlayerController) {
            newController.RegisterInputs(!newController.GetComponent<CrossPlayerController>().IsOnButton);
        } else {
            newController.RegisterInputs(true);
        }
        newController.SetControllerLED();
        current = newController;

        changePlayerVFX.gameObject.SetActive(false);
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
