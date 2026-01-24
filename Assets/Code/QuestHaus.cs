using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class QuestHaus : MonoBehaviour
{
    [Header("UI")]
    public DialogueScreen dialogueScreen;
    public Transform questScreen;
    public GameObject questDisplayPrefab;
    public Inventory inventory;
    public CanvasGroup blackScreen;

    [Header("Audio")]
    public AudioSource hausAudioSource;
    public AudioClip hausEntrySound;
    public QuestSoundManager questSoundManager;

    [Header("Quest: Huhn")]
    public NPCs huhn;
    public Item shinyItem;
    public DialogueLine huhnStart;
    public DialogueLine huhnNoItem;
    public DialogueLine huhnComplete;
    public DialogueLine huhnToNextScene;

    [Header("Scene Management")]
    public string nextSceneName = "MainAfter";

    private bool hausQuestStarted = false;
    private bool hausQuestDone = false;
    private TMP_Text questTMP;
    private GameObject questDisplay;

    void Start()
    {
        // Eingangs-Sound abspielen
        if (hausAudioSource != null && hausEntrySound != null)
        {
            hausAudioSource.PlayOneShot(hausEntrySound);
        }

        // Huhn registrieren
        if (huhn != null)
        {
            huhn.onInteracted += OnHuhnTalked;

            // Start-Dialog setzen
            if (huhnStart != null)
                huhn.dialogue = huhnStart;
        }

        // Blackscreen setup
        if (blackScreen != null)
        {
            blackScreen.alpha = 0f;
            blackScreen.gameObject.SetActive(false);
        }
    }

    private void OnHuhnTalked()
    {
        // Erstes Gespräch - Quest starten
        if (!hausQuestStarted)
        {
            hausQuestStarted = true;
            StartCoroutine(StartHausQuest());
            return;
        }

        // Quest läuft, aber Item noch nicht gefunden
        if (hausQuestStarted && !hausQuestDone && !inventory.HasItem(ItemType.ShinyObject))
        {
            if (huhnNoItem != null)
                huhn.dialogue = huhnNoItem;
            return;
        }

        // Quest abschließen - Item wird abgegeben
        if (hausQuestStarted && !hausQuestDone && inventory.HasItem(ItemType.ShinyObject))
        {
            hausQuestDone = true;

            if (huhnComplete != null)
                huhn.dialogue = huhnComplete;

            // Quest abschließen
            StartCoroutine(CompleteHausQuest());
            return;
        }

        // Nach Quest abgeschlossen - zur nächsten Scene
        if (hausQuestDone)
        {
            if (huhnToNextScene != null)
            {
                huhn.dialogue = huhnToNextScene;

                // Warte bis Dialog zu Ende ist, dann Scene laden
                StartCoroutine(WaitForDialogueAndLoadScene());
            }
        }
    }

    IEnumerator StartHausQuest()
    {
        yield return new WaitForSeconds(0.1f);

        // Questlog erstellen
        questDisplay = Instantiate(questDisplayPrefab, questScreen);
        questTMP = questDisplay.GetComponentInChildren<TMP_Text>();
        questTMP.text = "Find the shiny object";

        // Placeholder verstecken
        if (QuestPlaceholderManager.Instance != null)
            QuestPlaceholderManager.Instance.OnQuestStarted();

        // Shiny Item aktivieren (falls versteckt)
        if (shinyItem != null && !shinyItem.gameObject.activeSelf)
            shinyItem.gameObject.SetActive(true);
    }

    IEnumerator CompleteHausQuest()
    {
        yield return new WaitForSeconds(0.5f);

        // Quest Complete Sound abspielen
        if (questSoundManager != null)
            questSoundManager.PlayQuestComplete();

        if (questTMP != null)
            questTMP.text = "Talk to Laura again.";
    }

    IEnumerator WaitForDialogueAndLoadScene()
    {
        // Warte kurz damit Dialog angezeigt wird
        yield return new WaitForSeconds(3f);

        if (questTMP != null)
            questTMP.text = "Listen to the prophecy...";

        // Blackscreen einblenden
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            float fadeTime = 1f;
            float elapsed = 0f;

            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                blackScreen.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeTime);
                yield return null;
            }
            blackScreen.alpha = 1f;
        }

        // Quest Display entfernen vor Scene-Wechsel
        if (questDisplay != null)
            Destroy(questDisplay);

        // Placeholder anzeigen (optional, da Scene wechselt)
        if (QuestPlaceholderManager.Instance != null)
            QuestPlaceholderManager.Instance.OnQuestCompleted();

        yield return new WaitForSeconds(2f);

        // Scene laden
        SceneManager.LoadScene(nextSceneName);
    }
}