using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class HUBManager : MonoBehaviour {
    #region Fields
    [Header("General")]
    [SerializeField] PlayerHandler playerHandler;
    [SerializeField] string musicName;
    [SerializeField] MeshRenderer shipMeshRenderer;
    [SerializeField] Scenes creditScene;


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


    [Header("Doors")]
    [SerializeField] List<GameObject> orbs;
    const string colorPropertyRef = "_Tron";
    [SerializeField] List<Door> doors;
    [Serializable]
    struct Door {

        [SerializeField] [ColorUsage(true, true)] public Color colorOn;
        [SerializeField] [ColorUsage(true, true)] public Color colorOff;
        public MeshRenderer doorLeftMeshRenderer;
        public MeshRenderer doorRightMeshRenderer;
        public MeshRenderer socleMeshRenderer;
        public Animator animator;
        public int progressionRequirement;
    }



    public static HUBManager Instance { get; private set; }

    public Controller CurrentController {
        get {
            return playerHandler.CurrentPlayer;
        }
    }
    public bool PlayerInSelection {
        get {
            return playerInSelection;
        }
    }
    bool playerInSelection;

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
        playerInSelection = false;
        InitLevelScreen();
        CheckLevelRow();
        CheckDoors();
        CheckOrbs();

        //CheckForNewGems
        if (gemsPool.GemsCount < GameManager.Instance.GemsCount) {
            SwitchCam(gemsCam, playerCam);
            playerHandler.CurrentPlayer.RegisterInputs(false);
            for (int i = gemsPool.GemsCount; i < GameManager.Instance.GemsCount; i++) {
                gemsPool.Spawn();
                yield return new WaitForSeconds(secondsBetweenSpawn);
            }
            yield return new WaitForSeconds(secondsAfterSpawn);
            SwitchCam(playerCam, gemsCam);
        }
        //CheckForEvents
        switch (GameManager.Instance.Progression) {
            case 2:
                Debug.Log("Sphere Unlocked");
                break;
            case 4:
                Debug.Log("Triangle Unlocked");
                break;
            case 7:
                Debug.Log("Cube Unlocked");
                break;
            case 11:
                Debug.Log("Cross Unlocked");
                break;
            case 15:
                if (GameManager.Instance.GameDone && !GameManager.Instance.CreditSeenOnce) {
                    GameManager.Instance.CreditSeenOnce = true;
                    SceneManagement.Instance.LoadingRendering(creditScene.TargetScene, creditScene.AdditiveScene);
                    GameManager.Instance.SetState(GameState.Credits);
                }
                break;
        }
        playerHandler.CurrentPlayer.RegisterInputs(true);
        yield return null;
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
            if(i + 1 < GameManager.Instance.Progression || GameManager.Instance.GameDone) {
                levelRow[i].gameObject.SetActive(true);
                GameManager.Instance.GetCollectedGemsOfLevel(i + 1, out int collected, out int max);
                levelRow[i].SetGemsProgression(collected + " / " + max);
                if (GameManager.Instance.GameDone) return;
                levelRow[i].InvokeButtonEvents();
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
        levelScreenAth.DisplayATH(true);
        Action += InLevelSelectionRange;
    }

    public void OnLevelScreenAreaExit() {
        levelScreenAth.DisplayATH(false);
        Action -= InLevelSelectionRange;
    }

    void InLevelSelectionRange() {
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

    #region Doors
    void CheckDoors() {
        for (int i = 0; i < doors.Count; i++) {
            bool isUnlocked = GameManager.Instance.Progression >= doors[i].progressionRequirement;
            doors[i].doorLeftMeshRenderer.material.color = isUnlocked ? doors[i].colorOn : doors[i].colorOff;
            doors[i].doorRightMeshRenderer.material.color = isUnlocked ? doors[i].colorOn : doors[i].colorOff;
            doors[i].socleMeshRenderer.material.color = isUnlocked ? doors[i].colorOn : doors[i].colorOff;
            shipMeshRenderer.material.SetColor(colorPropertyRef + (i + 1), isUnlocked ? doors[i].colorOn : doors[i].colorOff);
        }
    }
    void CheckOrbs() {
        if (GameManager.Instance.Progression <= 1) return;
        //Fais en brute parce que short en temps mais il faudrait le changer parce que c'est moche et pas ouf
        orbs[0].SetActive(GameManager.Instance.Progression >=2);
        orbs[1].SetActive(GameManager.Instance.Progression == 3);
        orbs[2].SetActive(GameManager.Instance.Progression >= 4);
        orbs[3].SetActive(GameManager.Instance.Progression == 5 || GameManager.Instance.Progression == 6);
        orbs[4].SetActive(GameManager.Instance.Progression == 6);
        orbs[5].SetActive(GameManager.Instance.Progression >= 7);
        orbs[6].SetActive(GameManager.Instance.Progression >= 8 && GameManager.Instance.Progression <=10);
        orbs[7].SetActive(GameManager.Instance.Progression == 9 | GameManager.Instance.Progression == 10);
        orbs[8].SetActive(GameManager.Instance.Progression == 10);
        orbs[9].SetActive(GameManager.Instance.Progression == 11);
    }
    public void OpenDoor(int doorIndex) {
        if(GameManager.Instance.Progression >= doors[doorIndex].progressionRequirement) {
            doors[doorIndex].animator.SetBool("Open", true);
            SoundsManager.Instance.Play("Door");
        }
    }

    public void CloseDoor(int doorIndex) {
        doors[doorIndex].animator.SetBool("Open", false);
        SoundsManager.Instance.Play("Door");
    }
    #endregion

    void SwitchCam(GameObject newCam, GameObject oldCam) {
        newCam.SetActive(true);
        oldCam.SetActive(false);
    }
}
