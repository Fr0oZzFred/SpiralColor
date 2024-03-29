﻿using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using System.Collections.Generic;
using System.Collections;
using System;

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
    [SerializeField] GameObject blackBoards;
    [SerializeField] GameObject levelSelectionBlackBoards;

    [Header("InLevel")]
    [SerializeField] GameObject inLevelHUD;
    [SerializeField] TMP_Text crystalText;
    [SerializeField] ATHPlayer athPlayer;
    [SerializeField] List<ScriptableATH> aths; //As Usual 0 Sphere, 1 Triangle, 2 Square, 3 Cross;

    #region ATH
    [Serializable]
    struct ATHPlayer {
        public GameObject ath;
        public Image circle;
        public Image shape;
        public Image barTop;
        public Image barMiddle;
        public Image barBottom;
        public Image actionButtonMid;
        public TextMeshProUGUI actionTextMid;
        public GameObject triangleJumps;
        public Image tornadoTimer;
        public TextMeshProUGUI tornadoText;
    }

    #endregion
    [Header("Pause")]
    [SerializeField] GameObject pauseHUD;
    [SerializeField] GameObject restartButton;
    [SerializeField] GameObject backToHUB_Button;

    [Header("Score")]
    [SerializeField] GameObject scoreHUD;

    [Serializable]
    struct OrbFragment {
        [SerializeField] public Color flareColor;
        public Sprite fragmentSprite;
    }
    [SerializeField] List<OrbFragment> orbsFragments;
    [SerializeField] float flareSpeed;
    [SerializeField] RectTransform flareTransform;
    [SerializeField] Image orbsImage;
    [SerializeField] TMP_Text scoreText;


    [Header("Loading")]
    [SerializeField] GameObject loadingHUD;
    //[SerializeField] Slider loadingSlider;
    //[SerializeField] TMP_Text loadingProgressText;

    [Header("Cutscene")]
    [SerializeField] GameObject cutsceneHUD;

    [Header("Credits")]
    [SerializeField] GameObject creditsHUD;
    [SerializeField] List<RectTransform> flares;

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
    [SerializeField] GameObject errorText1;
    [SerializeField] GameObject errorText2;

    [Header("Event System")]
    [SerializeField] EventSystem eventSystem;
    [SerializeField] GameObject mainMenuFirstSelectedGO, pauseFirstSelectedGO, scoreFirstSelectedGO, optionFirstSelectedGO, keyboardFirstSelectionGO;

    [Header("Keyboard")]
    [SerializeField] GameObject keyboard;
    [SerializeField] TMP_Text keyboardText;
    Canvas canvas;
    public static UIManager Instance { get; private set; }
