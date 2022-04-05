using UnityEngine;
public class TrianglePlayerController : Controller {
    #region Fields
    [Tooltip("Camera pour baser le déplacement du joueur")]
    [SerializeField]
    Transform playerInputSpace = default, spin = default, spinPivot = default;


    [Header("Speed")]
    [Tooltip("Vitesse maximum")]
    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [Tooltip("Vitesse maximum en nageant")]
    [SerializeField, Range(0f, 100f)]
    float maxSwimSpeed = 5f;


    [Header("Acceleration")]
    [Tooltip("Acceleration maximum")]
    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;

    [Tooltip("Acceleration en l'air")]
    [SerializeField, Range(0f, 100f)]
    float maxAirAcceleration = 1f;

    [Tooltip("Acceleration en nageant")]
    [SerializeField, Range(0f, 100f)]
    float maxSwimAcceleration = 5f;


    [Header("Jump")]
    [Tooltip("Hauteur du saut")]
    [SerializeField, Range(0f, 10f)]
    float jumpHeight = 2f;

    [Tooltip("Nombre de saut sans toucher le sol")]
    [SerializeField, Range(0, 5)]
    int maxAirJumps = 0;


    [Header("Angles")]
    [Tooltip("Angle maximum pour catégoriser le sol")]
    [SerializeField, Range(0, 90)]
    float maxGroundAngle = 25f;

    [Tooltip("Angle maximum pour monter des escaliers")]
    [SerializeField, Range(0, 90)]
    float maxStairsAngle = 50f;

    [Tooltip("Vitesse maximum pour calculer l'angle")]
    [SerializeField, Range(0f, 100f)]
    float maxSnapSpeed = 100f;

    [Tooltip("Range de détection")]
    [SerializeField, Min(0f)]
    float probeDistance = 1f;


    [Header("Water")]
    [Tooltip("Offset pour la submersion")]
    [SerializeField]
    float submergenceOffset = 0.5f;

    [Tooltip("Range pour la submersion")]
    [SerializeField, Min(0.1f)]
    float submergenceRange = 1f;

    [Tooltip("Flottaison")]
    [SerializeField, Min(0f)]
    float buoyancy = 1f;

    [Tooltip("Puissance de la pression de l'eau")]
    [SerializeField, Range(0f, 10f)]
    float waterDrag = 1f;

    [Tooltip("Puissance de modification de la vitesse quand on arrive dans l'eau")]
    [SerializeField, Range(0.01f, 1f)]
    float swimThreshold = 0.5f;


    [Header("Layers")]
    [SerializeField]
    LayerMask probeMask = -1;

    [SerializeField]
    LayerMask stairsMask = -1;

    [SerializeField]
    LayerMask waterMask = 0;


    [Header("Materials")]
    [SerializeField]
    Material normalMaterial = default;

    [SerializeField]
    Material swimmingMaterial = default;


    [Header("Spin Settings")]
    [Tooltip("Vitesse de base")]
    [SerializeField, Min(0.1f)]
    float baseSpeed = 90f;

    [Tooltip("Multiplicateur pour la vitesse durant le movement")]
    [SerializeField, Min(0.1f)]
    float movementSpeed = 5f;
    [Tooltip("Adjustement pour la vitesse durant le movement")]
    [SerializeField]
    float speed;
    Vector3 currentSpeed;


    Rigidbody body, connectedBody, previousConnectedBody;

    Vector3 playerInput;

    Vector3 velocity, connectionVelocity;

    Vector3 connectionWorldPosition, connectionLocalPosition;

    Vector3 upAxis, rightAxis, forwardAxis;

    bool desiredJump;

    Vector3 contactNormal, steepNormal;

    Vector3 lastContactNormal, lastSteepNormal, lastConnectionVelocity;

    int groundContactCount, steepContactCount;

    bool OnGround => groundContactCount > 0;

    bool OnSteep => steepContactCount > 0;

