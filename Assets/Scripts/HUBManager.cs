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
    [SerializeField] List<GameObject> timelines;
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
    public bool CoroutineIsRunning {
        get {
            return playerHandler.CoroutineIsRunning;
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
        CheckOrbs();

        UIManager.Instance.ActivateLevelScreenBlackBoards(false);
        UIManager.Instance.ActivateBlackBoard(true);
        //CheckForNewGems
        if (gemsPool.GemsCount < GameManager.Instance.GemsCount) {
            SwitchCam(gemsCam, playerCam);
            playerHandler.CurrentPlayer.RegisterInputs(false);
            UIManager.Instance.UpdateCamera();
            for (int i = gemsPool.GemsCount; i < GameManager.Instance.GemsCount; i++) {
                gemsPool.Spawn(GameManager.Instance.GemsTypesIndex[i]);
                yield return new WaitForSeconds(secondsBetweenSpawn);
            }
            yield return new WaitForSeconds(secondsAfterSpawn);
            SwitchCam(playerCam, gemsCam);
            UIManager.Instance.UpdateCamera();
        }
        
    

        //CheckForEvents
        switch (GameManager.Instance.Progression) {
            case 2:
                //Sphere Unlocked
                timelines[0].SetActive(true);
                UIManager.Instance.UpdateCamera();
                yield return new WaitForSeconds(7f);
                break;
            case 4:
                //Triangle Unlocked
                timelines[1].SetActive(true);
                UIManager.Instance.UpdateCamera();
                yield return new WaitForSeconds(7f);
                break;
            case 7:
                //Cube Unlocked
                timelines[2].SetActive(true);
                UIManager.Instance.UpdateCamera();
                yield return new WaitForSeconds(7f);
                break;
            case 11:
                //Cross Unlocked
                timelines[3].SetActive(true);
                UIManager.Instance.UpdateCamera();
                yield return new WaitForSeconds(7f);
                break;
            case 15:
                if (GameManager.Instance.GameDone && !GameManager.Instance.CreditSeenOnce) {
                    timelines[4].SetActive(true);
                    UIManager.Instance.UpdateCamera();
                    yield return new WaitForSeconds(7f);
                    SaveSystem.SaveGemmes(gemsPool);
                    GameManager.Instance.CreditSeenOnce = true;
                    SceneManagement.Instance.LoadCreditsWithRendering();
                    GameManager.Instance.SetState(GameState.Credits);
                }
                CheckDoors();
                break;
            default:
                CheckDoors();
                break;
        }
        UIManager.Instance.ActivateBlackBoard(false);
        playerHandler.CurrentPlayer.RegisterInputs(true);
        UIManager.Instance.UpdateCamera();
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
                if (GameManager.Instance.GameDone) {
                    levelRow[i].InvokeButtonEvents();
                }
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
        UIManager.Instance.UpdateCamera();
        playerHandler.CurrentPlayer.SetInputSpace(portalCam.transform);
    }

    public void OnPortalAreaExit() {
        SwitchCam(playerCam, portalCam);
        UIManager.Instance.UpdateCamera();
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
            UIManager.Instance.ActivateLevelScreenBlackBoards(true);
            Action -= InLevelSelectionRange;
            Action += InLevelSelection;
            SwitchCam(levelScreenCam, portalCam);
            UIManager.Instance.UpdateCamera();
            playerInSelection = true;
            playerHandler.CurrentPlayer.RegisterInputs(false);
            UIManager.Instance.SetEventSystemCurrentSelectedGO(levelRow[GameManager.Instance.Progression - 1].gameObject);
            HUBManager.Instance.CurrentController.GetShape().gameObject.SetActive(false);
        }
    }

    void InLevelSelection() {
        if (InputHandler.Controller.buttonEast.wasPressedThisFrame ||
            InputHandler.Controller.buttonSouth.wasPressedThisFrame) {
            levelScreenAth.DisplayATH(true);
            UIManager.Instance.ActivateLevelScreenBlackBoards(false);
            Action -= InLevelSelection;
            Action += InLevelSelectionRange;
            SwitchCam(portalCam, levelScreenCam);
            UIManager.Instance.UpdateCamera();
            playerHandler.CurrentPlayer.RegisterInputs(true);
            UIManager.Instance.SetEventSystemCurrentSelectedGO(null);
            HUBManager.Instance.CurrentController.GetShape().gameObject.SetActive(true);
        }
    }
    #endregion

    #region Doors
    public void CheckDoors() {
        for (int i = 0; i < doors.Count -1; i++) {
            bool isUnlocked = GameManager.Instance.Progression >= doors[i].progressionRequirement;
            doors[i].socleMeshRenderer.material.color = isUnlocked ? doors[i].colorOn : doors[i].colorOff;
            doors[i].doorLeftMeshRenderer.material.color = isUnlocked ? doors[i].colorOn : doors[i].colorOff;
            doors[i].doorRightMeshRenderer.material.color = isUnlocked ? doors[i].colorOn : doors[i].colorOff;
            shipMeshRenderer.material.SetColor(colorPropertyRef + (i + 1), isUnlocked ? doors[i].colorOn : doors[i].colorOff);
        }
        doors[4].socleMeshRenderer.material.color = GameManager.Instance.GameDone ? doors[4].colorOn : doors[4].colorOff;
    }
    void CheckOrbs() {
        if (GameManager.Instance.Progression <= 1) {
            foreach (var item in orbs) {
                item.SetActive(false);
            }
            return;
        }
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
        orbs[9].SetActive(GameManager.Instance.Progression >= 11);
        orbs[10].SetActive(GameManager.Instance.Progression >= 12 && GameManager.Instance.Progression <= 15 && !GameManager.Instance.GameDone);
        orbs[11].SetActive(GameManager.Instance.Progression >= 13 && GameManager.Instance.Progression <= 15 && !GameManager.Instance.GameDone);
        orbs[12].SetActive(GameManager.Instance.Progression == 14 || GameManager.Instance.Progression == 15 && !GameManager.Instance.GameDone);
        orbs[13].SetActive(GameManager.Instance.Progression == 15 && !GameManager.Instance.GameDone);
        orbs[14].SetActive(GameManager.Instance.GameDone);
    }
    public void OpenDoor(int doorIndex) {
        if(GameManager.Instance.Progression >= doors[doorIndex].progressionRequirement) {
            doors[doorIndex].animator.SetBool("Open", true);
            SoundsManager.Instance.Play("Door");
        }
    }

    public void CloseDoor(int doorIndex) {
        if (GameManager.Instance.Progression >= doors[doorIndex].progressionRequirement) {
            doors[doorIndex].animator.SetBool("Open", false);
            SoundsManager.Instance.Play("Door");
        }
    }
    #endregion

    void SwitchCam(GameObject newCam, GameObject oldCam) {
        newCam.SetActive(true);
        oldCam.SetActive(false);
    }
}
