using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class InputSound : MonoBehaviour
{
    public AudioClip TypeSFX;
    public AudioClip ClickSFX;

    [Range(0f, 1f)] public float Volume = 0.5f;

    private System.IDisposable anyKeyObserver;
    private System.IDisposable anyMouseObserver;

    void OnEnable()
    {
        // Subscribe to any button press
        anyKeyObserver = InputSystem.onAnyButtonPress.Call(control =>
        {
            if (control.device is Keyboard)
            {
                AudioManager.Instance.PlaySfx(TypeSFX,Volume);
            }
        });

        anyMouseObserver = InputSystem.onAnyButtonPress.Call(control =>
        {
            if(control.device is Mouse)
            {
                AudioManager.Instance.PlaySfx(ClickSFX, Volume);
            }
        });
       
    }

    void OnDisable()
    {
        anyKeyObserver.Dispose();
        anyMouseObserver.Dispose();
    }
}
