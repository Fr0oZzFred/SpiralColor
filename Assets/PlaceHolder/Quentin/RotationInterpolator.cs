using UnityEngine;

public class RotationInterpolator : MonoBehaviour {
	[SerializeField] float speed;
	bool active = false;
	float limit = 0;
    private void Update() {
        if(active) transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.rotation.eulerAngles, new Vector3(0, limit, 0), speed));
		else transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.rotation.eulerAngles, new Vector3(0, limit, 0), speed));
	}
    public void Active() {
		active = true;
		limit += 90;
	}
	public void Unactive() {
		active = false;
		limit -= 90;
    }
}