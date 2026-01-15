using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private TMP_Text questTMP;
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
        GameObject questDisplay = Instantiate(questDisplayPrefab, questScreen);
        questTMP = questDisplay.GetComponentInChildren<TMP_Text>();
        questTMP.text = "Answer the frog's question";

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
                questTMP.text = "Correct! Returning to beach...";

            // Scene laden
            StartCoroutine(LoadNextScene());
        }
        else
        {
            if (questTMP != null)
                questTMP.text = "Wrong answer! Try again.";

            // Dialog zurücksetzen zur Frage
            if (frosch != null && froschQuestion != null)
                frosch.dialogue = froschQuestion;
        }
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(2f);

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

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(nextSceneName);
    }

    // Interface-Methoden (werden nicht benutzt, müssen aber da sein)
    public void StartQuest(string questId) { }
    public void UpdateQuestProgress(string questId, int current, int total) { }
    public void CompleteQuest(string questId) { }
}