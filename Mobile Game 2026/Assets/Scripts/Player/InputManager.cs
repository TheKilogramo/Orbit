using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;
    public static Vector2 TouchPosition;
    public static Vector2 TouchDelta;
    public static bool TouchWasPressed;
    public static bool TouchIsHeld;
    public static bool TouchWasReleased;

    private InputAction _touchAction;
    private InputAction _pointAction;
    private Vector2 _lastTouchPosition;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
        _touchAction = PlayerInput.actions["Touch"];
        _pointAction = PlayerInput.actions["Point"];
    }

    private void Update()
    {
        TouchWasPressed = _touchAction.WasPressedThisFrame();
        TouchIsHeld = _touchAction.IsPressed();
        TouchWasReleased = _touchAction.WasReleasedThisFrame();
        TouchPosition = _pointAction.ReadValue<Vector2>();

        if (TouchWasPressed)
        {
            // Reset delta on new press so there's no jump
            TouchDelta = Vector2.zero;
            _lastTouchPosition = TouchPosition;
        }
        else if (TouchIsHeld)
        {
            TouchDelta = TouchPosition - _lastTouchPosition;
            _lastTouchPosition = TouchPosition;
        }
        else
        {
            TouchDelta = Vector2.zero;
        }
    }
}