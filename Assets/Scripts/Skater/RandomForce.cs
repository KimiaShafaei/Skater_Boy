using UnityEngine;
using System.Collections;

public class RandomForce : MonoBehaviour
{
    public Rigidbody rb;
    public ForceAllertManager forceAllertManager;

    [Header("Random Force")]
    public float baseForce = 0.5f;
    public float forceGrowMul = 0.2f;
    public float maxForce = 20f;

    [Header("Forcing Timing")]
    public float minForceInterval = 1f;
    public float forceWarningDelay = 1.5f;
    private bool isForcePending = false;

    [Header("Force Visual Reaction")]
    public float baseForceLeanAngle = 10f;
    public float maxForceLeanAngle = 30f;
    public float forceLeanAngleMul = 0.1f;
    public float forceLeanReturnSpeed = 4f;


    private float totalDistance = 0f;
    private float distanceTraveled = 0f;
    private float forceLeanOffset = 0f;
    public float ForceLeanOffset => forceLeanOffset;
    private float lastForceTime = -999f;
    private Vector3 lastPosition;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;
    }


    void Update()
    {
        if (SkaterStateManager.Instance.currentState == SkaterState.Rail)
        {
            ApplyRandomForce();
            forceLeanOffset = Mathf.MoveTowards(forceLeanOffset, 0f, Time.deltaTime * forceLeanReturnSpeed);    
        }
    }

    void ApplyRandomForce()
    {
        float deltaDistance = Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;
        distanceTraveled += deltaDistance;
        totalDistance += deltaDistance;

        if (distanceTraveled >= 10f && Time.time - lastForceTime >= minForceInterval && !isForcePending)
        {
            distanceTraveled = 0f;
            lastForceTime = Time.time;
            StartCoroutine(ForceWithWarning());
        }
    }

    IEnumerator ForceWithWarning()
    {
        isForcePending = true;

        float direction = Random.value > 0.5f ? 1f : -1f;
        float currentForce = baseForce + (totalDistance * forceGrowMul);
        currentForce = Mathf.Min(currentForce, maxForce);

        if (direction > 0)
        {
            forceAllertManager.ShowRightAllert();
        }
        else
        {
            forceAllertManager.ShowLeftAllert();
        }

        yield return new WaitForSeconds(forceWarningDelay);

        Vector3 force = transform.right * direction * currentForce;
        rb.AddForce(force , ForceMode.Impulse);

        float calculateLeanAngle = baseForceLeanAngle + (currentForce - baseForce) * forceLeanAngleMul;
        calculateLeanAngle = Mathf.Min(calculateLeanAngle , maxForceLeanAngle);
        forceLeanOffset = -direction * calculateLeanAngle;

        isForcePending = false;

        Debug.Log("apply random force: " + force + "with currentForce: " + currentForce);
        // Debug.Log("force lean offset:   " + forceLeanOffset);
    }
}
