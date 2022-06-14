using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[System.Serializable]
public class GemsData {
    public int count;
    public float[] positions;
    public float[] rotations;
    public GemsData(GemsPool data) {
        count = data.Gems.Count;
        positions = new float[3 * count];
        rotations = new float[4 * count];
        if (count == 0) return;
        for (int i = 0; i < count; i++) {
            positions[3 * i] = data.Gems[i].transform.position.x;
            positions[3 * i + 1] = data.Gems[i].transform.position.y;
            positions[3 * i + 2] = data.Gems[i].transform.position.z;

            rotations[4 * i] = data.Gems[i].transform.rotation.x;
            rotations[4 * i + 1] = data.Gems[i].transform.rotation.y;
            rotations[4 * i + 2] = data.Gems[i].transform.rotation.z;
            rotations[4 * i + 3] = data.Gems[i].transform.rotation.w;
        }
    }
}
public class GemsPool : MonoBehaviour
{
    [SerializeField]
    List<GameObject> prefab;
    public List<GameObject> Gems;
    public int GemsCount { get { return Gems.Count; } }
    Vector3 pos {
        get {
            //Ajouter calcul pour avoir une zone de spawn et pas un seul point
            return transform.position;
        }
    }
    void Awake() {
        Gems = new List<GameObject>();
        if (System.IO.File.Exists(Application.persistentDataPath + "/GemsPool.data")) {
            LoadGemsPool();
        }
    }
    
    private void Update() {
        if (Keyboard.current.aKey.wasPressedThisFrame) {
            //Gems.Add(Instantiate(prefab, pos, Quaternion.identity));
        }
        if (Keyboard.current.nKey.wasPressedThisFrame) {
            SaveSystem.SaveGemmes(this);
        }
        if (Keyboard.current.bKey.wasPressedThisFrame) {
            LoadGemsPool();
        }
        if (Keyboard.current.dKey.wasPressedThisFrame) {
            DestroyGems();
        }
    }
    public void Spawn(int type) {
        Gems.Add(Instantiate(prefab[type], pos, Quaternion.identity));
    }
    void LoadGemsPool() {
        GemsData data = SaveSystem.LoadGemmes();
        if (data.count == 0) return;
        for (int i = 0; i < data.count; i++) {
            Vector3 position = new Vector3(data.positions[3 * i], data.positions[3 * i + 1], data.positions[3 * i + 2]);
            Quaternion rotation = new Quaternion(data.rotations[i * 4], data.rotations[i * 4 + 1], data.rotations[i * 4 + 2], data.rotations[i * 4 + 3]);
            Gems.Add(Instantiate(prefab[GameManager.Instance.GemsTypesIndex[i]], position, rotation));
            Gems[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }
    void DestroyGems() {
        if (Gems.Count == 0) return;
        for (int i = 0; i < Gems.Count; i++) Destroy(Gems[i]);
        Gems = new List<GameObject>();
    }
}
