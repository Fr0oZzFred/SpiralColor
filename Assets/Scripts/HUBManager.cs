using System.Collections;
using System;
using UnityEngine;

public class HUBManager : MonoBehaviour {
    #region Fields
    [Header("General")]
    [SerializeField] PlayerHandler playerHandler;
    [SerializeField] string musicName;


    [Header("Gems Pool")]
    public GemsPool gemsPool;
    [SerializeField] float secondsBetweenSpawn, secondsAfterSpawn;


    [Header("Level Screen")]
    [SerializeField] GameObject levelScreen;
    [SerializeField] DynamicATH levelScreenAth;


    [Header("Camera")]
    [SerializeField] GameObject gemsCam;
    [SerializeField] GameObject playerCam;
    [SerializeField] GameObject portalCam;
    [SerializeField] GameObject levelScreenCam;

    
    public static HUBManager Instance { get; private set; }

    public Controller CurrentController {
        get {
            return playerHandler.CurrentPlayer;
        }
    }

    public bool playerInSelection;

    Action Action;

    LevelRow[] levelRow;
    #endregion
    private void Awake() {
        if (!Instance) Instance = this;
        GameManager.Instance.SetState(GameState.InHUB);
        SoundsManager.Instance.StopCurrentMusic();
        SoundsManager.Instance.Play(musicName);
    }

    private IEnumerator Start() {
        InitLevelScreen();
        CheckLevelRow();

        //CheckForNewGems
        if (gemsPool.GemsCount >= GameManager.Instance.GemsCount) yield break;
        SwitchCam(gemsCam, playerCam);
        playerHandler.CurrentPlayer.RegisterInputs(false);
        for (int i = gemsPool.GemsCount; i < GameManager.Instance.GemsCount; i++) {
            gemsPool.Spawn();
            yield return new WaitForSeconds(secondsBetweenSpawn);
        }
        yield return new WaitForSeconds(secondsAfterSpawn);
        SwitchCam(playerCam, gemsCam);
        playerHandler.CurrentPlayer.RegisterInputs(true);
    }

    private void Update() {
        if(Action != null)
        Action();
    }

    #region Level Screen UI
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
    #endregion

    #region Portal Area

    public void OnPortalAreaEnter() {
        SwitchCam(portalCam, playerCam);
        playerHandler.CurrentPlayer.SetInputSpace(portalCam.transform);
    }

    public void OnPortalAreaExit() {
        SwitchCam(playerCam, portalCam);
        playerHandler.CurrentPlayer.SetInputSpace(playerCam.transform);
    }

    #endregion


    #region Level Screen Area

    public void OnLevelScreenAreaEnter() {
        Action += InLevelSelectionRange;
    }

    public void OnLevelScreenAreaExit() {
        Action -= InLevelSelectionRange;
    }

    void InLevelSelectionRange() {
        levelScreenAth.DisplayATH(true);
        if (InputHandler.Controller.buttonWest.wasPressedThisFrame) {
            levelScreenAth.DisplayATH(false);
            Action -= InLevelSelectionRange;
            Action += InLevelSelection;
            SwitchCam(levelScreenCam, portalCam);
            playerInSelection = true;
            playerHandler.CurrentPlayer.RegisterInputs(false);
            UIManager.Instance.SetEventSystemCurrentSelectedGO(levelRow[GameManager.Instance.Progression - 1].gameObject);
        }
    }

    void InLevelSelection() {
        if (InputHandler.Controller.buttonEast.wasPressedThisFrame ||
            InputHandler.Controller.buttonSouth.wasPressedThisFrame) {
            levelScreenAth.DisplayATH(true);
            Action -= InLevelSelection;
            Action += InLevelSelectionRange;
            SwitchCam(portalCam, levelScreenCam);
            playerHandler.CurrentPlayer.RegisterInputs(true);
            UIManager.Instance.SetEventSystemCurrentSelectedGO(null);
        }
    }
    #endregion
    void SwitchCam(GameObject newCam, GameObject oldCam) {
        newCam.SetActive(true);
        oldCam.SetActive(false);
    }
}
