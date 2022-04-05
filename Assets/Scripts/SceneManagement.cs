using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class SceneManagement : MonoBehaviour {
    public static SceneManagement Instance { get; private set; }
    //Fonction pour Load 2 scène en même temps

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
    public void LoadingRendering(string sceneLevel, string sceneRendering) {
        if (SceneManager.GetSceneByName(sceneLevel).isLoaded) {
            Debug.LogWarning("Scene demandée déjà chargée");
            return;
        }
        LoadLevel(sceneLevel);
        StartCoroutine(LoadingRendering(sceneRendering));
    }
    IEnumerator LoadingRendering(string sceneName) {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }
    IEnumerator LoadAsynchronously(int sceneIndex) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        GameManager.Instance.SetState(GameState.Loading);
        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            UIManager.Instance.UpdateLoadingScreen(progress, progress * 100);
            yield return null;
        }
    }

}