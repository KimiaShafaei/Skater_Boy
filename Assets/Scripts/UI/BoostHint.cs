using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoostHint : MonoBehaviour
{
    public TMP_Text boostHintText;
    private bool hintVisible = false;

    void Start()
    {
        boostHintText.gameObject.SetActive(true);
        hintVisible = true;
    }

    void Update()
    {
        if (hintVisible && Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
        boostHintText.gameObject.SetActive(false);
        hintVisible = false;
        }
    }
}
