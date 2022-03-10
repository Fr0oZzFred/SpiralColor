using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UIManager : MonoBehaviour{
    public static UIManager Instance { get; private set; }
    public GameObject pauseMenu;
    public GameObject pauseHUD, inGameHUD;

    void Awake() {
        Instance = this;
    }
    private void Start() {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }
    public void QuitPause() {
        pauseMenu.SetActive(false);
        GameManager.Instance.SetState(GameState.play);
    }
    
    void init() {
        pauseHUD.SetActive(false);
        inGameHUD.SetActive(false);
    }
    void OnGameStateChanged(GameState n) {
        init();
        switch (n) {
            case GameState.pause:
                pauseHUD.SetActive(true);
                break;

        }
    }
}
