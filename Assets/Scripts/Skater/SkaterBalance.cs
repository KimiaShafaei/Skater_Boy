using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class SkaterBalance : MonoBehaviour
{
    // public TMP_Text debugText;
    public Rigidbody rb;
    public Transform visuals;
    private float balance = 0f;
    public float maxBalance = 0.8f;
    public float wobbleStrength = 0.4f;
    public float tiltStrength = 2.5f;

    public float maxLeanAngle = 25f;
    public float turnSpeed = 20f;
    public float maxTurnAngle = 35f;
    private float currentTurnAngle = 0f;

    [SerializeField] private float accelerometerSmoothTime = 0.1f;
    private Vector3 filteredAcceleration;
    private Vector3 accelerationVelocity;
    private Vector3 initialAcceleration;
    private DistanceScore distanceScore;

    [Header("Random Force")]
    public float baseForce = 0.5f;
    public float forceGrowMul = 0.2f;
    public float maxForce = 20f;
    public float forceLeanAngle = 8f;
    public float forceLeanReturnSpeed = 4f;
    private float totalDistance = 0f;
    private float distanceTraveled = 0f;
    private float forceLeanOffset = 0f;

    [Header("Forcing Timing")]
    public float minForceInterval = 1f;
    private float lastForceTime = -999f;
    Vector3 lastPosition;
    
    void Start()
    {
        InputSystem.EnableDevice(Accelerometer.current);
        Invoke(nameof(SetInitialAcceleration), 0.1f);

        rb = GetComponent<Rigidbody>();

        distanceScore = GetComponent<DistanceScore>();

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
        balance += wobble * wobbleStrength * Time.deltaTime;
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

        float tilt = filteredAcceleration.x - initialAcceleration.x;
        
        float deadzone = 0.05f;
        if (Mathf.Abs(tilt) < deadzone)
        {
            tilt = 0f;
        }
        
        float targetBalance = Mathf.Clamp(tilt * tiltStrength, -maxBalance, maxBalance);
        balance = Mathf.MoveTowards(balance, targetBalance, Time.deltaTime * 3f);
        
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

        float tilt = filteredAcceleration.x - initialAcceleration.x;

        float deadzone = 0.05f;
        if (Mathf.Abs(tilt) < deadzone)
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
            float deadzone = 0.05f;
            if (Mathf.Abs(tilt) > deadzone)
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
            rb.AddForce(force , ForceMode.Impulse);
            forceLeanOffset = -direction * forceLeanAngle;

            // Debug.Log("apply random force: " + force + "with currentForce: " + currentForce);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            GameOver.Instance.ShowGameOver();
        }
    }
}