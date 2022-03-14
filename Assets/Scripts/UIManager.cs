using UnityEngine;
using UnityEngine.EventSystems;
public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; private set; }
    [Header("UI Main Element")]
    [SerializeField]
    GameObject mainMenuHUD, inGameHUD , pauseHUD, scoreHUD, creditsHUD;
    [Header("Event System")]
    [SerializeField]
    EventSystem eventSystem;
    [SerializeField]
    GameObject mainMenuFirstGO, pauseFirstGO;
    void Awake() {
        if(!Instance) Instance = this;
    }
    void Start() {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }
    void OnGameStateChanged(GameState newState) {

        mainMenuHUD.SetActive(newState == GameState.MainMenu);
        inGameHUD.SetActive(newState == GameState.InGame);
        pauseHUD.SetActive(newState == GameState.Pause);
        scoreHUD.SetActive(newState == GameState.Score);
        creditsHUD.SetActive(newState == GameState.Credits);

        switch (newState) {
            case GameState.MainMenu:
                eventSystem.SetSelectedGameObject(mainMenuFirstGO);
                break;
            case GameState.InGame:
                //eventSystem.firstSelectedGameObject = inGameHUD;
                break;
            case GameState.Pause:
                eventSystem.SetSelectedGameObject(pauseFirstGO);
                break;
            case GameState.Score:
                //eventSystem.firstSelectedGameObject = scoreHUD;
                break;
            case GameState.Credits:
                //eventSystem.firstSelectedGameObject = creditsHUD;
                break;
            default:
                Debug.Log("No GO for " + newState);
                break;
        }
    }
}