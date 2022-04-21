using UnityEngine;

public class RotationInterpolator : MonoBehaviour {
	[SerializeField] float speed;
	bool active = false;
	float angle = 0;
    private void Update() {
        if(active) transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.rotation.eulerAngles, new Vector3(0, angle, 0), speed));
		else transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.rotation.eulerAngles, new Vector3(0, angle, 0), speed));
	}
    public void Right() {
		active = true;
		angle += 90;
	}
	public void Left() {
		active = false;
		angle -= 90;
    }
}