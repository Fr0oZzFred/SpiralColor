using UnityEngine;

public class DecalObject : MonoBehaviour
{
    float timeToDie;

    DecalSpawner decalSpawner;
    void Update()
    {
        if (!decalSpawner) return;
        timeToDie -= Time.deltaTime;
        if(timeToDie < 0) {
            gameObject.SetActive(false);
            enabled = false;
        }
    }
    
    public void SetSpawnValue(float time, DecalSpawner spawner) {
        timeToDie = time;
        decalSpawner = spawner;
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.matrix = Matrix4x4.TRS(this.transform.localPosition, this.transform.rotation,Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, this.transform.localScale);
    }
}