    bool InWater => submergence > 0f;

    bool Swimming => submergence >= swimThreshold;

    float submergence;

    int jumpPhase;

    float minGroundDotProduct, minStairsDotProduct;

    int stepsSinceLastGrounded, stepsSinceLastJump;

    MeshRenderer meshRenderer;

    #endregion

    public float gravityCoef;

    public bool isCurrentlyPlayed = false;
    [SerializeField]
    Color color = default;

    bool desiredTornado;

    [SerializeField] GameObject tornado;

    void PreventSnapToGroundP() {
        stepsSinceLastJump = -1;
    }


    void OnValidate() {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
    }

    void Awake() {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        meshRenderer = spin.GetComponent<MeshRenderer>();
        OnValidate();
    }

    /// <summary>
    /// Check for the playerInput and if is played by the Player Handler
    /// </summary>
    void Update() {
        if (isCurrentlyPlayed) {
            playerInput.x = InputHandler.GetLeftStickValues().x;
            playerInput.z = InputHandler.GetLeftStickValues().y;
            playerInput.y = Swimming ? InputHandler.UpDownTrigger() : 0f;
            playerInput = Vector3.ClampMagnitude(playerInput, 1f);
        } else playerInput = Vector3.zero;

        if (playerInputSpace) {
            rightAxis = ProjectDirectionOnPlane(playerInputSpace.right, upAxis);
            forwardAxis =
                ProjectDirectionOnPlane(playerInputSpace.forward, upAxis);
        } else {
            rightAxis = ProjectDirectionOnPlane(Vector3.right, upAxis);
            forwardAxis = ProjectDirectionOnPlane(Vector3.forward, upAxis);
        }

        if (!isCurrentlyPlayed || InputHandler.Controller == null) {
            UpdateSpin();
            return;
        }

        desiredJump |= InputHandler.Controller.buttonNorth.wasPressedThisFrame;
        desiredTornado = InputHandler.Controller.buttonWest.isPressed;

        UpdateSpin();
    }

    /// <summary>
    /// For the Spin rotation and material
    /// </summary>
    void UpdateSpin() {
        Material ballMaterial = normalMaterial;
        if (Swimming) {
            ballMaterial = swimmingMaterial;
        }
        meshRenderer.material = ballMaterial;
        spinPivot.LookAt(forwardAxis);
        spinPivot.localRotation = Quaternion.Euler(velocity.z, 0f, -velocity.x);
        currentSpeed = Vector3.Lerp(currentSpeed, playerInput, speed);
        spin.Rotate(0f, ((Mathf.Abs(currentSpeed.x) + Mathf.Abs(currentSpeed.z)) * movementSpeed) + baseSpeed * Time.deltaTime, 0f);
    }


    void FixedUpdate() {
        Vector3 gravity = CustomGravity.GetGravity(body.position, out upAxis);
        UpdateState();

        if (InWater) {
            velocity *= 1f - waterDrag * submergence * Time.deltaTime;
        }

        AdjustVelocity();

        if (desiredJump) {
            desiredJump = false;
            Jump(gravity);
        }

        tornado.SetActive(desiredTornado);

        if (InWater) {
            velocity +=
                gravity * ((1f - buoyancy * submergence) * Time.deltaTime);
        } else if (OnGround && velocity.sqrMagnitude < 0.01f) {
            velocity +=
                contactNormal *
                (Vector3.Dot(gravity, contactNormal) * Time.deltaTime);
        } else {
            velocity.x += gravity.x * Time.deltaTime;
            velocity.y += gravityCoef * gravity.y * Time.deltaTime;
            velocity.z += gravity.z * Time.deltaTime;
        }
        body.velocity = velocity;
        ClearState();
    }

