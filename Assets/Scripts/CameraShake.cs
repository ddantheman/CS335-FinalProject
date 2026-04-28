using System.Collections;
using UnityEngine;

// this script handles the screen shake effect by moving the camera slightly for a short time.
// attach this to the main camera in the scene
public class CameraShake : MonoBehaviour
{
    // give the other scripts an easier way to trigger the shake
    public static CameraShake Instance { get; private set; }

    // used to return the camera to its normal position after each shake
    private Vector3 _originalLocalPos;

    private void Awake()
    {
        // keep one shared CameraShake so that other scripts can trigger the effect
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _originalLocalPos = transform.localPosition;
    }

    // starts a camera shake
    public void Shake(float duration, float magnitude)
    {
        // stop the previous shake so overlapping calls don't move the camera too far away
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float progress = elapsed / duration;

            // start at full strength and reduce the shake as progress gets closer to 1
            float currentMagnitude = magnitude * (1f - progress);

            Vector3 offset = Random.insideUnitSphere * currentMagnitude;
            offset.z = 0f; // keep the camera depth unchanged

            transform.localPosition = _originalLocalPos + offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = _originalLocalPos;
    }
}
