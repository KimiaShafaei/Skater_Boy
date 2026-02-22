using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class SkaterBalance : MonoBehaviour
{
    // public TMP_Text debugText;
    public Rigidbody rb;
    public Transform visuals;
    private RandomForce randomForce;
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

    private Vector3 filteredAcceleration;
    private Vector3 accelerationVelocity;
    private Vector3 initialAcceleration;
    private DistanceScore distanceScore;
    private SkaterForwardMove skaterForwardMove;

    private float balance = 0f;
    private float currentTurnAngle = 0f;
    private bool isDead = false;
    
    void Start()
    {
        InputSystem.EnableDevice(Accelerometer.current);
        Invoke(nameof(SetInitialAcceleration), 0.1f);

        rb = GetComponent<Rigidbody>();

        distanceScore = GetComponent<DistanceScore>();
        skaterForwardMove = GetComponent<SkaterForwardMove>();
        randomForce = GetComponent<RandomForce>();
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
        if (SkaterStateManager.Instance.currentState == SkaterState.Rail)
        {
            ApplyWobble();
            ApplyTilt();
            ApplyRotation();
        }

        else if (SkaterStateManager.Instance.currentState == SkaterState.Arena)
        {
            if (skaterForwardMove != null)
            {
                skaterForwardMove.ApplyArenaHorizontalMovement();
            }
            ApplyWobble();
            ApplyTilt();
            ApplyRotation();
        }

        UpdateVisuals();
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
        float forceLean = 0f;
        if (randomForce != null)
        {
            forceLean = randomForce.ForceLeanOffset;
        }
        float finalLean = balanceLean + forceLean;
        finalLean = Mathf.Clamp(finalLean, -maxLeanAngle, maxLeanAngle);

        visuals.localRotation = Quaternion.Euler(0f, 0f, finalLean);

    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            gameOver.ShowGameOver();
        }

        if (collision.gameObject.CompareTag("Arena"))
        {
            SkaterStateManager.Instance.currentState = SkaterState.Arena;
            Debug.Log("Entered Arena");

            SetInitialAcceleration();
            balance = 0f;
            filteredAcceleration = initialAcceleration;
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            gameOver.ShowGameOver();
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Arena"))
        {
            SkaterStateManager.Instance.currentState = SkaterState.Rail;
            Debug.Log("Exited Arena - back to Rail");

            SetInitialAcceleration();
            balance = 0f;
            filteredAcceleration = initialAcceleration;
        }
    }
}