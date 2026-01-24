using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class BeachSailorQuest : MonoBehaviour, IQuestManager
{
    [Header("UI")]
    public Transform questScreen;
    public GameObject questDisplayPrefab;
    public CanvasGroup blackScreen;

    [Header("NPC & Dialogue")]
    public NPCs frosch;
    public DialogueLine froschQuestion; // Die Frage mit Choices

    [Header("Scene")]
    public string nextSceneName = "Main";

    [Header("Blackscreen & Sound")]
    [Tooltip("FMOD Event das beim Blackscreen abgespielt wird")]
    public EventReference transitionSound;

    [Tooltip("Dauer des Blackscreens (in Sekunden)")]
    public float blackscreenDuration = 5f;

    private TMP_Text questTMP;
    private GameObject questDisplay;
    private bool questStarted = false;
    private bool questCompleted = false;

    void Start()
    {
        if (frosch != null)
        {
            frosch.onInteracted += OnFroschTalked;
            // Setze die Frage als Start-Dialog
            if (froschQuestion != null)
                frosch.dialogue = froschQuestion;
        }

        if (blackScreen != null)
        {
            blackScreen.alpha = 0f;
            blackScreen.gameObject.SetActive(false);
        }
    }

    private void OnFroschTalked()
    {
        if (!questStarted)
        {
            questStarted = true;
            StartCoroutine(StartQuest());
        }
    }

    IEnumerator StartQuest()
    {
        // Questlog erstellen
        questDisplay = Instantiate(questDisplayPrefab, questScreen);
        questTMP = questDisplay.GetComponentInChildren<TMP_Text>();
        questTMP.text = "Answer Victor";

        // Placeholder verstecken
        if (QuestPlaceholderManager.Instance != null)
            QuestPlaceholderManager.Instance.OnQuestStarted();

        yield return null;
    }

    // Wird vom DialogueScreen aufgerufen bei Choice-Auswahl
    public void OnAnswerSelected(bool isCorrect)
    {
        if (questCompleted)
            return;

        if (isCorrect)
        {
            questCompleted = true;
            if (questTMP != null)
                questTMP.text = "Let's Go!";

            // Scene laden
            StartCoroutine(LoadNextScene());
        }
        else
        {
            if (questTMP != null)
                questTMP.text = "Get ready";

            // Dialog zurücksetzen zur Frage
            if (frosch != null && froschQuestion != null)
                frosch.dialogue = froschQuestion;
        }
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(2f);

        // ========= PLAYER EINFRIEREN =========
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Player Movement deaktivieren
            var playerMovement = player.GetComponent<Player>();
            if (playerMovement != null)
                playerMovement.enabled = false;

            // Optional: Rigidbody stoppen
            var rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // ========= BLACKSCREEN EINBLENDEN =========
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);

            // Fade In (1 Sekunde)
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

        // ========= FMOD SOUND ABSPIELEN =========
        if (!transitionSound.IsNull)
        {
            RuntimeManager.PlayOneShot(transitionSound);
        }

        // Quest Display entfernen vor Scene-Wechsel
        if (questDisplay != null)
            Destroy(questDisplay);

        // Placeholder anzeigen (optional, da Scene wechselt)
        if (QuestPlaceholderManager.Instance != null)
            QuestPlaceholderManager.Instance.OnQuestCompleted();

        // ========= BLACKSCREEN HALTEN =========
        yield return new WaitForSeconds(blackscreenDuration);

        // ========= SCENE LADEN =========
        SceneManager.LoadScene(nextSceneName);
    }

    // Interface-Methoden (werden nicht benutzt, müssen aber da sein)
    public void StartQuest(string questId) { }
    public void UpdateQuestProgress(string questId, int current, int total) { }
    public void CompleteQuest(string questId) { }

    void OnDestroy()
    {
        if (frosch != null)
        {
            frosch.onInteracted -= OnFroschTalked;
        }
    }
}