    void ClearState() {
        lastContactNormal = contactNormal;
        lastSteepNormal = steepNormal;
        lastConnectionVelocity = connectionVelocity;
        groundContactCount = steepContactCount = 0;
        contactNormal = steepNormal = Vector3.zero;
        connectionVelocity = Vector3.zero;
        previousConnectedBody = connectedBody;
        connectedBody = null;
        submergence = 0f;
    }

    void UpdateState() {
        stepsSinceLastGrounded += 1;
        stepsSinceLastJump += 1;
        velocity = body.velocity;
        if (CheckSwimming() || OnGround || SnapToGround() || CheckSteepContacts()
        ) {
            stepsSinceLastGrounded = 0;
            if (stepsSinceLastJump > 1) {
                jumpPhase = 0;
            }
            if (groundContactCount > 1) {
                contactNormal.Normalize();
            }
        } else {
            contactNormal = upAxis;
        }

        if (connectedBody) {
            if (connectedBody.isKinematic || connectedBody.mass >= body.mass) {
                UpdateConnectionState();
            }
        }
    }

    void UpdateConnectionState() {
        if (connectedBody == previousConnectedBody) {
            Vector3 connectionMovement =
                connectedBody.transform.TransformPoint(connectionLocalPosition) -
                connectionWorldPosition;
            connectionVelocity = connectionMovement / Time.deltaTime;
        }
        connectionWorldPosition = body.position;
        connectionLocalPosition = connectedBody.transform.InverseTransformPoint(
            connectionWorldPosition
        );
    }

    bool CheckSwimming() {
        if (Swimming) {
            groundContactCount = 0;
            contactNormal = upAxis;
            return true;
        }
        return false;
    }

    bool SnapToGround() {
        if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2 || InWater) {
            return false;
        }
        float speed = velocity.magnitude;
        if (speed > maxSnapSpeed) {
            return false;
        }
        if (!Physics.Raycast(
            body.position, -upAxis, out RaycastHit hit,
            probeDistance, probeMask, QueryTriggerInteraction.Ignore
        )) {
            return false;
        }

        float upDot = Vector3.Dot(upAxis, hit.normal);
        if (upDot < GetMinDot(hit.collider.gameObject.layer)) {
            return false;
        }

