using System.Collections.Generic;
using UnityEngine;

public class GemsPool : MonoBehaviour
{
    [SerializeField]
    GameObject prefab;

    List<GameObject> pool;
    int activeIndex = 0;

    Vector3 pos {
        get {
            return this.transform.position;
        }
    }
    public void InitPool(int number) {
        pool = new List<GameObject>(number);
        for (int i = 0; i < number; i++) {
            GetPrefab();
            pool[i].SetActive(false);
        }
    }
    public void InstantiatePrefab() {
        GameObject current = GetPrefab();
        current.transform.position = pos;
    }
    GameObject GetPrefab() {
        if (activeIndex == pool.Count) {
            pool.Add(Instantiate(prefab, Vector3.zero, Quaternion.identity));
            return pool[activeIndex++];
        }
        pool[activeIndex].SetActive(true);
        return pool[activeIndex++];
    }

    public void Destroy(int index = 0) {
        if (!pool[0].activeSelf) return;
        GameObject tmp = pool[index];
        pool[index] = pool[--activeIndex];
        pool[activeIndex] = tmp;
        pool[activeIndex].SetActive(false);
    }
}
