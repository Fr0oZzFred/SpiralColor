using System.Collections;
using UnityEngine;

public class HUBManager : MonoBehaviour {

    [SerializeField] PlayerHandler playerHandler;
    public static HUBManager Instance { get; private set; }

    [SerializeField] string musicName;

    public GemsPool gemsPool;

    [SerializeField] float secondsBetweenSpawn, secondsAfterSpawn;

    [SerializeField] GameObject cam, playerCam, levelScreenCam;
    [SerializeField] GameObject levelScreen;
    [SerializeField] float detectionRange = 5f;
    bool closeToLevelScreen;

    LevelRow[] levelRow;
    private void Awake() {
        if (!Instance) Instance = this;
        GameManager.Instance.SetState(GameState.InHUB);
        SoundsManager.Instance.StopCurrentMusic();
        SoundsManager.Instance.Play(musicName);
    }

    private IEnumerator Start() {
        InitLevelScreen();
        CheckLevelRow();
        if (gemsPool.GemsCount >= GameManager.Instance.GemsCount) yield break;
        playerHandler.CurrentPlayer.RegisterInputs(false);
        for (int i = gemsPool.GemsCount; i < GameManager.Instance.GemsCount; i++) {
            gemsPool.Spawn();
            yield return new WaitForSeconds(secondsBetweenSpawn);
        }
        yield return new WaitForSeconds(secondsAfterSpawn);
        playerHandler.CurrentPlayer.RegisterInputs(true);
    }
    private void Update() {
        Vector3 p = levelScreen.transform.position - playerHandler.CurrentPlayer.transform.position;
        if(!closeToLevelScreen && p.magnitude < detectionRange) {
            PlayerInRangeLevelScreen();
            closeToLevelScreen = true;
        } else if(closeToLevelScreen && p.magnitude > detectionRange) {
            PlayerOutRangeLevelScreen();
            closeToLevelScreen = false;
        } else if(p.magnitude < detectionRange) {
            CheckLevelScreenInput();
        } else {
            cam.SetActive(false);
        }
    }
    void InitLevelScreen() {
        levelRow = new LevelRow[levelScreen.transform.childCount];
        for (int i = 0; i < levelScreen.transform.childCount; i++) {
            levelRow[i] = levelScreen.transform.GetChild(i).gameObject.GetComponent<LevelRow>();
        }
    }

    private void CheckLevelRow() {
        for (int i = 0; i < levelRow.Length; i++) {
            if(i + 1 < GameManager.Instance.Progression) {
                levelRow[i].gameObject.SetActive(true);
                GameManager.Instance.GetCollectedGemsOfLevel(i + 1, out int collected, out int max);
                levelRow[i].SetGemsProgression(collected + " / " + max);
            } 
            else if (GameManager.Instance.Progression == i + 1) {
                levelRow[i].gameObject.SetActive(true);
                levelRow[i].SetGemsProgression("???");
                levelRow[i].InvokeButtonEvents();
            } 
            else {
                levelRow[i].gameObject.SetActive(false);
            }
        }
    }

    public void ClosePortal() {
        if (GameManager.Instance.CurrentState != GameState.InHUB) return;
        playerHandler.CurrentPlayer.SetInputSpace(cam.activeInHierarchy ? cam.transform : playerCam.transform);
        playerCam.SetActive(cam.activeInHierarchy);
        cam.SetActive(!cam.activeInHierarchy);
    }
    void PlayerInRangeLevelScreen() {
        //Display Dynamic UI
        //UIManager.Instance.SetEventSystemCurrentSelectedGO(levelRow[GameManager.Instance.Progression - 1].gameObject);
    }
    void PlayerOutRangeLevelScreen() {
        //Hide Dynamic UI
        UIManager.Instance.SetEventSystemCurrentSelectedGO(null);
    }
    void CheckLevelScreenInput() {
        cam.SetActive(true);
        if (InputHandler.Controller.buttonWest.wasPressedThisFrame && !levelScreenCam.activeInHierarchy) {
            cam.SetActive(false);
            levelScreenCam.SetActive(true);
            UIManager.Instance.SetEventSystemCurrentSelectedGO(levelRow[GameManager.Instance.Progression - 1].gameObject);
            playerHandler.CurrentPlayer.RegisterInputs(false);
        } else if (InputHandler.Controller.buttonEast.wasPressedThisFrame) {
            cam.SetActive(true);
            levelScreenCam.SetActive(false);
            UIManager.Instance.SetEventSystemCurrentSelectedGO(null);
            playerHandler.CurrentPlayer.RegisterInputs(true);
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(levelScreen.transform.position, detectionRange);
    }
}
