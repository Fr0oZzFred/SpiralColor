using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HUBManager : MonoBehaviour {

    [SerializeField] PlayerHandler playerHandler;
    public static HUBManager Instance { get; private set; }

    [SerializeField] string musicName;

    public GemsPool gemsPool;

    [SerializeField] float secondsBetweenSpawn, secondsAfterSpawn;

    [SerializeField] GameObject cam, playerCam;
    [SerializeField] GameObject levelScreen;

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
        if (InputHandler.Controller == null) return;
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
                GameManager.Instance.GetCollectedGemsOfLevel(i, out int collected, out int max);
                levelRow[i].SetGemsProgression(collected + " / " + max);
            } 
            else if (GameManager.Instance.Progression == i + 1) {
                levelRow[i].gameObject.SetActive(true);
                levelRow[i].SetGemsProgression("???");
                UIManager.Instance.SetEventSystemSelectedGO(levelRow[i].gameObject);
            } 
            else {
                levelRow[i].gameObject.SetActive(false);
            }
        }
    }

    public void CloseLevelScreen() {
        if (GameManager.Instance.CurrentState != GameState.InHUB) return;
        playerHandler.CurrentPlayer.SetInputSpace(cam.activeInHierarchy ? cam.transform : playerCam.transform);
        playerCam.SetActive(cam.activeInHierarchy);
        cam.SetActive(!cam.activeInHierarchy);
    }
}
