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
        count = data.gems.Count;
        positions = new float[3 * count];
        rotations = new float[4 * count];
        if (count == 0) return;
        for (int i = 0; i < count; i++) {
            positions[3 * i] = data.gems[i].transform.position.x;
            positions[3 * i + 1] = data.gems[i].transform.position.y;
            positions[3 * i + 2] = data.gems[i].transform.position.z;
        }
        for (int i = 0; i < count; i++) {
            rotations[4 * i] = data.gems[i].transform.rotation.x;
            rotations[4 * i + 1] = data.gems[i].transform.rotation.y;
            rotations[4 * i + 2] = data.gems[i].transform.rotation.z;
            rotations[4 * i + 3] = data.gems[i].transform.rotation.w;
        }
    }
}
public class GemsPool : MonoBehaviour
{
    [SerializeField]
    GameObject prefab;
    public List<GameObject> gems = new List<GameObject>();
    public int GemsCount {
        get {
            return gems.Count;
        }
    }
    void Awake() {
        if (System.IO.File.Exists(Application.persistentDataPath + "/GemsPool.data")) {
            LoadGemsPool();
        }
    }
    
    private void Update() {
        if (Keyboard.current.aKey.wasPressedThisFrame) {
            gems.Add(Instantiate(prefab, pos, Quaternion.identity));
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
    Vector3 pos {
        get {
            return transform.position;
        }
    }
    public void Spawn() {
        gems.Add(Instantiate(prefab, pos, Quaternion.identity));
    }
    void LoadGemsPool() {
        GemsData data = SaveSystem.LoadGemmes();
        if (data.count == 0) return;
        for(int i = 0; i < data.count; i++) {
            Vector3 position = new Vector3(data.positions[3 * i], data.positions[3 * i + 1], data.positions[3 * i + 2]);
            Quaternion rotation = new Quaternion(data.rotations[i * 4], data.rotations[i * 4 + 1], data.rotations[i * 4 + 2], data.rotations[i * 4 + 3]);
            gems.Add(Instantiate(prefab, position, rotation));
            gems[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }
    void DestroyGems() {
        if (gems.Count == 0) return;
        for (int i = 0; i < gems.Count; i++) Destroy(gems[i]);
        gems = new List<GameObject>();
    }
}
