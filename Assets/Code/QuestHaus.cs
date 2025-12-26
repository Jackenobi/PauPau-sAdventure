using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class QuestHaus : MonoBehaviour, IQuestManager

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

    [Header("Quest: Haus NPC")]
    public NPCs hausNPC;
    public Item mapItem; // Das Map-Item das man bekommt
    public DialogueLine[] hausQuestions; // 3 Fragen später
    public DialogueLine hausWrongAnswer;
    public DialogueLine hausComplete;

    [Header("Scene Management")]
    public string mainSceneName = "Prophecy";
    public Vector3 spawnPositionInMain; // Wo Player in Main spawnen soll

    private bool hausQuestStarted = false;
    private bool hausQuestDone = false;
    private TMP_Text questTMP;
    private int correctAnswers = 0;

    void Start()
    {
        // Eingangs-Sound abspielen
        if (hausAudioSource != null && hausEntrySound != null)
        {
            hausAudioSource.PlayOneShot(hausEntrySound);
        }

        // NPC registrieren
        if (hausNPC != null)
        {
            hausNPC.onInteracted += OnHausNPCTalked;
        }

        // Blackscreen setup
        if (blackScreen != null)
        {
            blackScreen.alpha = 0f;
            blackScreen.gameObject.SetActive(false);
        }

        // Map Item verstecken
        if (mapItem != null)
            mapItem.gameObject.SetActive(false);
    }

    private void OnHausNPCTalked()
    {
        if (!hausQuestStarted)
        {
            hausQuestStarted = true;
            StartCoroutine(Questhaus());
            return;
        }

        if (hausQuestStarted && !hausQuestDone)
        {
            if (correctAnswers < hausQuestions.Length)
                hausNPC.dialogue = hausQuestions[correctAnswers];
            return;
        }

        if (hausQuestDone)
        {
            if (hausComplete != null)
                hausNPC.dialogue = hausComplete;
        }
    }

    IEnumerator Questhaus()
    {
        yield return new WaitForSeconds(0.1f);

        GameObject questDisplay = Instantiate(questDisplayPrefab, questScreen);
        questTMP = questDisplay.GetComponentInChildren<TMP_Text>();
        questTMP.text = "Answer the questions";

        correctAnswers = 0;

        if (hausQuestions != null && hausQuestions.Length > 0)
            hausNPC.dialogue = hausQuestions[0];
    }

    public void OnAnswerSelected(bool isCorrect)
    {
        if (!hausQuestStarted || hausQuestDone)
            return;

        if (isCorrect)
        {
            correctAnswers++;
            questTMP.text = $"Answer the questions ({correctAnswers}/1)";

            if (correctAnswers >= 1)
            {
                hausQuestDone = true;
                questTMP.text = "Quest complete!";

                // Map Item spawnen
                if (mapItem != null)
                    mapItem.gameObject.SetActive(true);

                if (hausComplete != null)
                    hausNPC.dialogue = hausComplete;

                // Nach kurzer Zeit zurück zur Main Scene
                StartCoroutine(ReturnToMain());
            }
            else
            {
                if (correctAnswers < hausQuestions.Length)
                    hausNPC.dialogue = hausQuestions[correctAnswers];
            }
        }
        else
        {
            correctAnswers = 0;
            questTMP.text = "Wrong answer! Starting over...";

            if (hausWrongAnswer != null)
                hausNPC.dialogue = hausWrongAnswer;

            StartCoroutine(ResetHausQuest());
        }
    }

    IEnumerator ResetHausQuest()
    {
        yield return new WaitForSeconds(2f);
        questTMP.text = "Answer the questions";

        if (hausQuestions != null && hausQuestions.Length > 0)
            hausNPC.dialogue = hausQuestions[0];
    }

    IEnumerator ReturnToMain()
    {
        yield return new WaitForSeconds(3f);

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

        // Speichere dass Map aufgesammelt wurde
        PlayerPrefs.SetInt("HasMapItem", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(mainSceneName);
    }
}