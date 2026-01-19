using UnityEngine;

/// <summary>
/// Sorgt dafür dass der Cursor im Startscreen/Hauptmenü immer sichtbar ist
/// Platziere diesen auf einem GameObject in deiner Startscreen Scene
/// </summary>
public class StartScreenCursor : MonoBehaviour
{
    void Start()
    {
        // Cursor einschalten und entsperren
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Stelle sicher dass Time.timeScale auf 1 ist
        Time.timeScale = 1f;

        Debug.Log("[StartScreenCursor] Cursor enabled for menu");
    }
}