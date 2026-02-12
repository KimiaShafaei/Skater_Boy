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

    private int lastScore = 0;
    private float currentSpeed;
    public DistanceScore distanceScore;

    void Start()
    {
        currentSpeed = firstSpeed;
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
}
