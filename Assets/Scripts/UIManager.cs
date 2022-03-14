using UnityEngine;
public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; private set; }
    public GameObject pauseMenu;
    public GameObject pauseHUD, inGameHUD, mainMenuHUD, optionHUD, creditsHUD;
    void Awake() {
        if(!Instance) Instance = this;
    }
    void Start() {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }
    void OnGameStateChanged(GameState newState) {
        pauseHUD.SetActive(newState == GameState.Pause);
    }
    public void QuitPause() {
        pauseMenu.SetActive(false);
        GameManager.Instance.SetState(GameState.InGame);
    }
}