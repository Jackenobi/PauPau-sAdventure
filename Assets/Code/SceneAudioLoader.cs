using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using System;

/// <summary>
/// Lädt automatisch Musik und Ambience für jede Scene
/// Platziere dies auf dem Music GameObject
/// </summary>
public class SceneAudioLoader : MonoBehaviour
{
    [Header("Scene Audio Settings")]
    [Tooltip("Liste aller Scenes mit ihren Audio Events")]
    public SceneAudioConfig[] sceneConfigs;

    private bool hasLoadedOnce = false;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // Audio für aktuelle Scene beim Start laden
        if (!hasLoadedOnce)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            LoadAudioForScene(sceneName);
            hasLoadedOnce = true;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[SceneAudioLoader] Scene loaded: {scene.name}");
        LoadAudioForScene(scene.name);
        hasLoadedOnce = true;
    }

    private void LoadAudioForScene(string sceneName)
    {
        Debug.Log($"[SceneAudioLoader] Loading audio for scene: {sceneName}");

        // Finde Config für diese Scene
        SceneAudioConfig config = Array.Find(sceneConfigs, c => c.sceneName == sceneName);

        if (config == null)
        {
            Debug.LogWarning($"[SceneAudioLoader] No audio config found for scene: {sceneName}");
            return;
        }

        Debug.Log($"[SceneAudioLoader] Config found! UseMainMusic: {config.useMainMusicWithParameters}, MusicEvent: {config.musicEvent.Path}");

        // MUSIK
        if (config.useMainMusicWithParameters)
        {
            Debug.Log("[SceneAudioLoader] Starting main music with parameters...");
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.StartMainMusic();
                if (!string.IsNullOrEmpty(config.musicAreaParameter))
                {
                    MusicManager.Instance.SetMainMusicArea(config.musicAreaParameter);
                }
            }
            else
            {
                Debug.LogError("[SceneAudioLoader] MusicManager.Instance is NULL!");
            }
        }
        else if (!config.musicEvent.IsNull)
        {
            Debug.Log($"[SceneAudioLoader] Playing simple music: {config.musicEvent.Path}");
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.PlaySimpleMusic(config.musicEvent);
            }
            else
            {
                Debug.LogError("[SceneAudioLoader] MusicManager.Instance is NULL!");
            }
        }
        else
        {
            Debug.LogWarning("[SceneAudioLoader] No music event configured!");
        }

        // AMBIENCE
        if (!config.ambienceEvent.IsNull)
        {
            Debug.Log($"[SceneAudioLoader] Playing ambience: {config.ambienceEvent.Path}");
            if (AmbienceManager.Instance != null)
            {
                AmbienceManager.Instance.PlayAmbienceForScene(config.ambienceEvent);
            }
        }
        else
        {
            Debug.Log("[SceneAudioLoader] No ambience event configured (this is OK)");
        }
    }
}

/// <summary>
/// Konfiguration für Audio einer Scene
/// </summary>
[System.Serializable]
public class SceneAudioConfig
{
    [Tooltip("Name der Unity Scene (muss genau übereinstimmen)")]
    public string sceneName;

    [Header("Music Settings")]
    [Tooltip("Nutze Main Music mit Parametern? (nur für Main Scene)")]
    public bool useMainMusicWithParameters = false;

    [Tooltip("Musik Event (wenn NICHT Main Music mit Parametern)")]
    public EventReference musicEvent;

    [Tooltip("Musik-Bereich Parameter (nur wenn Main Music: beach/city/forest)")]
    public string musicAreaParameter = "";

    [Header("Ambience Settings")]
    [Tooltip("Ambience Event für diese Scene")]
    public EventReference ambienceEvent;
}