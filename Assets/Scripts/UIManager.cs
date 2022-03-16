using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; private set; }
    [Header("UI Main Element")]
    [SerializeField]
    GameObject loadingHUD, mainMenuHUD, inHubHUD, inLevelHUD , pauseHUD, scoreHUD, creditsHUD;
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
    void SetLoadingSlider(float sliderValue) {
        loadingSlider.value = sliderValue;
    }

    void SetLoadingText(float progressText) {
        loadingProgressText.text = progressText + "%";
    }
}