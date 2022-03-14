using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class SceneManagement : MonoBehaviour {
    public static SceneManagement Instance { get; private set; }
    public GameObject loadingScreen;
    public Slider slider;
    public TMP_Text progressText;


    private void Awake() {
        if (!Instance) Instance = this;
    }

    public void LoadLevel(int sceneIndex) {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    public void LoadLevel(string sceneName) {
        if (SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/" + sceneName + ".unity") == -1) Debug.LogError($"The Scene {sceneName} has not been found");
        LoadLevel(SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/" + sceneName + ".unity"));
    }

    IEnumerator LoadAsynchronously(int sceneIndex) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);
        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            slider.value = progress;
            progressText.text = progress * 100 + "%";
            yield return null;
        }
        loadingScreen.SetActive(false);
    }
}