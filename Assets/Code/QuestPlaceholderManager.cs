using UnityEngine;
using TMPro;

/// <summary>
/// Verwaltet den Placeholder-Text im Quest Screen
/// Zeigt "Look for clues..." wenn keine aktive Quest läuft
/// </summary>
public class QuestPlaceholderManager : MonoBehaviour
{
    public static QuestPlaceholderManager Instance { get; private set; }

    [Header("Placeholder Settings")]
    [Tooltip("Der Placeholder GameObject (mit TMP_Text)")]
    public GameObject placeholderObject;

    [Tooltip("Der Text der angezeigt wird wenn keine Quest aktiv ist")]
    [TextArea(2, 4)]
    public string placeholderText = "";

    private TMP_Text placeholderTMP;
    private int activeQuestCount = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        // Hole TMP_Text Component
        if (placeholderObject != null)
        {
            placeholderTMP = placeholderObject.GetComponent<TMP_Text>();
            if (placeholderTMP != null)
            {
                placeholderTMP.text = placeholderText;
            }
        }

        // Am Anfang Placeholder anzeigen
        ShowPlaceholder();
    }

    /// <summary>
    /// Wird aufgerufen wenn eine Quest startet
    /// </summary>
    public void OnQuestStarted()
    {
        activeQuestCount++;

        if (activeQuestCount > 0)
        {
            HidePlaceholder();
        }

        Debug.Log($"[QuestPlaceholder] Quest started. Active quests: {activeQuestCount}");
    }

    /// <summary>
    /// Wird aufgerufen wenn eine Quest endet
    /// </summary>
    public void OnQuestCompleted()
    {
        activeQuestCount--;

        if (activeQuestCount <= 0)
        {
            activeQuestCount = 0; // Sicherheit
            ShowPlaceholder();
        }

        Debug.Log($"[QuestPlaceholder] Quest completed. Active quests: {activeQuestCount}");
    }

    /// <summary>
    /// Zeigt den Placeholder an
    /// </summary>
    private void ShowPlaceholder()
    {
        if (placeholderObject != null)
        {
            placeholderObject.SetActive(true);
            Debug.Log("[QuestPlaceholder] Showing placeholder");
        }
    }

    /// <summary>
    /// Versteckt den Placeholder
    /// </summary>
    private void HidePlaceholder()
    {
        if (placeholderObject != null)
        {
            placeholderObject.SetActive(false);
            Debug.Log("[QuestPlaceholder] Hiding placeholder");
        }
    }

    /// <summary>
    /// Manuell den Placeholder-Text ändern
    /// </summary>
    public void SetPlaceholderText(string newText)
    {
        placeholderText = newText;
        if (placeholderTMP != null)
        {
            placeholderTMP.text = newText;
        }
    }

    /// <summary>
    /// Reset für Scene-Wechsel
    /// </summary>
    public void ResetQuestCount()
    {
        activeQuestCount = 0;
        ShowPlaceholder();
    }
}