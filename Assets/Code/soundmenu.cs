using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

/// <summary>
/// Sound Menu mit FMOD VCA Kontrolle
/// Speichert Einstellungen in PlayerPrefs und wendet sie auf FMOD VCAs an
/// </summary>
public class SoundMenu : MonoBehaviour
{
    [Header("UI Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider voiceSlider;

    [Header("FMOD VCA Pfade")]
    [Tooltip("VCA Pfade aus FMOD - müssen EXAKT mit FMOD Namen übereinstimmen!")]
    public string masterVCAPath = "vca:/Master";
    public string musicVCAPath = "vca:/Music";
    public string sfxVCAPath = "vca:/SFX";
    public string voiceVCAPath = "vca:/Voice";

    // FMOD VCA Referenzen
    private VCA masterVCA;
    private VCA musicVCA;
    private VCA sfxVCA;
    private VCA voiceVCA;

    void Start()
    {
        // Warte bis FMOD bereit ist
        StartCoroutine(InitializeWhenReady());
    }

    /// <summary>
    /// Wartet bis FMOD bereit ist, dann lädt VCAs und Slider
    /// </summary>
    private System.Collections.IEnumerator InitializeWhenReady()
    {
        // Warte 1 Frame damit FMOD initialisiert ist
        yield return null;

        // FMOD VCAs laden
        LoadVCAs();

        // Master Slider
        if (masterSlider != null)
        {
            masterSlider.value = PlayerPrefs.GetFloat("masterVolume", 1.0f);
            masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            OnMasterVolumeChanged(masterSlider.value); // Initial setzen
        }

        // Music Slider
        if (musicSlider != null)
        {
            musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 1.0f);
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            OnMusicVolumeChanged(musicSlider.value);
        }

        // SFX Slider
        if (sfxSlider != null)
        {
            sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume", 1.0f);
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            OnSFXVolumeChanged(sfxSlider.value);
        }

        // Voice Slider
        if (voiceSlider != null)
        {
            voiceSlider.value = PlayerPrefs.GetFloat("voiceVolume", 1.0f);
            voiceSlider.onValueChanged.AddListener(OnVoiceVolumeChanged);
            OnVoiceVolumeChanged(voiceSlider.value);
        }
    }

    /// <summary>
    /// Lädt alle FMOD VCAs
    /// </summary>
    private void LoadVCAs()
    {
        try
        {
            masterVCA = FMODUnity.RuntimeManager.GetVCA(masterVCAPath);
            Debug.Log($"[SoundMenu] Master VCA loaded: {masterVCAPath} - Valid: {masterVCA.isValid()}");

            musicVCA = FMODUnity.RuntimeManager.GetVCA(musicVCAPath);
            Debug.Log($"[SoundMenu] Music VCA loaded: {musicVCAPath} - Valid: {musicVCA.isValid()}");

            sfxVCA = FMODUnity.RuntimeManager.GetVCA(sfxVCAPath);
            Debug.Log($"[SoundMenu] SFX VCA loaded: {sfxVCAPath} - Valid: {sfxVCA.isValid()}");

            voiceVCA = FMODUnity.RuntimeManager.GetVCA(voiceVCAPath);
            Debug.Log($"[SoundMenu] Voice VCA loaded: {voiceVCAPath} - Valid: {voiceVCA.isValid()}");

            if (!masterVCA.isValid() || !musicVCA.isValid() || !sfxVCA.isValid() || !voiceVCA.isValid())
            {
                Debug.LogError("[SoundMenu] Some VCAs are INVALID! Check VCA paths in Inspector!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SoundMenu] Failed to load FMOD VCAs: {e.Message}");
        }
    }

    /// <summary>
    /// Master Volume geändert (0.0 - 1.0)
    /// </summary>
    private void OnMasterVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("masterVolume", value);
        PlayerPrefs.Save();

        if (masterVCA.isValid())
        {
            FMOD.RESULT result = masterVCA.setVolume(value);
            if (result == FMOD.RESULT.OK)
            {
                Debug.Log($"[SoundMenu] Master volume set to: {value}");
            }
            else
            {
                Debug.LogWarning($"[SoundMenu] Failed to set Master volume: {result}");
            }
        }
        else
        {
            Debug.LogWarning("[SoundMenu] Master VCA is not valid!");
        }
    }

    /// <summary>
    /// Music Volume geändert (0.0 - 1.0)
    /// </summary>
    private void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("musicVolume", value);
        PlayerPrefs.Save();

        if (musicVCA.isValid())
        {
            FMOD.RESULT result = musicVCA.setVolume(value);
            if (result == FMOD.RESULT.OK)
            {
                Debug.Log($"[SoundMenu] Music volume set to: {value}");
            }
            else
            {
                Debug.LogWarning($"[SoundMenu] Failed to set Music volume: {result}");
            }
        }
        else
        {
            Debug.LogWarning("[SoundMenu] Music VCA is not valid!");
        }
    }

    /// <summary>
    /// SFX Volume geändert (0.0 - 1.0)
    /// </summary>
    private void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("sfxVolume", value);
        PlayerPrefs.Save();

        if (sfxVCA.isValid())
        {
            FMOD.RESULT result = sfxVCA.setVolume(value);
            if (result == FMOD.RESULT.OK)
            {
                Debug.Log($"[SoundMenu] SFX volume set to: {value}");
            }
            else
            {
                Debug.LogWarning($"[SoundMenu] Failed to set SFX volume: {result}");
            }
        }
        else
        {
            Debug.LogWarning("[SoundMenu] SFX VCA is not valid!");
        }
    }

    /// <summary>
    /// Voice Volume geändert (0.0 - 1.0)
    /// </summary>
    private void OnVoiceVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("voiceVolume", value);
        PlayerPrefs.Save();

        if (voiceVCA.isValid())
        {
            FMOD.RESULT result = voiceVCA.setVolume(value);
            if (result == FMOD.RESULT.OK)
            {
                Debug.Log($"[SoundMenu] Voice volume set to: {value}");
            }
            else
            {
                Debug.LogWarning($"[SoundMenu] Failed to set Voice volume: {result}");
            }
        }
        else
        {
            Debug.LogWarning("[SoundMenu] Voice VCA is not valid!");
        }
    }

    /// <summary>
    /// Optional: Reset auf Standard-Werte
    /// </summary>
    public void ResetToDefaults()
    {
        if (masterSlider != null) masterSlider.value = 1.0f;
        if (musicSlider != null) musicSlider.value = 1.0f;
        if (sfxSlider != null) sfxSlider.value = 1.0f;
        if (voiceSlider != null) voiceSlider.value = 1.0f;

        Debug.Log("[SoundMenu] Reset to default volumes");
    }
}