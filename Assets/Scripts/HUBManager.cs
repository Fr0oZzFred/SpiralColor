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

    [SerializeField] GameObject cam;

    private void Awake() {
        if (!Instance) Instance = this;
        GameManager.Instance.SetState(GameState.InHUB);
        SoundsManager.Instance.StopCurrentMusic();
        SoundsManager.Instance.Play(musicName);
    }

    private IEnumerator Start() {
        InitLevelScreen();
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
        if (Keyboard.current.tKey.wasPressedThisFrame) {
            playerHandler.CurrentPlayer.RegisterInputs(false);
        }
        if (InputHandler.Controller == null) return;
        if (InputHandler.Controller.rightStickButton.wasPressedThisFrame) {
            playerHandler.CurrentPlayer.RegisterInputs(cam.activeInHierarchy);
            cam.SetActive(!cam.activeInHierarchy);
        }
    }
    void InitLevelScreen() {

    }
}
