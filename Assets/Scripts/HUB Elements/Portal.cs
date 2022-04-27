using UnityEngine;
using TMPro;
using System.Collections.Generic;

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

    [Header("Text")]
    [SerializeField]
    TMP_Text textGO;

    [Header("Teleporter")]
    [SerializeField]
    Controller controller;
    [SerializeField]
    float rangeDetection;
    [SerializeField]
    List<Scenes> scenes;
    Dictionary<ScenesEnum, Scenes> scenesDico;

    bool sphere, triangle, square, cross;

    int enumValue {
        get {
            int total = 0;
            total += sphere ? 1 : 0;
            total += triangle ? 2 : 0;
            total += square ? 4 : 0;
            total += cross ? 8 : 0;
            return total;
        }
    }
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

        CheckForLevelSelection();
    }
    private void Update() {
        Vector3 p = transform.position - controller.transform.position;
        if (p.magnitude < rangeDetection) {
            CheckForLevelSelection();
        }
    }
    /// <summary>
    /// Checkez la progression pour savoir s'il peut poser une orbe et le bloque en cas de tutorial
    /// </summary>
    private void CheckForLevelSelection() {
        if (InputHandler.Controller == null) return;

        switch (GameManager.Instance.Progression) {
            case 0:
                sphere = triangle = square = cross = false;
                ScenesEnum tutorialLevel = (ScenesEnum)enumValue;
                ChangeTextContent(tutorialLevel.ToString());
                return;
            case 1:
                sphere = true;
                triangle = square = cross = false;
                ScenesEnum sphereLevel = (ScenesEnum)enumValue;
                ChangeTextContent(sphereLevel.ToString());
                return;
            case 2:
                triangle = true;
                sphere = square = cross = false;
                ScenesEnum triangleLevel = (ScenesEnum)enumValue;
                ChangeTextContent(triangleLevel.ToString());
                return;
            case 4:
                square = true;
                sphere = triangle = cross = false;
                ScenesEnum squareLevel = (ScenesEnum)enumValue;
                ChangeTextContent(squareLevel.ToString());
                return;
            case 7:
                cross = true;
                sphere = triangle = square = false;
                ScenesEnum crossLevel = (ScenesEnum)enumValue;
                ChangeTextContent(crossLevel.ToString());
                return;
        }

        if (GameManager.Instance.Progression >= 1 &&
            InputHandler.Controller.buttonEast.wasPressedThisFrame) {
            sphere = !sphere;
        }
        else if (GameManager.Instance.Progression >= 2 && 
            InputHandler.Controller.buttonNorth.wasPressedThisFrame) {
            triangle = !triangle;
        }
        else if (GameManager.Instance.Progression >= 4 && 
            InputHandler.Controller.buttonWest.wasPressedThisFrame) {
            square = !square;
        }
        else if (GameManager.Instance.Progression >= 7 && 
            InputHandler.Controller.buttonSouth.wasPressedThisFrame) {
            cross = !cross;
        }
        ScenesEnum current = (ScenesEnum)enumValue;
        ChangeTextContent(current.ToString());
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Controller>() != null) {
            PlayTestData.Instance.Restart();
            SaveSystem.SaveGemmes(HUBManager.Instance.gemsPool);
            ScenesEnum current = (ScenesEnum)enumValue;
            SceneManagement.Instance.LoadingRendering(scenesDico[current].TargetScene, scenesDico[current].AdditiveScene);
        }
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangeDetection);
    }
}
