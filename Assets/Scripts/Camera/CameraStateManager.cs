using Unity.Cinemachine;
using UnityEngine;

public class CameraState : MonoBehaviour
{
    public CinemachineCamera railCamera;
    public CinemachineCamera arenaCamera;
    
    void Update()
    {
        if (SkaterStateManager.Instance.currentState == SkaterState.Rail)
        {
            railCamera.Priority = 20;
            arenaCamera.Priority = 5;
        }
        else if (SkaterStateManager.Instance.currentState == SkaterState.Arena)
        {
            railCamera.Priority = 5;
            arenaCamera.Priority = 20;
        }
        
    }
}
