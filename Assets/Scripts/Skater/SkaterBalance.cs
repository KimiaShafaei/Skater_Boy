using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class SkaterBalance : MonoBehaviour
{
    // public TMP_Text debugText;
    public Rigidbody rb;
    public Transform visuals;

    public ForceAllertManager forceAllertManager;
    public GameOver gameOver;

    [Header("Tilt Settings")]
    public float tiltStrength = 2.5f;
    public float tiltDeadzone = 0.08f;
    public float boostTiltMultiplier = 1.2f;

    public float tiltSensivity = 0.4f;
    [SerializeField] private float accelerometerSmoothTime = 0.1f;
    public float maxBalance = 0.8f;

    [Header("Wobble Settings")]
    public float wobbleStrength = 0.4f;
    public float boostWobbleMultiplier = 2f;

    [Header("Turning")]
    public float turnSpeed = 20f;
    public float maxTurnAngle = 35f;

    [Header("Visual Lean")]
    public float maxLeanAngle = 25f;

    [Header("Random Force")]
    public float baseForce = 0.5f;
    public float forceGrowMul = 0.2f;
    public float maxForce = 20f;

    [Header("Forcing Timing")]
    public float minForceInterval = 1f;

    [Header("Force Visual Reaction")]
    public float baseForceLeanAngle = 10f;
    public float maxForceLeanAngle = 30f;
    public float forceLeanAngleMul = 0.1f;
    public float forceLeanReturnSpeed = 4f;

    private Vector3 filteredAcceleration;
    private Vector3 accelerationVelocity;
    private Vector3 initialAcceleration;
    private DistanceScore distanceScore;
    private SkaterForwardMove skaterForwardMove;

    private float balance = 0f;
    private float currentTurnAngle = 0f;
    private float totalDistance = 0f;
    private float distanceTraveled = 0f;
    private float forceLeanOffset = 0f;
    private float lastForceTime = -999f;
    Vector3 lastPosition;
    
    void Start()
    {
        InputSystem.EnableDevice(Accelerometer.current);
        Invoke(nameof(SetInitialAcceleration), 0.1f);

        rb = GetComponent<Rigidbody>();

        distanceScore = GetComponent<DistanceScore>();
        skaterForwardMove = GetComponent<SkaterForwardMove>();

        lastPosition = transform.position;
    }

    void SetInitialAcceleration()
    {
        if (Accelerometer.current != null)
        {
            initialAcceleration = Accelerometer.current.acceleration.ReadValue();
            filteredAcceleration = initialAcceleration;
        }
    }

    void Update()
    {
        ApplyWobble();
        ApplyTilt();
        ApplyRotation();
        UpdateVisuals();
        ApplyRandomForce();
    }

    void ApplyWobble()
    {
        float wobble = Random.Range(-1f, 1f);
        float finalWobble = wobbleStrength;
        if (skaterForwardMove != null && skaterForwardMove.isBoosting)
        {
            finalWobble *= boostWobbleMultiplier;
        }
        balance += wobble * finalWobble * Time.deltaTime;
    }

    void ApplyTilt()
    {
        if (Accelerometer.current == null) return;
        Vector3 rawAcceleration = Accelerometer.current.acceleration.ReadValue();
        
        filteredAcceleration = Vector3.SmoothDamp(
            filteredAcceleration, 
            rawAcceleration, 
            ref accelerationVelocity, 
            accelerometerSmoothTime
        );

        float tilt = (filteredAcceleration.x - initialAcceleration.x) * tiltSensivity;

        if (skaterForwardMove != null && skaterForwardMove.isBoosting)
        {
            tilt *= boostTiltMultiplier;
        }
        
        if (Mathf.Abs(tilt) < tiltDeadzone)
        {
            tilt = 0f;
        }
        
        float targetBalance = Mathf.Clamp(tilt * tiltStrength, -maxBalance, maxBalance);
        balance = Mathf.MoveTowards(balance, targetBalance, Time.deltaTime * 1.5f);
        
        // Update debug text if available
        // if (debugText != null)
        // {
        //     debugText.text = 
        //         $"Accel X: {filteredAcceleration.x:F2}\n" +
        //         $"Accel Y: {filteredAcceleration.y:F2}\n" +
        //         $"Accel Z: {filteredAcceleration.z:F2}\n" +
        //         $"Balance: {balance:F2}\n" +
        //         $"Tilt: {tilt:F2}\n";
        // }
    }
    
    void ApplyRotation()
    {
        if (Accelerometer.current == null) return;

        float tilt = (filteredAcceleration.x - initialAcceleration.x) * tiltSensivity;

        if (Mathf.Abs(tilt) < tiltDeadzone)
        {
            currentTurnAngle = Mathf.MoveTowards(
                currentTurnAngle,
                0f,
                turnSpeed * Time.deltaTime
            );
        }
        else
        {
            float targetAngle = Mathf.Clamp(
                tilt * maxTurnAngle,
                -maxTurnAngle,
                maxTurnAngle
            );

            currentTurnAngle = Mathf.MoveTowards(
                currentTurnAngle,
                targetAngle,
                turnSpeed * Time.deltaTime
            );
        }
        transform.rotation = Quaternion.Euler(0f, currentTurnAngle, 0f);
    }

    void UpdateVisuals()
    {
        float normalized = Mathf.Clamp(balance / maxBalance, -1f, 1f);
        float balanceLean = -normalized * maxLeanAngle;
        if (Accelerometer.current != null)
        {
            float tilt = filteredAcceleration.x - initialAcceleration.x;
            if (Mathf.Abs(tilt) > tiltDeadzone)
            {
                forceLeanOffset = Mathf.MoveTowards(forceLeanOffset, 0f, Time.deltaTime * forceLeanReturnSpeed);
                
            }
        }
        float finalLean = balanceLean + forceLeanOffset;
        finalLean = Mathf.Clamp(finalLean, -maxLeanAngle, maxLeanAngle);
        visuals.localRotation = Quaternion.Euler(0f, 0f, finalLean);

    }

    void ApplyRandomForce()
    {
        float deltaDistance = Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;
        distanceTraveled += deltaDistance;
        totalDistance += deltaDistance;

        if (distanceTraveled >= 10f && Time.time - lastForceTime >= minForceInterval)
        {
            distanceTraveled = 0f;
            lastForceTime = Time.time;
            float direction = Random.value > 0.5f ? 1f : -1f;
            float currentForce = baseForce + (totalDistance * forceGrowMul);
            currentForce = Mathf.Min(currentForce, maxForce);

            Vector3 force = transform.right * direction * currentForce;


            if (direction > 0)
            {
                forceAllertManager.ShowRightAllert();
            }
            else
            {
                forceAllertManager.ShowLeftAllert();
            }


            rb.AddForce(force , ForceMode.Impulse);

            float calculateLeanAngle = baseForceLeanAngle + (currentForce - baseForce) * forceLeanAngleMul;
            calculateLeanAngle = Mathf.Min(calculateLeanAngle , maxForceLeanAngle);
            forceLeanOffset = -direction * calculateLeanAngle;

            // Debug.Log("apply random force: " + force + "with currentForce: " + currentForce);
            // Debug.Log("force lean offset:   " + forceLeanOffset);

        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            gameOver.ShowGameOver();
        }
    }
}