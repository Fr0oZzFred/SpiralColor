using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
public class UIManager : MonoBehaviour {
    #region Fields
    [Header("MainMenu")]
    [SerializeField]
    GameObject mainMenuHUD;

    [Header("InHUB")]
    [SerializeField]
    GameObject inHubHUD;

    [Header("InLevel")]
    [SerializeField]
    GameObject inLevelHUD;

    [Header("Pause")]
    [SerializeField]
    GameObject pauseHUD;

    [Header("Score")]
    [SerializeField]
    GameObject scoreHUD;
    [SerializeField]
    List<GameObject> stars;

    [Header("Loading")]
    [SerializeField]
    GameObject loadingHUD;
    [SerializeField]
    Slider loadingSlider;
    [SerializeField]
    TMP_Text loadingProgressText;

    [Header("Cutscene")]
    [SerializeField]
    GameObject cutsceneHUD;

    [Header("Credits")]
    [SerializeField]
    GameObject creditsHUD;

    [Header("Options")]
    [SerializeField]
    GameObject optionsHUD;

    [Header("ControllerDisconnected")]
    [SerializeField]
    GameObject controllerDisconnectedHUD;
    [SerializeField]
    TMP_Text errorText;


    [Header("Event System")]
    [SerializeField]
    EventSystem eventSystem;
    [SerializeField]
    GameObject mainMenuFirstSelectedGO, pauseFirstSelectedGO, scoreFirstSelectedGO, optionFirstSelectedGO;
    public static UIManager Instance { get; private set; }

    #endregion
    void Awake() {
        if(!Instance) Instance = this;
    }
    void Start() {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }
    void OnGameStateChanged(GameState newState) {

        mainMenuHUD.SetActive(newState == GameState.MainMenu);
        inHubHUD.SetActive(newState == GameState.InHUB);
        inLevelHUD.SetActive(newState == GameState.InLevel);
        pauseHUD.SetActive(newState == GameState.Pause);
        scoreHUD.SetActive(newState == GameState.Score);
        loadingHUD.SetActive(newState == GameState.Loading);
        cutsceneHUD.SetActive(newState == GameState.Cutscene);
        creditsHUD.SetActive(newState == GameState.Credits);
        optionsHUD.SetActive(newState == GameState.Options);
        controllerDisconnectedHUD.SetActive(newState == GameState.ControllerDisconnected);

        eventSystem.SetSelectedGameObject(null);
        switch (newState) {
            case GameState.MainMenu:
                eventSystem.SetSelectedGameObject(mainMenuFirstSelectedGO);
                break;
            case GameState.Pause:
                eventSystem.SetSelectedGameObject(pauseFirstSelectedGO);
                break;
            case GameState.Score:
                DisplayScore();
                eventSystem.SetSelectedGameObject(scoreFirstSelectedGO);
                break;
            case GameState.Options:
                eventSystem.SetSelectedGameObject(optionFirstSelectedGO);
                break;
            case GameState.ControllerDisconnected:
                errorText.SetText(InputHandler.Instance.ErrorMessage);
                break;
            default:
                break;
        }
    }
    public void UpdateLoadingScreen(float sliderValue, float progressText) {
        SetLoadingSlider(sliderValue);
        SetLoadingText(progressText);
    }

    public void DisplayScore() {
        ResetScore();
        for (int i = 1; i < stars.Count + 1; i++) {
            stars[i - 1].SetActive(GameManager.Instance.pieces["Star " + LevelManager.Instance.LevelInt + "-" + i]);
        }
    }
    private void ResetScore() {
        for (int i = 0; i < stars.Count; i++) {
            stars[i].SetActive(false);
        }
    }
    void SetLoadingSlider(float sliderValue) {
        loadingSlider.value = sliderValue;
    }

    void SetLoadingText(float progressText) {
        loadingProgressText.text = progressText + "%";
    }
}