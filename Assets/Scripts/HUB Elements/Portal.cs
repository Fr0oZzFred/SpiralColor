using UnityEngine;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public enum ScenesEnum {
    Tutorial = 0,

    Sphere = 1,
    Triangle = 2,
    Square = 4,
    Cross = 8,

    Sphere_Triangle = 3,
    Sphere_Square = 5,
    Sphere_Cross = 9,
    Triangle_Square = 6,
    Triangle_Cross = 10,
    Square_Cross = 12,

    Sphere_Triangle_Square = 7,
    Sphere_Triangle_Cross = 11,
    Sphere_Square_Cross = 13,
    Triangle_Square_Cross = 14,
    Sphere_Triangle_Square_Cross = 15
}
public class Portal : MonoBehaviour {
    #region Fields
    [Header("Text")]
    [SerializeField]
    TMP_Text textGO;

    [Header("Portal")]
    [SerializeField]
    List<Scenes> scenes;
    Dictionary<ScenesEnum, Scenes> scenesDico;

    ScenesEnum current;
    #endregion
    void ChangeTextContent(string text) {
        if (textGO) textGO.SetText(text);
    }


    private void Start() {
        scenesDico = new Dictionary<ScenesEnum, Scenes>();
        foreach (Scenes scene in scenes) {
            Scenes value;
            if (!scenesDico.TryGetValue(scene.destination, out value))
                scenesDico.Add(scene.destination, scene);
            else
                Debug.LogError("The" + scene.destination.ToString() + " destination already exists in the Dictonary.");
        }
    }


    public void ChangeCurrentDestination(int newCurrent) {
        current = (ScenesEnum)newCurrent;
        ChangeTextContent(current.ToString());
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Controller>() != null) {
            PlayTestData.Instance.Restart();
            SaveSystem.SaveGemmes(HUBManager.Instance.gemsPool);
            SceneManagement.Instance.LoadingRendering(scenesDico[current].TargetScene, scenesDico[current].AdditiveScene);
        }
    }
}
