using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class pausemenu : MonoBehaviour
{
    public Button resumeButton;
    public Button soundButton;
    public Button controlsButton;
    public Button exitButton;
    public GameObject pausePanel;
    public GameObject soundPanel;
    public GameObject dialogueScreen;
    public PlayerInput input;

    void Start()
    {
        input.actions.FindActionMap("Player").FindAction("Pause").performed += TogglePause;
        input.actions.FindActionMap("UI").FindAction("Pause").performed += TogglePause;

        resumeButton.onClick.AddListener(Unpause);
        soundButton.onClick.AddListener(() =>
        {
            pausePanel.SetActive(false);
        });
        exitButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Startscreen");
            input.actions.FindActionMap("Player").FindAction("Pause").performed -= TogglePause;
            input.actions.FindActionMap("UI").FindAction("Pause").performed -= TogglePause;
        });

        Time.timeScale = 1f;

        // Cursor beim Start ausschalten (für Spiel-Scenes)
        HideCursor();
    }

    // Wenn scene wechselt wird OnDestroy ausgelöst
    private void OnDestroy()
    {
        if (input != null)
        {
            input.actions.FindActionMap("Player").FindAction("Pause").performed -= TogglePause;
            input.actions.FindActionMap("UI").FindAction("Pause").performed -= TogglePause;
        }
    }

    private void TogglePause(InputAction.CallbackContext obj)
    {
        if (Time.timeScale == 0f)
            Unpause();
        else
            pause();
    }

    private void pause()
    {
        Time.timeScale = 0f; // Pause
        pausePanel.SetActive(true); // Menü öffnen
        input.SwitchCurrentActionMap("UI"); // UI Steuerung
        soundPanel.SetActive(false);

        // Cursor einschalten für Menü-Navigation
        ShowCursor();
    }

    private void Unpause()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);

        if (!dialogueScreen.activeSelf)
            input.SwitchCurrentActionMap("Player");

        soundPanel.SetActive(false);

        // Cursor wieder ausschalten fürs Spiel
        HideCursor();
    }

    /// <summary>
    /// Zeigt den Cursor für Menü-Navigation
    /// </summary>
    private void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// Versteckt den Cursor fürs Gameplay
    /// </summary>
    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}