using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkaterForwardMove : MonoBehaviour
{
    [Header("Speed")]
    public float firstSpeed = 3f;
    public float maxSpeed = 50f;

    [Header("Boost")]
    public float boostMultiplier = 1.5f;
    public bool isBoosting {get; private set;}

    [Header("Arena Movement")]
    public float arenaMaxSpeed = 6f;
    public float tiltSensivity = 0.4f;
    public float tiltDeadzone = 0.08f;

    private int lastScore = 0;
    private float currentSpeed;
    public DistanceScore distanceScore;
    private Vector3 initialAcceleration;

    void Start()
    {
        currentSpeed = firstSpeed;
        if (Accelerometer.current != null)
        {
            initialAcceleration = Accelerometer.current.acceleration.ReadValue();
        }
    }

    void Update()
    {
        isBoosting = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed;
        float finalSpeed = currentSpeed;
        if (isBoosting)
        {
            finalSpeed *= boostMultiplier;
            // Debug.Log("Boosting" + finalSpeed);
        }
        
        transform.Translate(Vector3.forward * finalSpeed * Time.deltaTime);
        
        if (distanceScore.score > lastScore)
        {
            lastScore = Mathf.FloorToInt(distanceScore.score);
            currentSpeed += 1f;
            
            if (currentSpeed >= maxSpeed)
            {
                currentSpeed = maxSpeed;
            }
        }
    }

    public void ApplyArenaHorizontalMovement()
    {
        if (Accelerometer.current == null) return;

        Vector3 rawAcceleration = Accelerometer.current.acceleration.ReadValue();
        float tilt = (rawAcceleration.x - initialAcceleration.x) * tiltSensivity;
        
        if (Mathf.Abs(tilt) < tiltDeadzone)
        {
            tilt = 0f;
        }
        
        float horizontalMovement = tilt * arenaMaxSpeed;
        transform.Translate(Vector3.right * horizontalMovement * Time.deltaTime);
    }
}
