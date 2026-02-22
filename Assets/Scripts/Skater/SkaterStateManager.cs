using UnityEngine;

public enum SkaterState
{
    Rail,
    Arena
}

public class SkaterStateManager : MonoBehaviour
{
    public static SkaterStateManager Instance;
    public SkaterState currentState = SkaterState.Rail;

    private void Awake()
    {
        Instance = this;
    }
}
