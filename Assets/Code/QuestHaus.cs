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

    [Header("Quest: Huhn")]
    public NPCs huhn;
    public Item shinyItem; // Das Item im Raum
    public DialogueLine huhnStart; // Erstes Gespräch - startet Quest
    public DialogueLine huhnNoItem; // Wenn Item noch nicht gefunden
    public DialogueLine huhnComplete; // Wenn Item abgegeben wird
    public DialogueLine huhnToNextScene; // Letzte Line → lädt Scene

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

        // Shiny Item verstecken (falls du es spawnen willst)
        // Falls es schon in der Scene platziert ist, lass das weg
        // if (shinyItem != null)
        //     shinyItem.gameObject.SetActive(true);
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

        // Shiny Item aktivieren (falls versteckt)
        if (shinyItem != null && !shinyItem.gameObject.activeSelf)
            shinyItem.gameObject.SetActive(true);
    }

    IEnumerator CompleteHausQuest()
    {
        yield return new WaitForSeconds(0.5f);

        if (questTMP != null)
            questTMP.text = "Quest complete! Talk to the chicken again.";
    }

    IEnumerator WaitForDialogueAndLoadScene()
    {
        // Warte kurz damit Dialog angezeigt wird
        yield return new WaitForSeconds(3f);

        if (questTMP != null)
            questTMP.text = "Returning outside...";

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

        yield return new WaitForSeconds(2f);

        // Scene laden
        SceneManager.LoadScene(nextSceneName);
    }
}