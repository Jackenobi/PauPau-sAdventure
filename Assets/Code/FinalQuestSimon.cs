using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalQuestSimon : MonoBehaviour, IQuestManager
{
    [Header("UI")]
    public Transform questScreen;
    public GameObject questDisplayPrefab;

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
        questTMP.text = "Door Puzzle (Round 1 / 3)";
        puzzleManager.StartPuzzle(this);
    }

    public void OnRoundCompleted(int round)
    {
        questTMP.text = $"Door Puzzle (Round {round + 1} / 3)";
    }

    // NEU: Fehler-Reset
    public void OnRoundReset(int round)
    {
        questTMP.text = $"Wrong! Try again - Round {round + 1} / 3";
    }

    public void OnPuzzleCompleted()
    {
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