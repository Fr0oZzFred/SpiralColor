using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public enum ScenesEnum {
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
public class Teleporter : MonoBehaviour {

    [Header("Text")]
    [SerializeField]
    TMP_Text textGO;
    [SerializeField]
    string text;

    [Header("Teleporter")]
    [SerializeField]
    Controller controller;
    [SerializeField]
    float rangeDetection;
    [SerializeField]
    List<Scenes> scenes;

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
        ScenesEnum current = (ScenesEnum)enumValue + 1;
        ChangeTextContent(current.ToString());
        Debug.Log(GameManager.Instance.Progression);
    }
    private void Update() {
        Vector3 p = transform.position - controller.transform.position;
        if(p.magnitude < rangeDetection) {
            CheckForLevelSelection();
        }
    }

    private void CheckForLevelSelection() {
        if (InputHandler.Controller == null) return;
        //ScenesEnum current =
        if (InputHandler.Controller.buttonEast.wasPressedThisFrame) {
            sphere = !sphere;
        }
        if (InputHandler.Controller.buttonNorth.wasPressedThisFrame) {
            triangle = !triangle;
        }
        if (InputHandler.Controller.buttonWest.wasPressedThisFrame) {
            square = !square;
        }
        if (InputHandler.Controller.buttonSouth.wasPressedThisFrame) {
            cross = !cross;
        }
    }

    /*private void OnTriggerEnter(Collider other) {
   if (other.GetComponent<Controller>() != null) {
       PlayTestData.Instance.Restart();
       if (AdditiveScene != null) SceneManagement.Instance.LoadingRendering(TargetScene, AdditiveScene);
       else SceneManagement.Instance.LoadLevel(TargetScene);
   }
}*/
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangeDetection);
    }
}
