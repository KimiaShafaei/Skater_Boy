using UnityEngine;

public class ForceAllertManager : MonoBehaviour
{
    public GameObject rigthAllert;
    public GameObject leftAllert;

    public float alertDuration = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HideAllerts();
    }

    public void ShowRightAllert()
    {
        HideAllerts();
        if (rigthAllert != null)
        {
            rigthAllert.SetActive(true);
            Invoke("HideRightAllert", alertDuration);
        }
    }

    public void ShowLeftAllert()
    {
        HideAllerts();
        if (leftAllert != null);
        {
            leftAllert.SetActive(true);
            Invoke("HideLeftAllert", alertDuration);
        }
    }

    public void HideRightAllert()
    {
        if (rigthAllert != null && rigthAllert.activeSelf)
        {
            rigthAllert.SetActive(false);
        }
        CancelInvoke("HideRightAllert");
    }

    public void HideLeftAllert()
    {
        if (leftAllert != null && leftAllert.activeSelf)
        {
            leftAllert.SetActive(false);
        }
        CancelInvoke("HideLeftAllert");
    }

    public void HideAllerts()
    {
        HideRightAllert();
        HideLeftAllert();
    }
}
