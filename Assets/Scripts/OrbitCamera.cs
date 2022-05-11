using UnityEngine;
using Cinemachine;

public class OrbitCamera : MonoBehaviour {
    #region Fields

	[Header("Target Settings")]
	[Tooltip("Cible")]
    [SerializeField]
	Transform focus = default;

	[Tooltip("Distance entre la cible et la camera")]
	[SerializeField, Range(1f, 20f)]
	float distance = 5f;

	[Tooltip("Cercle de focus basé sur le centre de la cible")]
	[SerializeField, Min(0f)]
	float focusRadius = 5f;

	[Tooltip("Focus du centre de l'objet")]
	[SerializeField, Range(0f, 1f)]
	float focusCentering = 0.5f;

	[Header("Camera Settings")]
	[Tooltip("Vitesse de rotation")]
	[SerializeField, Range(1f, 360f)]
	float rotationSpeed = 90f;


	[Tooltip("Limite min de l'angle d'après la cible")]
	[SerializeField, Range(-89f, 89f)]
	float minVerticalAngle = -45f;

	[Tooltip("Limite max de l'angle d'après la cible")]
	[SerializeField, Range(-89f, 89f)]
	float maxVerticalAngle = 45f;

	[Tooltip("Délai pour reset auto de la caméra")]
	[SerializeField, Min(0f)]
	float alignDelay = 5f;

	[Tooltip("Smoothness lors de la rotation")]
	[SerializeField, Range(0f, 90f)]
	float alignSmoothRange = 45f;

	[Tooltip("Vitesse lors du changement gravité")]
	[SerializeField, Min(0f)]
	float upAlignmentSpeed = 360f;

	[Tooltip("Layers qui empêche la vision du personnage")]
	[SerializeField]
	LayerMask obstructionMask = -1;


	[Header("Temporaire")]
	public bool inverseCameraX, inverseCameraY;
	float RotationSpeed {
		get {
			if (UIManager.Instance) return UIManager.Instance.Sensitivity * 30f;
			return rotationSpeed;
		}
	}

	bool InverseCamX {
        get {
			if (UIManager.Instance) return UIManager.Instance.XCam;
			return inverseCameraX;
        }
    }
	bool InverseCamY {
		get {
			if (UIManager.Instance) return UIManager.Instance.YCam;
			return inverseCameraY;
		}
	}

	CinemachineVirtualCamera regularCamera;

	Vector3 focusPoint, previousFocusPoint;

	Vector2 orbitAngles = new Vector2(45f, 0f);

	float lastManualRotationTime;

	Quaternion gravityAlignment = Quaternion.identity;

	Quaternion orbitRotation;
	#endregion


    Vector3 CameraHalfExtends {
		get {
			Vector3 halfExtends;
			halfExtends.y =
				regularCamera.m_Lens.NearClipPlane *
				Mathf.Tan(0.5f * Mathf.Deg2Rad * regularCamera.m_Lens.FieldOfView);;
			halfExtends.x = halfExtends.y * regularCamera.m_Lens.Aspect;
			halfExtends.z = 0f;
			return halfExtends;
		}
	}

	void OnValidate() {
		if (maxVerticalAngle < minVerticalAngle) {
			maxVerticalAngle = minVerticalAngle;
		}
	}

	void Awake() {
		regularCamera = GetComponent<CinemachineVirtualCamera>();
		focusPoint = focus.position;
		transform.localRotation = orbitRotation = Quaternion.Euler(orbitAngles);
	}

