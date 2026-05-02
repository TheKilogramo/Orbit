using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraEffectsManager : MonoBehaviour
{
    public static CameraEffectsManager Instance;

    [Header("References")]
    public Camera cam;
    public Volume volume;

    [Header("Camera Growth Per Orbit")]
    public float cameraIncreasePerOrbit = 0.8f;

    [Header("Durations")]
    public float zoomInDuration = 0.15f;
    public float zoomOutDuration = 0.25f;
    public float distortionDuration = 0.2f;
    public float shakeDuration = 0.1f;

    [Header("Effects")]
    public float zoomInMultiplier = 0.75f; // how far in before zooming out
    public float distortionAmount = -0.6f;
    public float shakeAmount = 0.1f;

    private float targetSize;
    private LensDistortion lens;
    private Coroutine effectRoutine;


    private void Awake()
    {
        Instance = this;

        if (cam == null) cam = Camera.main;
        if (volume == null) volume = FindFirstObjectByType<Volume>();

        if (volume != null)
        {
            volume.profile.TryGet(out lens);
        }

        targetSize = cam.orthographicSize;
    }

    public void OnOrbitAdded()
    {
        targetSize += cameraIncreasePerOrbit;

        if (effectRoutine != null)
            StopCoroutine(effectRoutine);

        effectRoutine = StartCoroutine(GrowthEffect());
    }

  

    private IEnumerator GrowthEffect()
    {
        if (lens != null)
            lens.intensity.value = 0f;

        float startSize = cam.orthographicSize;
        float zoomInTarget = targetSize * zoomInMultiplier;
        float t = 0f;

        //zoom in
        while (t < 1f)
        {
            t += Time.deltaTime / zoomInDuration;
            cam.orthographicSize = Mathf.Lerp(startSize, zoomInTarget, t);
            yield return null;
        }
        cam.orthographicSize = zoomInTarget;

        //distortion / shake
        StartCoroutine(DistortionPulse());
        StartCoroutine(ScreenShake());

        //zoom out
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / zoomOutDuration;
            cam.orthographicSize = Mathf.Lerp(zoomInTarget, targetSize, t);
            yield return null;
        }
        cam.orthographicSize = targetSize;

        if (lens != null)
            lens.intensity.value = 0f;

        effectRoutine = null;
    }

    private IEnumerator DistortionPulse()
    {
        if (lens == null)
            yield break;

        float t = 0f;

        //distortion in
        while (t < 1f)
        {
            t += Time.deltaTime / (distortionDuration * 0.5f);
            lens.intensity.value = Mathf.Lerp(0f, distortionAmount, t);
            yield return null;
        }

        t = 0f;
        //distortion out
        while (t < 1f)
        {
            t += Time.deltaTime / (distortionDuration * 0.5f);
            lens.intensity.value = Mathf.Lerp(distortionAmount, 0f, t);
            yield return null;
        }

        lens.intensity.value = 0f;
    }

    public void ShakeCamera(float duration, float strength)
    {
        //stop any existing shake so they don't stack uncontrollably
        StopCoroutine(ScreenShake());

        //start a new shake with the given values
        StartCoroutine(ScreenShakeCustom(duration, strength));
    }
    private IEnumerator ScreenShakeCustom(float duration, float amount)
    {
        Vector3 originalPos = cam.transform.localPosition;
        float t = 0f;

        while (t < duration)
        {
            //cancel if game paused
            if (Time.timeScale == 0f)
            {
                cam.transform.localPosition = originalPos;
                yield break;
            }

            t += Time.deltaTime;

            float x = Random.Range(-amount, amount);
            float y = Random.Range(-amount, amount);
            cam.transform.localPosition = originalPos + new Vector3(x, y, 0);

            yield return null;
        }

        cam.transform.localPosition = originalPos;
    }


    private IEnumerator ScreenShake()
    {
        Vector3 originalPos = cam.transform.localPosition;
        float t = 0f;

        while (t < shakeDuration)
        {
            t += Time.deltaTime;
            float x = Random.Range(-shakeAmount, shakeAmount);
            float y = Random.Range(-shakeAmount, shakeAmount);
            cam.transform.localPosition = originalPos + new Vector3(x, y, 0);
            yield return null;
        }

        cam.transform.localPosition = originalPos;
    }
}
