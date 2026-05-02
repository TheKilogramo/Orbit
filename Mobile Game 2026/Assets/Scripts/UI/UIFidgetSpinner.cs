using UnityEngine;
using UnityEngine.EventSystems;

public class UIFidgetSpinner : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Spin Settings")]
    public float spinFriction = 400f;
    public float maxAngularVelocity = 1500f;

    [Header("Smoothing")]
    public float inputSmoothing = 0.15f;

    [Header("Idle Spin")]
    public float idleSpinSpeed = 10f; // ← NEW (degrees per second)

    private RectTransform rectTransform;

    private Vector2 lastLocalPos;
    private Vector2 smoothedLocalPos;
    private bool dragging;
    private int activePointerId;
    private Camera eventCamera;

    private float angularVelocity;
    private float lastTime;

    private void Awake()
    {
        rectTransform = transform as RectTransform;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        activePointerId = eventData.pointerId;
        eventCamera = eventData.pressEventCamera;

        Vector2 lp;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventCamera, out lp);

        lastLocalPos = lp;
        smoothedLocalPos = lp;
        angularVelocity = 0f;
        lastTime = Time.unscaledTime;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging || eventData.pointerId != activePointerId)
            return;

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventCamera, out localPos);

        smoothedLocalPos = Vector2.Lerp(smoothedLocalPos, localPos, 1f - Mathf.Exp(-inputSmoothing * 60f * Time.unscaledDeltaTime));

        float now = Time.unscaledTime;
        float dt = now - lastTime;

        if (dt > 0f)
        {
            float deltaAngle = Vector2.SignedAngle(lastLocalPos, smoothedLocalPos);
            rectTransform.Rotate(0f, 0f, deltaAngle);

            angularVelocity = deltaAngle / dt;
            if (maxAngularVelocity > 0f)
                angularVelocity = Mathf.Clamp(angularVelocity, -maxAngularVelocity, maxAngularVelocity);
        }

        lastLocalPos = smoothedLocalPos;
        lastTime = now;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != activePointerId)
            return;

        dragging = false;
        activePointerId = -1;
        eventCamera = null;
    }

    private void Update()
    {
        float dt = Time.unscaledDeltaTime;

        if (!dragging)
        {
            // Player spin inertia still active?
            if (Mathf.Abs(angularVelocity) > 0.01f)
            {
                float delta = angularVelocity * dt;
                rectTransform.Rotate(0f, 0f, delta);

                float frictionFrame = spinFriction * dt;
                if (Mathf.Abs(angularVelocity) <= frictionFrame)
                    angularVelocity = 0f;
                else
                    angularVelocity -= Mathf.Sign(angularVelocity) * frictionFrame;
            }
            else
            {
                // Not dragging + no velocity → apply idle auto-spin
                rectTransform.Rotate(0f, 0f, idleSpinSpeed * dt);
            }
        }
    }
}
