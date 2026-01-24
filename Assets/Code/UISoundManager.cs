using UnityEngine;
using FMODUnity;

/// <summary>
/// Verwaltet UI Sounds (Hover, Click)
/// Singleton - kann von überall aufgerufen werden
/// </summary>
public class UISoundManager : MonoBehaviour
{
    public static UISoundManager Instance { get; private set; }

    [Header("FMOD UI Events")]
    [Tooltip("Sound wenn man über einen Button hovert")]
    public EventReference buttonHoverSound;

    [Tooltip("Sound wenn man einen Button klickt")]
    public EventReference buttonClickSound;

    void Awake()
    {
        // Singleton Pattern - nur eine Instanz
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Spielt den Hover Sound ab
    /// </summary>
    public void PlayHover()
    {
        if (!buttonHoverSound.IsNull)
        {
            RuntimeManager.PlayOneShot(buttonHoverSound);
        }
    }

    /// <summary>
    /// Spielt den Click Sound ab
    /// </summary>
    public void PlayClick()
    {
        if (!buttonClickSound.IsNull)
        {
            RuntimeManager.PlayOneShot(buttonClickSound);
        }
    }
}