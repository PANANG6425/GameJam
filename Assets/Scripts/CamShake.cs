using UnityEngine;
using System.Collections;

public class CamShake : MonoBehaviour
{
    public static CamShake Instance { get; private set; }

    [SerializeField] private float defaultDuration = 0.1f;
    [SerializeField] private float defaultMagnitude = 0.2f;

    private Transform camTransform;
    private Vector3 originalPos;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            camTransform = Camera.main != null ? Camera.main.transform : transform;
            originalPos = camTransform.localPosition;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Shake()
    {
        Shake(defaultDuration, defaultMagnitude);
    }

    public void Shake(float duration, float magnitude)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            camTransform.localPosition = originalPos;
        }
        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            camTransform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            // Use unscaledDeltaTime so it still shakes during HitStop
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        camTransform.localPosition = originalPos;
        shakeCoroutine = null;
    }
}
