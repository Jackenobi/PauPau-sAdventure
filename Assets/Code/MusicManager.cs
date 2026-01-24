using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("FMOD Music Events")]
    [Tooltip("Haupt-Musik Event mit Parametern (z.B. OST_PauPau+Egventure)")]
    public EventReference mainMusicEvent;

    [Header("Main Scene Parameter")]
    [Tooltip("Name des Parameters in FMOD (z.B. 'Scene')")]
    public string sceneParameterName = "Scene";

    [Header("Fade Settings")]
    [Tooltip("Fade-Out Dauer in Sekunden")]
    public float fadeOutDuration = 1.5f;

    [Tooltip("Fade-In Dauer in Sekunden")]
    public float fadeInDuration = 1.5f;

    private EventInstance currentMusicInstance;
    private bool isMusicPlaying = false;
    private bool isUsingParameterMusic = false;
    private string currentScene = "";
    private Coroutine fadeCoroutine;
    private float targetVolume = 1f; // Ziel-Volume für Fades

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
    /// Startet die Haupt-Musik mit Parametern (für Main Scene)
    /// </summary>
    public void StartMainMusic()
    {
        if (mainMusicEvent.IsNull)
        {
            Debug.LogError("[MusicManager] Main Music Event is not assigned!");
            return;
        }

        // Wenn schon die Main Music läuft, nichts tun
        if (isMusicPlaying && isUsingParameterMusic)
        {
            Debug.Log("[MusicManager] Main music already playing");
            return;
        }

        // Crossfade wenn andere Musik läuft
        if (currentMusicInstance.isValid() && isMusicPlaying)
        {
            StartCoroutine(CrossfadeToMainMusic());
            return;
        }

        StopCurrentMusic();

        currentMusicInstance = RuntimeManager.CreateInstance(mainMusicEvent);
        currentMusicInstance.start();
        isMusicPlaying = true;
        isUsingParameterMusic = true;

        // Fade In
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeIn());

        Debug.Log("[MusicManager] Main music started with fade-in");
    }

    /// <summary>
    /// Crossfade zur Main Music
    /// </summary>
    private IEnumerator CrossfadeToMainMusic()
    {
        // Fade Out alte Musik
        yield return StartCoroutine(FadeOut());
        StopCurrentMusic();

        yield return new WaitForSeconds(0.1f);

        // Main Music starten
        currentMusicInstance = RuntimeManager.CreateInstance(mainMusicEvent);
        currentMusicInstance.start();
        isMusicPlaying = true;
        isUsingParameterMusic = true;

        yield return StartCoroutine(FadeIn());

        Debug.Log("[MusicManager] Crossfaded to main music");
    }

    /// <summary>
    /// Wechselt den Bereich in der Main Scene (beach, city, forest)
    /// </summary>
    public void SetMainMusicArea(string area)
    {
        if (!isMusicPlaying || !isUsingParameterMusic)
        {
            Debug.LogWarning("[MusicManager] Main music not playing. Starting it first.");
            StartMainMusic();
        }

        float parameterValue = 0f;

        switch (area.ToLower())
        {
            case "beach":
                parameterValue = 0f;
                break;
            case "city":
                parameterValue = 1f;
                break;
            case "forest":
                parameterValue = 2f;
                break;
            default:
                Debug.LogWarning($"[MusicManager] Unknown area: {area}");
                return;
        }

        FMOD.RESULT result = currentMusicInstance.setParameterByName(sceneParameterName, parameterValue);

        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError($"[MusicManager] Failed to set parameter '{sceneParameterName}': {result}");
        }
        else
        {
            currentScene = area;
            Debug.Log($"[MusicManager] Music area changed to: {area} (value: {parameterValue})");
        }
    }

    /// <summary>
    /// Spielt einfache Musik ohne Parameter (für Haus, Tempel, Startscreen, etc.)
    /// </summary>
    public void PlaySimpleMusic(EventReference musicEvent)
    {
        if (musicEvent.IsNull)
        {
            Debug.LogWarning("[MusicManager] Music event is null!");
            return;
        }

        StartCoroutine(CrossfadeToMusic(musicEvent, false));
    }

    /// <summary>
    /// Crossfade zu neuer Musik
    /// </summary>
    private IEnumerator CrossfadeToMusic(EventReference newMusicEvent, bool isParameterMusic)
    {
        // Fade Out alte Musik
        if (currentMusicInstance.isValid() && isMusicPlaying)
        {
            yield return StartCoroutine(FadeOut());
            StopCurrentMusic();
        }

        // Kleine Pause
        yield return new WaitForSeconds(0.1f);

        // Neue Musik starten mit Fade In
        currentMusicInstance = RuntimeManager.CreateInstance(newMusicEvent);
        currentMusicInstance.start();
        isMusicPlaying = true;
        isUsingParameterMusic = isParameterMusic;

        yield return StartCoroutine(FadeIn());

        Debug.Log("[MusicManager] Crossfaded to new music");
    }

    /// <summary>
    /// Fade In - nutzt Volume Multiplier statt direktes setVolume
    /// So werden VCAs respektiert!
    /// </summary>
    private IEnumerator FadeIn()
    {
        if (!currentMusicInstance.isValid())
            yield break;

        float elapsed = 0f;
        targetVolume = 1f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float volume = Mathf.Lerp(0f, targetVolume, elapsed / fadeInDuration);

            // Nutze Volume als Multiplier - VCAs bleiben wirksam!
            currentMusicInstance.setVolume(volume);
            yield return null;
        }

        currentMusicInstance.setVolume(targetVolume);
    }

    /// <summary>
    /// Fade Out
    /// </summary>
    private IEnumerator FadeOut()
    {
        if (!currentMusicInstance.isValid())
            yield break;

        // Hole aktuelles Volume
        currentMusicInstance.getVolume(out float currentVolume, out float _);

        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float volume = Mathf.Lerp(currentVolume, 0f, elapsed / fadeOutDuration);
            currentMusicInstance.setVolume(volume);
            yield return null;
        }

        currentMusicInstance.setVolume(0f);
    }

    /// <summary>
    /// Stoppt die aktuelle Musik
    /// </summary>
    public void StopCurrentMusic()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        if (currentMusicInstance.isValid())
        {
            currentMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            currentMusicInstance.release();
            isMusicPlaying = false;
            isUsingParameterMusic = false;
            Debug.Log("[MusicManager] Music stopped");
        }
    }

    /// <summary>
    /// Stoppt Musik mit Fade Out
    /// </summary>
    public void StopMusicWithFade()
    {
        if (currentMusicInstance.isValid() && isMusicPlaying)
        {
            StartCoroutine(StopMusicFadeCoroutine());
        }
    }

    private IEnumerator StopMusicFadeCoroutine()
    {
        yield return StartCoroutine(FadeOut());
        StopCurrentMusic();
    }

    /// <summary>
    /// Pausiert die Musik
    /// </summary>
    public void PauseMusic(bool pause)
    {
        if (currentMusicInstance.isValid())
        {
            currentMusicInstance.setPaused(pause);
            Debug.Log($"[MusicManager] Music paused: {pause}");
        }
    }

    void OnDestroy()
    {
        StopCurrentMusic();
    }
}