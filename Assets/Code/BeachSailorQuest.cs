using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BeachSailorQuest : MonoBehaviour, IQuestManager
{
    [Header("UI")]
    public DialogueScreen dialogueScreen;
    public Transform questScreen;
    public GameObject questDisplayPrefab;
    public CanvasGroup blackScreen;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip successSound;

    [Header("NPC")]
    public NPCs sailorNPC;

    [Header("Dialogue")]
    public DialogueLine sailorIntro;      // Dialog VOR der Frage
    public DialogueLine sailorQuestion;   // Dialog MIT Choices
    public DialogueLine sailorComplete;   // Optional nach Abschluss

    [Header("Scene Transition")]
    public string nextSceneName = "NextScene";

    private TMP_Text questTMP;

    private bool introShown = false;
    private bool questStarted = false;
    private bool questCompleted = false;

    void Start()
    {
        if (sailorNPC != null)
        {
            sailorNPC.onInteracted += OnSailorTalked;
            sailorNPC.dialogue = sailorIntro; // 🔑 GANZ WICHTIG
        }

        if (blackScreen != null)
        {
            blackScreen.alpha = 0f;
            blackScreen.gameObject.SetActive(false);
        }

        // Quest-UI sofort anzeigen
        GameObject questDisplay = Instantiate(questDisplayPrefab, questScreen);
        questTMP = questDisplay.GetComponentInChildren<TMP_Text>();
        questTMP.text = "Talk to the sailor at the beach";
    }

    private void OnSailorTalked()
    {
        // 1️⃣ Intro-Dialog
        if (!introShown)
        {
            introShown = true;
            sailorNPC.dialogue = sailorIntro;
            return;
        }

        // 2️⃣ Frage starten
        if (!questStarted)
        {
            questStarted = true;
            questTMP.text = "Answer the sailor's question";
            sailorNPC.dialogue = sailorQuestion;
            return;
        }

        // 3️⃣ Nach Abschluss
        if (questCompleted && sailorComplete != null)
        {
            sailorNPC.dialogue = sailorComplete;
        }
    }

    //  Wird AUTOMATISCH vom DialogueScreen aufgerufen
    public void OnAnswerSelected(bool isCorrect)
    {
        if (!questStarted || questCompleted)
            return;

        if (isCorrect)
        {
            questCompleted = true;
            questTMP.text = "Quest complete!";

            if (sailorComplete != null)
                sailorNPC.dialogue = sailorComplete;

            StartCoroutine(CompleteQuest());
        }
        else
        {
            // falsche Antwort → Frage erneut
            questTMP.text = "Wrong answer. Try again.";
            sailorNPC.dialogue = sailorQuestion;
        }
    }

    IEnumerator CompleteQuest()
    {
        yield return new WaitForSeconds(0.5f);

        // Sound
        if (audioSource != null && successSound != null)
        {
            audioSource.PlayOneShot(successSound);
        }

        // Blackscreen Fade In
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                blackScreen.alpha = t;
                yield return null;
            }
            blackScreen.alpha = 1f;
        }

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(nextSceneName);
    }
    public void StartQuest(string questId) { }
    public void UpdateQuestProgress(string questId, int current, int total) { }
    public void CompleteQuest(string questId) { }

}

