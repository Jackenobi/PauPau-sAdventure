public interface IQuestManager
{
    void OnAnswerSelected(bool isCorrect);

    void StartQuest(string questId);
    void UpdateQuestProgress(string questId, int current, int total);
    void CompleteQuest(string questId);
}

