using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using System;

/// <summary>
/// Lädt automatisch Musik und Ambience für jede Scene
/// Platziere dies auf dem Music/Ambience GameObject
/// </summary>
public class SceneAudioLoader : MonoBehaviour
{
    [Header("Scene Audio Settings")]
    [Tooltip("Liste aller Scenes mit ihren Audio Events")]
    public SceneAudioConfig[] sceneConfigs;

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
        string sceneName = SceneManager.GetActiveScene().name;
        LoadAudioForScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[SceneAudioLoader] Scene loaded: {scene.name}");
        LoadAudioForScene(scene.name);
    }

    private void LoadAudioForScene(string sceneName)
    {
        // Finde Config für diese Scene
        SceneAudioConfig config = Array.Find(sceneConfigs, c => c.sceneName == sceneName);

        if (config == null)
        {
            Debug.LogWarning($"[SceneAudioLoader] No audio config found for scene: {sceneName}");
            return;
        }

        // MUSIK
        if (config.useMainMusicWithParameters)
        {
            // Main Music mit Parametern (für Main Scene)
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.StartMainMusic();

                // Optional: Setze Parameter-Bereich
                if (!string.IsNullOrEmpty(config.musicAreaParameter))
                {
                    MusicManager.Instance.SetMainMusicArea(config.musicAreaParameter);
                }
            }
        }
        else if (!config.musicEvent.IsNull)
        {
            // Einfache Musik ohne Parameter (Haus, Tempel, etc.)
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.PlaySimpleMusic(config.musicEvent);
            }
        }

        // AMBIENCE
        if (!config.ambienceEvent.IsNull)
        {
            if (AmbienceManager.Instance != null)
            {
                AmbienceManager.Instance.PlayAmbienceForScene(config.ambienceEvent);
            }
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