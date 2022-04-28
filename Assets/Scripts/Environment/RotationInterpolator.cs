using UnityEngine;

public class RotationInterpolator : MonoBehaviour {
	[SerializeField] float speed;
	float angle;
    private void Update() {
		//transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.rotation.eulerAngles, new Vector3(0, angle, 0), speed));
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, angle, 0), speed);
	}
    public void Right() {
		angle += 90;
	}
	public void Left() {
		angle -= 90;
    }
}