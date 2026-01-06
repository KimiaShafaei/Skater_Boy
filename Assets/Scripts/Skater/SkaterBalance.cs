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

    // Filter settings for smoother accelerometer data
    [SerializeField] private float accelerometerSmoothTime = 0.1f;
    private Vector3 filteredAcceleration;
    private Vector3 accelerationVelocity;
    private Vector3 initialAcceleration;
    private DistanceScore distanceScore;

    [Header("Random Force")]
    public float baseForce = 0.5f;
    public float forceGrowMul = 0.2f;
    public float maxForce = 20f;
    private float totalDistance = 0f;
    private float distanceTraveled = 0f;
    Vector3 lastPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Enable accelerometer using Input System
        InputSystem.EnableDevice(Accelerometer.current);
        
        // Wait for one frame to get stable accelerometer reading
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

    // Update is called once per frame
    void Update()
    {
        // ApplyWobble();
        ApplyTilt();
        ApplyRotation();
        UpdateVisuals();
        ApplyRandomForce();
        CheckFall();   
    }

    void ApplyWobble()
    {
        float wobble = Random.Range(-1f, 1f);
        balance += wobble * wobbleStrength * Time.deltaTime;
    }

    void ApplyTilt()
    {
        if (Accelerometer.current == null) return;

        // Read raw accelerometer data
        Vector3 rawAcceleration = Accelerometer.current.acceleration.ReadValue();
        
        // Apply low-pass filter for smoother movement
        filteredAcceleration = Vector3.SmoothDamp(
            filteredAcceleration, 
            rawAcceleration, 
            ref accelerationVelocity, 
            accelerometerSmoothTime
        );

        // Calculate tilt from x-axis (landscape left/right tilt)
        // In portrait mode, you might want to use y-axis instead
        float tilt = filteredAcceleration.x - initialAcceleration.x;
        
        // Adjust sensitivity and deadzone
        float deadzone = 0.05f;
        if (Mathf.Abs(tilt) < deadzone)
        {
            tilt = 0f;
        }
        
        // Map tilt to balance with sensitivity
        float targetBalance = Mathf.Clamp(tilt * tiltStrength, -maxBalance, maxBalance);
        
        // Smoothly move toward target balance
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
            // Smoothly return to center
            currentTurnAngle = Mathf.MoveTowards(
                currentTurnAngle,
                0f,
                turnSpeed * Time.deltaTime
            );
        }
        else
        {
            // Convert tilt into a TARGET angle instead of endless rotation
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

        // Apply rotation ABSOLUTELY, not incrementally
        transform.rotation = Quaternion.Euler(0f, currentTurnAngle, 0f);
    }
    void UpdateVisuals()
    {
        float normalized = Mathf.Clamp(balance / maxBalance, -1f, 1f);
        float targetAngle = -normalized * maxLeanAngle;
        targetAngle = Mathf.Clamp(targetAngle, -maxLeanAngle, maxLeanAngle);
        visuals.localRotation = Quaternion.Euler(0f, 0f, targetAngle);
    }

    void ApplyRandomForce()
    {
        float deltaDistance = Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;
        distanceTraveled += deltaDistance;
        totalDistance += deltaDistance;

        if (distanceTraveled >= 10f)
        {
            distanceTraveled = 0f;
            float direction = Random.value > 0.5f ? 1f : -1f;
            float currentForce = baseForce + (totalDistance * forceGrowMul);
            currentForce = Mathf.Min(currentForce, maxForce);

            Vector3 force = transform.right * direction * currentForce;
            rb.AddForce(force , ForceMode.Impulse);

            // Debug.Log("apply random force: " + force + "with currentForce: " + currentForce);
        }
    }

    void CheckFall()
    {
        if (Mathf.Abs(balance) >= maxBalance)
        {
            Debug.Log("Skater has fallen!");

            // if (distanceScore != null)
            // {
            //     distanceScore.StopScoring();
            // }

            // enabled = false;
        }
    }

    // void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Ground"))
    //     {
    //         GameOver.Instance.ShowGameOver();
    //     }
    // }

    void OnDisable()
    {
        // Clean up when object is disabled
        InputSystem.DisableDevice(Accelerometer.current);
    }
}