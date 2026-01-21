using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

/// <summary>
/// Verwaltet Ambience Sounds für verschiedene Scenes
/// Pro Scene gibt es nur EINE Ambience (keine Parameter/Bereiche)
/// </summary>
public class AmbienceManager : MonoBehaviour
{
    public static AmbienceManager Instance { get; private set; }

    [Header("Fade Settings")]
    [Tooltip("Fade-Out Dauer in Sekunden")]
    public float fadeOutDuration = 2f;

    [Tooltip("Fade-In Dauer in Sekunden")]
    public float fadeInDuration = 2f;

    private EventInstance currentAmbienceInstance;
    private bool isAmbiencePlaying = false;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Startet Ambience für eine Scene mit Crossfade
    /// </summary>
    public void PlayAmbienceForScene(EventReference ambienceEvent)
    {
        if (ambienceEvent.IsNull)
        {
            Debug.LogWarning("[AmbienceManager] Ambience event is null!");
            return;
        }

        StartCoroutine(CrossfadeToAmbience(ambienceEvent));
    }

    /// <summary>
    /// Crossfade von aktueller Ambience zu neuer Ambience
    /// </summary>
    private IEnumerator CrossfadeToAmbience(EventReference newAmbienceEvent)
    {
        // Fade Out alte Ambience
        if (currentAmbienceInstance.isValid() && isAmbiencePlaying)
        {
            yield return StartCoroutine(FadeVolume(1f, 0f, fadeOutDuration));
            StopCurrentAmbience();
        }

        // Kleine Pause zwischen den Sounds
        yield return new WaitForSeconds(0.1f);

        // Neue Ambience starten mit Fade In
        currentAmbienceInstance = RuntimeManager.CreateInstance(newAmbienceEvent);
        currentAmbienceInstance.setVolume(0f);
        currentAmbienceInstance.start();
        isAmbiencePlaying = true;

        yield return StartCoroutine(FadeVolume(0f, 1f, fadeInDuration));

        Debug.Log("[AmbienceManager] Crossfaded to new ambience");
    }

    /// <summary>
    /// Fade Volume Coroutine
    /// </summary>
    private IEnumerator FadeVolume(float startVolume, float targetVolume, float duration)
    {
        if (!currentAmbienceInstance.isValid())
            yield break;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            currentAmbienceInstance.setVolume(volume);
            yield return null;
        }

        currentAmbienceInstance.setVolume(targetVolume);
    }

    /// <summary>
    /// Stoppt die aktuelle Ambience
    /// </summary>
    public void StopCurrentAmbience()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        if (currentAmbienceInstance.isValid())
        {
            currentAmbienceInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            currentAmbienceInstance.release();
            isAmbiencePlaying = false;
            Debug.Log("[AmbienceManager] Ambience stopped");
        }
    }

    /// <summary>
    /// Setzt die Lautstärke der Ambience (0.0 - 1.0)
    /// </summary>
    public void SetAmbienceVolume(float volume)
    {
        if (currentAmbienceInstance.isValid())
        {
            currentAmbienceInstance.setVolume(Mathf.Clamp01(volume));
        }
    }

    void OnDestroy()
    {
        StopCurrentAmbience();
    }
}