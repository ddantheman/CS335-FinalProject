using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// this script runs the timeout failure sequence it first shakes the camera,
// then plays the explosion and smoke particles and then flashes the game over screen and panel
public class FailureSequenceManager : MonoBehaviour
{
    [Header("Shake Settings")]
    // how long the camera shakes/rumbles before the explosion
    public float rumblingDuration = 3f;

    // the shake strength at the start and end/peak of the rumble
    public float startMagnitude = 0.05f;
    public float peakMagnitude = 0.4f;

    [Header("Explosion")]
    // the explosion and smoke particles that play when the failure sequence reaches the explosion
    public ParticleSystem explosionParticles;

    // the audio source that plays the explosion sound
    public AudioSource explosionAudio;

    [Header("Screen Flash")]
    // the full screen image used for the explosion flash
    public Image flashImage;

    // how long it takes the flash to fade out
    public float flashFadeDuration = 0.5f;

    [Header("Game Over")]
    // the panel shown after the explosion
    public GameObject gameOverPanel;

    // how long to wait after the explosion before showing game over
    public float gameOverDelay = 1.5f;

    // makes sure the failure sequence doesn't start more than once
    private bool _sequenceStarted;

    // starts the failure sequence, should be called when the timer hits 0
    public void TriggerFailure()
    {
        if (_sequenceStarted) return;
        _sequenceStarted = true;
        StartCoroutine(FailureRoutine());
    }

    private IEnumerator FailureRoutine()
    {
        // run each part in order so the failure feels like one continuous event
        yield return StartCoroutine(EscalatingRumble());
        TriggerExplosion();
        yield return StartCoroutine(FlashScreen());
        yield return new WaitForSeconds(gameOverDelay);
        ShowGameOver();
    }

    private IEnumerator EscalatingRumble()
    {
        // make sure the main camera has the camera shake attached
        if (CameraShake.Instance == null)
        {
            Debug.LogError("[FSM] CameraShake.Instance is NULL - CameraShake component is not on the Main Camera!");
            yield break;
        }

        Debug.Log($"[FSM] Starting rumble. CameraShake found on: {CameraShake.Instance.gameObject.name}");

        float elapsed = 0f;

        while (elapsed < rumblingDuration)
        {
            float t = elapsed / rumblingDuration;

            // keep increasing the shake strength as the explosion gets sooner
            float currentMagnitude = startMagnitude + (peakMagnitude - startMagnitude) * t;

            // retrigger the shake so the rumble keep getting stronger
            CameraShake.Instance.Shake(0.15f, currentMagnitude);

            elapsed += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        // wait for the final shake to finish before starting the explosion
        yield return new WaitForSeconds(0.15f);
    }

    private void TriggerExplosion()
    {
        // make sure the explosion and smoke particles are assigned in the inspector
        if (explosionParticles != null)
        {
            Debug.Log($"[FSM] Triggering explosion particles at position: {explosionParticles.transform.position}");
            explosionParticles.gameObject.SetActive(true);
            explosionParticles.Play();
        }
        else
        {
            Debug.LogError("[FSM] explosionParticles is NULL - not assigned in Inspector!");
        }

        // make sure the explosion sound is assigned in the inspector
        if (explosionAudio != null)
        {
            explosionAudio.Play();
        }
    }

    private IEnumerator FlashScreen()
    {
        // make sure the flash image is assigned in the inspector
        if (flashImage == null) yield break;

        // start fully visible so the explosion flash happens instantly
        Color c = flashImage.color;
        c.a = 1f;
        flashImage.color = c;
        flashImage.gameObject.SetActive(true);

        // make the flash fade away instead of turning it off instantly
        float elapsed = 0f;
        while (elapsed < flashFadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / flashFadeDuration);
            flashImage.color = c;
            yield return null;
        }

        // turn the flash image off
        c.a = 0f;
        flashImage.color = c;
        flashImage.gameObject.SetActive(false);
    }

    private void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            // if there is no game over panel then restart the current scene instead
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