#endregion
    void Awake() {
        if(!Instance) Instance = this;
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        canvas = GetComponent<Canvas>();
    }
    IEnumerator Start() {
        int tmpIndexLanguage = -1;
        if (System.IO.File.Exists(Application.persistentDataPath + "/UIManager.data")) {
            LoadData();
            tmpIndexLanguage = IndexLanguage;
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
        languageDropdown.value = tmpIndexLanguage > -1 ? tmpIndexLanguage : index;
        languageDropdown.onValueChanged.AddListener(LocaleSelected);
        languageDropdown.onValueChanged.Invoke(languageDropdown.value);
    }

    private void Update() {
        if (UnityEngine.InputSystem.Keyboard.current.hKey.wasPressedThisFrame) {
            Debug.Log(Camera.main);
            canvas.worldCamera = Camera.main;
        }
        if (flareTransform.gameObject.activeInHierarchy) {
            Vector3 rota = flareTransform.rotation.eulerAngles;
            rota.z += flareSpeed * Time.deltaTime;
            Quaternion rot = new Quaternion {
                eulerAngles = rota
            };
            flareTransform.rotation = rot;
        }
        else if(GameManager.Instance.CurrentState == GameState.Credits) {
            foreach (var item in flares) {
                Vector3 rota = item.rotation.eulerAngles;
                rota.z += flareSpeed * Time.deltaTime;
                Quaternion rot = new Quaternion {
                    eulerAngles = rota
                };
                item.rotation = rot;
            }
        }
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
                StartCoroutine(WaitForEndOfTimeline(5f, mainMenuFirstSelectedGO));
                break;
            case GameState.Pause:
                restartButton.SetActive(GameManager.Instance.OldState == GameState.InLevel);
                StartCoroutine(WaitForEndOfTimeline(1.5f, pauseFirstSelectedGO));
                break;
            case GameState.InHUB:
                athPlayer.ath.transform.SetParent(inHubHUD.transform);
                break;
            case GameState.InLevel:
                athPlayer.ath.transform.SetParent(inLevelHUD.transform);
                break;
            case GameState.Score:
                DisplayScore();
                StartCoroutine(WaitForEndOfTimeline(4f, scoreFirstSelectedGO));
                break;
            case GameState.Options:
                UpdateSensibilityText();
                StartCoroutine(WaitForEndOfTimeline(1.5f, optionFirstSelectedGO));
                break;
            case GameState.ControllerDisconnected:
                errorText1.SetActive(InputHandler.Instance.ErrorMessage);
                errorText2.SetActive(!InputHandler.Instance.ErrorMessage);
                break;
            case GameState.Keyboard:
                eventSystem.SetSelectedGameObject(keyboardFirstSelectionGO);
                break;
            default:
                break;
        }
    }
    IEnumerator WaitForEndOfTimeline(float time, GameObject gameObjectToSetSelected) {
        yield return new WaitForSeconds(time);
        eventSystem.SetSelectedGameObject(gameObjectToSetSelected);
    }
    public void UpdateCamera() {
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = 1;
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
    }
    #endregion

    #region options
    static void LocaleSelected(int index) {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
    #endregion
    #region InHUB
    public void ActivateBlackBoard(bool b) {
        blackBoards.SetActive(b);
    }
    public void ActivateLevelScreenBlackBoards(bool b) {
        levelSelectionBlackBoards.SetActive(b);
    }
    #endregion
    #region InLevel
    //bool utilisé pour éviter les animations lors des init
    public void DisplayGems(bool b) {
        if (b) {
            StartCoroutine(AnimGemsScore());
        } else {
            GameManager.Instance.GetCollectedGemsOfLevel(LevelManager.Instance.LevelInt, out int collected, out int max);
            crystalText.SetText(collected + " / " + max);
        }
    }

    IEnumerator AnimGemsScore() {
        crystalText.transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        GameManager.Instance.GetCollectedGemsOfLevel(LevelManager.Instance.LevelInt, out int collected, out int max);
        crystalText.SetText(collected + " / " + max);
    }

    public void UpdateATH(int index) {
        if (index > aths.Count) return;

        if(index < 0) {
            athPlayer.ath.SetActive(false);
            return;
        }


        athPlayer.circle.sprite = aths[index].circle;
        athPlayer.shape.sprite = aths[index].shape;
        athPlayer.barTop.sprite = aths[index].top;
        athPlayer.tornadoText.SetText(LocalizationSettings.Instance.GetSelectedLocale().name == "English (en)" ? aths[index].actionTextTopEN : aths[index].actionTextTopFR);
        athPlayer.barMiddle.sprite = aths[index].middle;
        athPlayer.actionButtonMid.sprite = aths[index].actionButtonMid;
        athPlayer.actionTextMid.SetText(LocalizationSettings.Instance.GetSelectedLocale().name == "English (en)" ? aths[index].actionTextMidEN : aths[index].actionTextMidFR);
        athPlayer.barBottom.sprite = aths[index].bottom;

        athPlayer.barTop.gameObject.SetActive(aths[index].top);
        athPlayer.barMiddle.gameObject.SetActive(aths[index].middle);
        athPlayer.triangleJumps.SetActive(index == 1);



        athPlayer.ath.SetActive(true);
    }

    public void UpdateTornadoTimer(float coef) {
        coef = Mathf.Clamp(coef, 0, 1);
        athPlayer.tornadoTimer.fillAmount = coef;
    }

    public void UpdateTriangleJumps(int jump) {
        for (int i = 0; i < athPlayer.triangleJumps.transform.childCount-4; i++) {
            athPlayer.triangleJumps.transform.GetChild(i+4).gameObject.SetActive(i >= jump);
        }
    }

    public void UpdateMidBar(int index, bool pressed) {
        switch (index) {
            case 0:
                athPlayer.barMiddle.color = pressed ? Color.red : Color.white;
                break;
            case 1:
                athPlayer.barMiddle.color = pressed ? Color.green : Color.white;
                athPlayer.barTop.color = pressed ? Color.green : Color.white;
                break;
            case 3:
                athPlayer.barMiddle.color = pressed ? Color.cyan : Color.white;
                break;
        }
        
    }
    #endregion

    #region InPause
    public void RestartLevel() {
        LevelManager.Instance.ReloadLevel();
    }
    #endregion

    #region LoadingScreen
   /*public void UpdateLoadingScreen(float sliderValue, float progressText) {
        SetLoadingSlider(sliderValue);
        SetLoadingText(progressText);
    }

    void SetLoadingSlider(float sliderValue) {
        loadingSlider.value = sliderValue;
    }

    void SetLoadingText(float progressText) {
        loadingProgressText.text = progressText + "%";
    }*/
    #endregion

    #region Score
    public void DisplayScore() {
        orbsImage.sprite = orbsFragments[LevelManager.Instance.LevelInt - 1].fragmentSprite;
        flareTransform.GetComponent<Image>().color = orbsFragments[LevelManager.Instance.LevelInt - 1].flareColor;
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