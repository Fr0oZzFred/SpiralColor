using UnityEngine;
using System.Collections.Generic;

public class DecalSpawner : MonoBehaviour {

	[SerializeField] CrossPlayerController cross;
	[SerializeField] LayerMask layermask;
	[SerializeField] DecalObject decal;
	[SerializeField] int amountToPool;
	[SerializeField] int decalDeathTimer;

	List<DecalObject> pooledDecals;

    int activeIndex;

    private void Start() {
		activeIndex = 0;
		pooledDecals = new List<DecalObject>();
		DecalObject tmp;
        for (int i = 0; i < amountToPool; i++) {
			tmp = Instantiate(decal);
			tmp.gameObject.SetActive(false);
			tmp.enabled = false;
			pooledDecals.Add(tmp);
        }
    }
    private void OnTriggerExit(Collider other) {
		if (layermask == (layermask | (1 << other.gameObject.layer))) {
            InstantiateDecal(transform.position, transform.rotation);
			Debug.Log(cross.GetNormal);
		}
    }
	
	void InstantiateDecal(Vector3 position, Quaternion rotation) {
		DecalObject spawn = GetPooledDecal();

		spawn.gameObject.transform.position = position;
		spawn.gameObject.transform.rotation = rotation;


		spawn.SetSpawnValue(decalDeathTimer, this);
		spawn.gameObject.SetActive(true);
		spawn.enabled = true;

		activeIndex++;

	}
	DecalObject GetPooledDecal() {
		activeIndex %= amountToPool;
		return pooledDecals[activeIndex];
    }
}
