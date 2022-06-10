using UnityEngine;
using UnityEngine.VFX;
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
    [Header("CurrentLevel")]
    [SerializeField] LevelRow currentLevel;
    [SerializeField] List<ScriptableLevelRow> scriptables;


    [Header("Portal")]
    [SerializeField] VisualEffect portalVFX;
    [SerializeField] List<VFXParam> vfxParams;
    //Note ne pas faire de list dans une struct, l'affichage est buggé
    [System.Serializable] struct VFXParam {
        [SerializeField] [ColorUsage(true, true)] public Color edgeColor1;
        [SerializeField] [ColorUsage(true, true)] public Color edgeColor2;
        [SerializeField] [ColorUsage(true, true)] public Color edgeColor3;
        [SerializeField] [ColorUsage(true, true)] public Color edgeColor4;
        [SerializeField] [ColorUsage(true, true)] public Color portalColor1;
        [SerializeField] [ColorUsage(true, true)] public Color portalColor2;
        [SerializeField] [ColorUsage(true, true)] public Color portalColor3;
        [SerializeField] [ColorUsage(true, true)] public Color portalColor4;
        public float dissolve;
    }

    [Header("Destination")]
    [SerializeField]
    List<Scenes> scenes;
    Dictionary<ScenesEnum, Scenes> scenesDico;

    ScenesEnum current;
    #endregion

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
        int index = int.Parse(scenesDico[current].TargetScene.Remove(0, scenesDico[current].TargetScene.Length - 2));
        if (index < GameManager.Instance.Progression || GameManager.Instance.GameDone) {
            GameManager.Instance.GetCollectedGemsOfLevel(index, out int collected, out int max);
            currentLevel.SetGemsProgression(collected + " / " + max);
        } else if (GameManager.Instance.Progression == index) {
            currentLevel.SetGemsProgression("???");
        }
        currentLevel.SetLevel(scriptables[index-1]);
        ChangePortalColor(index - 1);
    }
    private void ChangePortalColor(int index) {
        portalVFX.gameObject.SetActive(false);
        portalVFX.SetVector4("Portal Edge Color 1", vfxParams[index].edgeColor1);
        portalVFX.SetVector4("Portal Edge Color 2", vfxParams[index].edgeColor2);
        portalVFX.SetVector4("Portal Edge Color 3", vfxParams[index].edgeColor3);
        portalVFX.SetVector4("Portal Edge Color 4", vfxParams[index].edgeColor4);
        portalVFX.SetVector4("Portal Color 1", vfxParams[index].portalColor1);
        portalVFX.SetVector4("Portal Color 2", vfxParams[index].portalColor2);
        portalVFX.SetVector4("Portal Color 3", vfxParams[index].portalColor3);
        portalVFX.SetVector4("Portal Color 4", vfxParams[index].portalColor4);
        portalVFX.SetFloat("Twirl Dissolve", vfxParams[index].dissolve);
        portalVFX.gameObject.SetActive(true);
    }
    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Controller>() != null) {
            PlayTestData.Instance.Restart();
            SaveSystem.SaveGemmes(HUBManager.Instance.gemsPool);
            SceneManagement.Instance.LoadingRendering(scenesDico[current].TargetScene, scenesDico[current].AdditiveScene);
        }
    }
}
