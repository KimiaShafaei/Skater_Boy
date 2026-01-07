using UnityEngine;

public class SkaterForwardMove : MonoBehaviour
{
    public float firstSpeed = 3f;
    public float maxSpeed = 50f;
    private int lastScore = 0;
    private float currentSpeed;
    public DistanceScore distanceScore;

    void Start()
    {
        currentSpeed = firstSpeed;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        
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
