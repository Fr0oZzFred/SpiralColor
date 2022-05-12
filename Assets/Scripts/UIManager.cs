using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using System.Collections.Generic;
using System.Collections;
[System.Serializable]
public class UIManagerData {
    public bool XCam, YCam;
    public float Sensitivity;
    public int IndexLanguage;
    public UIManagerData(UIManager data) {
        XCam = data.XCam;
        YCam = data.YCam;
        Sensitivity = data.Sensitivity;
        IndexLanguage = data.IndexLanguage;
    }
}
    public class UIManager : MonoBehaviour {
    #region Fields
    [Header("MainMenu")]
    [SerializeField]
    GameObject mainMenuHUD;

    [Header("InHUB")]
    [SerializeField]
    GameObject inHubHUD;

    [Header("InLevel")]
    [SerializeField] GameObject inLevelHUD;
    [SerializeField] TMP_Text crystalText;

    [Header("Pause")]
    [SerializeField] GameObject pauseHUD;
    [SerializeField] GameObject restartButton;
    [SerializeField] GameObject backToHUB_Button;

    [Header("Score")]
    [SerializeField] GameObject scoreHUD;
    [SerializeField] TMP_Text scoreText;

    [Header("Loading")]
    [SerializeField] GameObject loadingHUD;
    [SerializeField] Slider loadingSlider;
    [SerializeField] TMP_Text loadingProgressText;

    [Header("Cutscene")]
    [SerializeField] GameObject cutsceneHUD;

    [Header("Credits")]
    [SerializeField] GameObject creditsHUD;

    [Header("Options")]
    [SerializeField] GameObject optionsHUD;
    [SerializeField] TMP_Dropdown languageDropdown;
    [SerializeField] Toggle x, y;
    [SerializeField] Slider sensibility;
    [SerializeField] TMP_Text sensibilityText;
    public bool XCam { get { return x.isOn; } }
    public bool YCam { get { return y.isOn; } }
    public float Sensitivity { get { return sensibility.value; } }
    public int IndexLanguage { get { return languageDropdown.value; } }


    [Header("ControllerDisconnected")]
    [SerializeField] GameObject controllerDisconnectedHUD;
    [SerializeField] TMP_Text errorText;

    [Header("Event System")]
    [SerializeField] EventSystem eventSystem;
    [SerializeField] GameObject mainMenuFirstSelectedGO, pauseFirstSelectedGO, scoreFirstSelectedGO, optionFirstSelectedGO, keyboardFirstSelectionGO;

    [Header("Keyboard")]
    [SerializeField] GameObject keyboard;
    [SerializeField] TMP_Text keyboardText;
    public static UIManager Instance { get; private set; }

    #endregion
    void Awake() {
        if(!Instance) Instance = this;
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }
    IEnumerator Start() {
        if (System.IO.File.Exists(Application.persistentDataPath + "/UIManager.data")) {
            LoadData();
        }
        yield return LocalizationSettings.InitializationOperation;
        List<TMP_Dropdown.OptionData> languages = new List<TMP_Dropdown.OptionData>();
        int index = 0;
        for(int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++) {
            var local = LocalizationSettings.AvailableLocales.Locales[i];
            if (LocalizationSettings.SelectedLocale == local) index = i;
            languages.Add(new TMP_Dropdown.OptionData(local.name));
        }
        languageDropdown.options = languages;
        languageDropdown.value = index;
        languageDropdown.onValueChanged.AddListener(LocaleSelected);
        languageDropdown.onValueChanged.Invoke(IndexLanguage);
    }
    void OnGameStateChanged(GameState newState) {
        if (GameManager.Instance.OldState == GameState.Options) SaveData();
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
        keyboard.SetActive(newState == GameState.Keyboard);

        eventSystem.SetSelectedGameObject(null);
        switch (newState) {
            case GameState.MainMenu:
                eventSystem.SetSelectedGameObject(mainMenuFirstSelectedGO);
                break;
            case GameState.Pause:
                restartButton.SetActive(GameManager.Instance.OldState == GameState.InLevel);
                backToHUB_Button.SetActive(GameManager.Instance.OldState == GameState.InLevel);
                eventSystem.SetSelectedGameObject(pauseFirstSelectedGO);
                break;
            case GameState.Score:
                DisplayScore(); 
                eventSystem.SetSelectedGameObject(scoreFirstSelectedGO);
                break;
            case GameState.Options:
                UpdateSensibilityText();
                eventSystem.SetSelectedGameObject(optionFirstSelectedGO);
                break;
            case GameState.ControllerDisconnected:
                errorText.SetText(InputHandler.Instance.ErrorMessage);
                break;
            case GameState.Keyboard:
                eventSystem.SetSelectedGameObject(keyboardFirstSelectionGO);
                break;
            default:
                break;
        }
    }
    public void SetEventSystemCurrentSelectedGO(GameObject g) {
        eventSystem.SetSelectedGameObject(g);
    }
    #region Data
    public void SaveData() {
        SaveSystem.SaveUIManager(this);
    }
    public void LoadData() {
        UIManagerData data = SaveSystem.LoadUIManager();
        x.isOn = data.XCam;
        y.isOn = data.YCam;
        sensibility.value = data.Sensitivity;
        languageDropdown.value = data.IndexLanguage;
        Debug.Log(languageDropdown.value);
    }
    #endregion

    #region options
    static void LocaleSelected(int index) {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
    #endregion

    #region InLevel
    public void DisplayGems() {
        GameManager.Instance.GetCollectedGemsOfLevel(LevelManager.Instance.LevelInt, out int collected, out int max);
        crystalText.SetText(collected + " / " + max);
    }
    #endregion

    #region InPause
    public void RestartLevel() {
        LevelManager.Instance.ReloadLevel();
    }
    #endregion

    #region LoadingScreen
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
    #endregion

    #region Score
    public void DisplayScore() {
        GameManager.Instance.GetCollectedGemsOfLevel(LevelManager.Instance.LevelInt, out int collected, out int max);
        scoreText.SetText(collected + " / " + max);
    }
    #endregion

    #region Options

    public void UpdateSensibilityText() {
        sensibilityText.SetText(sensibility.value.ToString());
    }

    #endregion

    #region keyboard
    public void OpenKeyboard() {
        keyboard.SetActive(true);
        keyboardText.text = "";
        GameManager.Instance.SetState(GameState.Keyboard);
    }
    public void InputKeyboard(string letter) {
        keyboardText.text += letter;
    }
    public void DeleteLetter() {
        if (keyboardText.text.Length > 0) {
            keyboardText.text = keyboardText.text.Remove(keyboardText.text.Length - 1);
            Debug.Log(keyboardText.text);
        }
    }
    public void Enter() {
        GameManager.Instance.ApplyUsername(keyboardText.text);
        keyboard.SetActive(false);
    }
    #endregion
}