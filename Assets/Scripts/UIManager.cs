using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; private set; }
    [Header("UI Main Element")]
    [SerializeField]
    GameObject loadingHUD, mainMenuHUD, inHubHUD, inLevelHUD , pauseHUD, scoreHUD, creditsHUD, UITest, endButton, star1, star2, star3;
    [Header("Loading")]
    [SerializeField]
    Slider loadingSlider;
    [SerializeField]
    TMP_Text loadingProgressText;
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

        loadingHUD.SetActive(newState == GameState.Loading);
        mainMenuHUD.SetActive(newState == GameState.MainMenu);
        inHubHUD.SetActive(newState == GameState.InHUB);
        inLevelHUD.SetActive(newState == GameState.InLevel);
        pauseHUD.SetActive(newState == GameState.Pause);
        scoreHUD.SetActive(newState == GameState.Score);
        creditsHUD.SetActive(newState == GameState.Credits);

        switch (newState) {
            case GameState.MainMenu:
                eventSystem.SetSelectedGameObject(mainMenuFirstGO);
                break;
            case GameState.Pause:
                eventSystem.SetSelectedGameObject(pauseFirstGO);
                break;
            default:
                Debug.LogWarning("No GO in the switch for State : " + newState);
                break;
        }
    }
    public void UpdateLoadingScreen(float sliderValue, float progressText) {
        SetLoadingSlider(sliderValue);
        SetLoadingText(progressText);
    }
    public void Score() {
        endButton.SetActive(false);
        UITest.SetActive(true);
        star1.SetActive(GameManager.Instance.pieces["Star " + LevelManager.Instance.Level + "-" + 1]);
        star1.SetActive(GameManager.Instance.pieces["Star " + LevelManager.Instance.Level + "-" + 2]);
        star1.SetActive(GameManager.Instance.pieces["Star " + LevelManager.Instance.Level + "-" + 3]);
    }
    public void ToHub() {
        UITest.SetActive(false);
        SceneManagement.Instance.LoadLevel(0);
    }
    void SetLoadingSlider(float sliderValue) {
        loadingSlider.value = sliderValue;
    }

    void SetLoadingText(float progressText) {
        loadingProgressText.text = progressText + "%";
    }
}