	void LateUpdate() {
		UpdateGravityAlignment();
		UpdateFocusPoint();
		if (ManualRotation() || AutomaticRotation()) {
			ConstrainAngles();
			orbitRotation = Quaternion.Euler(orbitAngles);
		}
		Quaternion lookRotation = gravityAlignment * orbitRotation;

		Vector3 lookDirection = lookRotation * Vector3.forward;
		Vector3 lookPosition = focusPoint - lookDirection * distance;

		Vector3 rectOffset = lookDirection * regularCamera.m_Lens.NearClipPlane;
		Vector3 rectPosition = lookPosition + rectOffset;
		Vector3 castFrom = focus.position;
		Vector3 castLine = rectPosition - castFrom;
		float castDistance = castLine.magnitude;
		Vector3 castDirection = castLine / castDistance;

		if (Physics.BoxCast(
			castFrom, CameraHalfExtends, castDirection, out RaycastHit hit,
			lookRotation, castDistance, obstructionMask, QueryTriggerInteraction.Ignore
		)) {
			rectPosition = castFrom + castDirection * hit.distance;
			lookPosition = rectPosition - rectOffset;
		}

		transform.SetPositionAndRotation(lookPosition, lookRotation);
	}

	void UpdateGravityAlignment() {
		Vector3 fromUp = gravityAlignment * Vector3.up;
		Vector3 toUp = CustomGravity.GetUpAxis(focusPoint);
		float dot = Mathf.Clamp(Vector3.Dot(fromUp, toUp), -1f, 1f);
		float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
		float maxAngle = upAlignmentSpeed * Time.deltaTime;

		Quaternion newAlignment =
			Quaternion.FromToRotation(fromUp, toUp) * gravityAlignment;
		if (angle <= maxAngle) {
			gravityAlignment = newAlignment;
		} else {
			gravityAlignment = Quaternion.SlerpUnclamped(
				gravityAlignment, newAlignment, maxAngle / angle
			);
		}
	}

	void UpdateFocusPoint() {
		previousFocusPoint = focusPoint;
		Vector3 targetPoint = focus.position;
		if (focusRadius > 0f) {
			float distance = Vector3.Distance(targetPoint, focusPoint);
			float t = 1f;
			if (distance > 0.01f && focusCentering > 0f) {
				t = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);
			}
			if (distance > focusRadius) {
				t = Mathf.Min(t, focusRadius / distance);
			}
			focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
		} else {
			focusPoint = targetPoint;
		}
	}

	bool ManualRotation() {
		Vector2 input = InputHandler.GetRightStickValues();
		float tmp = input.x;
		input.x = InverseCamY ? input.y : -input.y;
		input.y = InverseCamX ? -tmp : tmp;
		const float e = 0.001f;
		if (input.x < -e || input.x > e || input.y < -e || input.y > e) {
			orbitAngles += RotationSpeed * Time.unscaledDeltaTime * input;
			lastManualRotationTime = Time.unscaledTime;
			return true;
		}
		return false;
	}

	bool AutomaticRotation() {
		if (Time.unscaledTime - lastManualRotationTime < alignDelay) {
			return false;
		}

		Vector3 alignedDelta =
			Quaternion.Inverse(gravityAlignment) *
			(focusPoint - previousFocusPoint);
		Vector2 movement = new Vector2(alignedDelta.x, alignedDelta.z);
		float movementDeltaSqr = movement.sqrMagnitude;
		if (movementDeltaSqr < 0.0001f) {
			return false;
		}

		float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
		float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngles.y, headingAngle));
		float rotationChange =
			rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
		if (deltaAbs < alignSmoothRange) {
			rotationChange *= deltaAbs / alignSmoothRange;
		} else if (180f - deltaAbs < alignSmoothRange) {
			rotationChange *= (180f - deltaAbs) / alignSmoothRange;
		}
		orbitAngles.y =
			Mathf.MoveTowardsAngle(orbitAngles.y, headingAngle, rotationChange);
		return true;
	}

	void ConstrainAngles() {
		orbitAngles.x =
			Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);

		if (orbitAngles.y < 0f) {
			orbitAngles.y += 360f;
		} else if (orbitAngles.y >= 360f) {
			orbitAngles.y -= 360f;
		}
	}

	static float GetAngle(Vector2 direction) {
		float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
		return direction.x < 0f ? 360f - angle : angle;
	}

	public void SetFocus(Transform focus) {
		this.focus = focus;
    }
	public void SetRot(Quaternion q) {
		orbitRotation = q;
		orbitAngles = orbitRotation.eulerAngles;
    }
}
