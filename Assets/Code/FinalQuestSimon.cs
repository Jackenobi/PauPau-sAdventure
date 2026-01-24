using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalQuestSimon : MonoBehaviour, IQuestManager
{
    [Header("UI")]
    public Transform questScreen;
    public GameObject questDisplayPrefab;

    [Header("Audio")]
    public QuestSoundManager questSoundManager;

    [Header("Puzzle")]
    public SimonPuzzleManager puzzleManager;

    [Header("Scene")]
    public string nextSceneName;

    private TMP_Text questTMP;
    private bool questStarted = false;

    public void StartQuest(string questId)
    {
        StartQuest();
    }

    public void OnAnswerSelected(bool isCorrect) { }

    public void CompleteQuest(string questId) { }

    public void StartQuest()
    {
        if (questStarted)
            return;

        questStarted = true;
        GameObject questDisplay = Instantiate(questDisplayPrefab, questScreen);
        questTMP = questDisplay.GetComponentInChildren<TMP_Text>();
        questTMP.text = "Test (Round 1 / 3)";

        puzzleManager.StartPuzzle(this);
    }

    public void OnRoundCompleted(int round)
    {
        questTMP.text = $"Test (Round {round + 1} / 3)";
    }

    // Fehler-Reset
    public void OnRoundReset(int round)
    {
        // Fehler-Sound abspielen
        if (questSoundManager != null)
            questSoundManager.PlayError();

        questTMP.text = $"Try again - Round {round + 1} / 3";
    }

    public void OnPuzzleCompleted()
    {
        // Quest Complete Sound abspielen
        if (questSoundManager != null)
            questSoundManager.PlayQuestComplete();

        questTMP.text = "YOU DID IT!";
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(nextSceneName);
    }

    public void UpdateQuestProgress(string questId, int current, int total)
    {
        throw new System.NotImplementedException();
    }
}