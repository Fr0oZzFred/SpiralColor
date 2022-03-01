﻿using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StableFloatingRigidbody : MonoBehaviour {
    #region Fields

	[Header("Rigidbody")]
	[Tooltip("Sleep d'après un timer")]
    [SerializeField]
	bool floatToSleep = false;


	[Header("Water")]
	[Tooltip("Flottaison safe")]
	[SerializeField]
	bool safeFloating = false;

	[Tooltip("Offset pour la submersion")]
	[SerializeField]
	float submergenceOffset = 0.5f;

	[Tooltip("Range pour la submersion")]
	[SerializeField, Min(0.1f)]
	float submergenceRange = 1f;

	[Tooltip("Flottaison")]
	[SerializeField, Min(0f)]
	float buoyancy = 1f;

	[Tooltip("Flottaison Offsets")]
	[SerializeField]
	Vector3[] buoyancyOffsets = default;

	[Tooltip("Puissance de la pression de l'eau")]
	[SerializeField, Range(0f, 10f)]
	float waterDrag = 1f;

	[Tooltip("Layer de l'eau")]
	[SerializeField]
	LayerMask waterMask = 0;

	Rigidbody body;

	float floatDelay;

	float[] submergence;

	Vector3 gravity;
    #endregion
    void Awake() {
		body = GetComponent<Rigidbody>();
		body.useGravity = false;
		submergence = new float[buoyancyOffsets.Length];
	}

	void FixedUpdate() {
		if (floatToSleep) {
			if (body.IsSleeping()) {
				floatDelay = 0f;
				return;
			}

			if (body.velocity.sqrMagnitude < 0.0001f) {
				floatDelay += Time.deltaTime;
				if (floatDelay >= 1f) {
					return;
				}
			} else {
				floatDelay = 0f;
			}
		}

		gravity = CustomGravity.GetGravity(body.position);
		float dragFactor = waterDrag * Time.deltaTime / buoyancyOffsets.Length;
		float buoyancyFactor = -buoyancy / buoyancyOffsets.Length;
		for (int i = 0; i < buoyancyOffsets.Length; i++) {
			if (submergence[i] > 0f) {
				float drag =
					Mathf.Max(0f, 1f - dragFactor * submergence[i]);
				body.velocity *= drag;
				body.angularVelocity *= drag;
				body.AddForceAtPosition(
					gravity * (buoyancyFactor * submergence[i]),
					transform.TransformPoint(buoyancyOffsets[i]),
					ForceMode.Acceleration
				);
				submergence[i] = 0f;
			}
		}
		body.AddForce(gravity, ForceMode.Acceleration);
	}

	void OnTriggerEnter(Collider other) {
		if ((waterMask & (1 << other.gameObject.layer)) != 0) {
			EvaluateSubmergence();
		}
	}

	void OnTriggerStay(Collider other) {
		if (
			!body.IsSleeping() &&
			(waterMask & (1 << other.gameObject.layer)) != 0
		) {
			EvaluateSubmergence();
		}
	}

	void EvaluateSubmergence() {
		Vector3 down = gravity.normalized;
		Vector3 offset = down * -submergenceOffset;
		for (int i = 0; i < buoyancyOffsets.Length; i++) {
			Vector3 p = offset + transform.TransformPoint(buoyancyOffsets[i]);
			if (Physics.Raycast(
				p, down, out RaycastHit hit, submergenceRange + 1f,
				waterMask, QueryTriggerInteraction.Collide
			)) {
				submergence[i] = 1f - hit.distance / submergenceRange;
			} else if (
				  !safeFloating || Physics.CheckSphere(
					  p, 0.01f, waterMask, QueryTriggerInteraction.Collide
				  )
			  ) {
				submergence[i] = 1f;
			}
		}
	}
}