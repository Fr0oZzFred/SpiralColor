using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceScene : MonoBehaviour {
    [SerializeField] Scenes scene;
    [SerializeField] GameState gameState;
    void Start() {
        this.gameObject.SetActive(false);
    }

    private void OnEnable() {
        LoadScene();
    }
    void LoadScene() {
        SceneManagement.Instance.LoadingRendering(scene.TargetScene, scene.AdditiveScene);
        GameManager.Instance.SetState(gameState);
        this.gameObject.SetActive(false);
    }
}
