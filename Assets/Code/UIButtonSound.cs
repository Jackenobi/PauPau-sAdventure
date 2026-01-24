using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Füge dieses Script zu jedem Button hinzu für automatische Sounds
/// Oder nutze das AutoAddButtonSounds Script (siehe unten)
/// </summary>
[RequireComponent(typeof(Button))]
public class UIButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Sound Settings")]
    [Tooltip("Soll Hover Sound abgespielt werden?")]
    public bool playHoverSound = true;

    [Tooltip("Soll Click Sound abgespielt werden?")]
    public bool playClickSound = true;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
    }

    // Wird aufgerufen wenn Maus über Button kommt
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Nur Sound abspielen wenn Button interaktierbar ist
        if (playHoverSound && button != null && button.interactable)
        {
            if (UISoundManager.Instance != null)
            {
                UISoundManager.Instance.PlayHover();
            }
        }
    }

    // Wird aufgerufen wenn Button geklickt wird
    public void OnPointerClick(PointerEventData eventData)
    {
        // Nur Sound abspielen wenn Button interaktierbar ist
        if (playClickSound && button != null && button.interactable)
        {
            if (UISoundManager.Instance != null)
            {
                UISoundManager.Instance.PlayClick();
            }
        }
    }
}