        groundContactCount = 1;
        contactNormal = hit.normal;
        float dot = Vector3.Dot(velocity, hit.normal);
        if (dot > 0f) {
            velocity = (velocity - hit.normal * dot).normalized * speed;
        }
        connectedBody = hit.rigidbody;
        return true;
    }

    bool CheckSteepContacts() {
        if (steepContactCount > 1) {
            steepNormal.Normalize();
            float upDot = Vector3.Dot(upAxis, steepNormal);
            if (upDot >= minGroundDotProduct) {
                steepContactCount = 0;
                groundContactCount = 1;
                contactNormal = steepNormal;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Complex calcul for the velocity
    /// </summary>
    void AdjustVelocity() {
        float acceleration, speed;
        Vector3 xAxis, zAxis;
        if (InWater) {
            float swimFactor = Mathf.Min(1f, submergence / swimThreshold);
            acceleration = Mathf.LerpUnclamped(
                OnGround ? maxAcceleration : maxAirAcceleration,
                maxSwimAcceleration, swimFactor
            );
            speed = Mathf.LerpUnclamped(maxSpeed, maxSwimSpeed, swimFactor);
            xAxis = rightAxis;
            zAxis = forwardAxis;
        } else {
            acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
            speed = maxSpeed;
            xAxis = rightAxis;
            zAxis = forwardAxis;
        }
        xAxis = ProjectDirectionOnPlane(xAxis, contactNormal);
        zAxis = ProjectDirectionOnPlane(zAxis, contactNormal);

        Vector3 relativeVelocity = velocity - connectionVelocity;

        Vector3 adjustment;
        adjustment.x =
            playerInput.x * speed - Vector3.Dot(relativeVelocity, xAxis);
        adjustment.z =
            playerInput.z * speed - Vector3.Dot(relativeVelocity, zAxis);
        adjustment.y = InWater ?
            playerInput.y * speed - Vector3.Dot(relativeVelocity, upAxis) : 0f;

        adjustment =
            Vector3.ClampMagnitude(adjustment, acceleration * Time.deltaTime);

        velocity += xAxis * adjustment.x + zAxis * adjustment.z;

        if (InWater) {
            velocity += upAxis * adjustment.y;
        }
    }

    void Jump(Vector3 gravity) {
        Vector3 jumpDirection;
        if (OnGround) {
            jumpDirection = contactNormal;
        } else if (OnSteep) {
            jumpDirection = steepNormal;
            jumpPhase = 0;
        } else if (maxAirJumps > 0 && jumpPhase <= maxAirJumps) {
            if (jumpPhase == 0) {
                jumpPhase = 1;
            }
            jumpDirection = contactNormal;
        } else {
            return;
        }

        stepsSinceLastJump = 0;
        jumpPhase += 1;
        float jumpSpeed = Mathf.Sqrt(2f * gravity.magnitude * jumpHeight);
        if (InWater) {
            jumpSpeed *= Mathf.Max(0f, 1f - submergence / swimThreshold);
        }
        jumpDirection = (jumpDirection + upAxis).normalized;
        float alignedSpeed = Vector3.Dot(velocity, jumpDirection);
        if (alignedSpeed > 0f) {
            jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
        }
        velocity += jumpDirection * jumpSpeed;
    }

    void OnCollisionEnter(Collision collision) {
        EvaluateCollision(collision);
    }

    void OnCollisionStay(Collision collision) {
        EvaluateCollision(collision);
    }

    void EvaluateCollision(Collision collision) {
        if (Swimming) {
            return;
        }
        int layer = collision.gameObject.layer;
        float minDot = GetMinDot(layer);
        for (int i = 0; i < collision.contactCount; i++) {
            Vector3 normal = collision.GetContact(i).normal;
            float upDot = Vector3.Dot(upAxis, normal);
            if (upDot >= minDot) {
                groundContactCount += 1;
                contactNormal += normal;
                connectedBody = collision.rigidbody;
            } else {
                if (upDot > -0.01f) {
                    steepContactCount += 1;
                    steepNormal += normal;
                    if (groundContactCount == 0) {
                        connectedBody = collision.rigidbody;
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if ((waterMask & (1 << other.gameObject.layer)) != 0) {
            EvaluateSubmergence(other);
        }
    }

    void OnTriggerStay(Collider other) {
        if ((waterMask & (1 << other.gameObject.layer)) != 0) {
            EvaluateSubmergence(other);
        }
    }

    void EvaluateSubmergence(Collider collider) {
        if (Physics.Raycast(
            body.position + upAxis * submergenceOffset,
            -upAxis, out RaycastHit hit, submergenceRange + 1f,
            waterMask, QueryTriggerInteraction.Collide
        )) {
            submergence = 1f - hit.distance / submergenceRange;
        } else {
            submergence = 1f;
        }
        if (Swimming) {
            connectedBody = collider.attachedRigidbody;
        }
    }

    Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal) {
        return (direction - normal * Vector3.Dot(direction, normal)).normalized;
    }

    float GetMinDot(int layer) {
        return (stairsMask & (1 << layer)) == 0 ?
            minGroundDotProduct : minStairsDotProduct;
    }
    #region Controller abstractFunctions
    public override void RegisterInputs(bool b) {
        isCurrentlyPlayed = b;
    }
    public override void PreventSnapToGround() {
        PreventSnapToGroundP();
    }
    public override void SetControllerLED() {
        InputHandler.SetControllerLED(color);
    }

    public override void Respawn(Vector3 pos) {
        this.transform.position = pos;
        body.velocity = velocity = Vector3.zero;
    }
    public override void SetInputSpace(Transform transform) {
        playerInputSpace = transform;
    }
    #endregion
}
