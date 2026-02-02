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
        float finalSpeedd = currentSpeed;
        if (isBoosting)
        {
            finalSpeedd *= boostMultiplier;
            Debug.Log("Boosting" + finalSpeedd);
        }
        
        transform.Translate(Vector3.forward * finalSpeedd * Time.deltaTime);
        
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
