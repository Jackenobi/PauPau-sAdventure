using UnityEngine;
using FMODUnity;

public class QuestSoundManager : MonoBehaviour
{
    [Header("FMOD Events")]
    public EventReference questCompleteSound;
    public EventReference questFailSound;

    public void PlayQuestComplete()
    {
        Debug.Log("PlayQuestComplete called!");
        Debug.Log("Event is null: " + questCompleteSound.IsNull);
        Debug.Log("Event path: " + questCompleteSound.Path);

        if (!questCompleteSound.IsNull)
        {
            FMOD.Studio.EventInstance instance = RuntimeManager.CreateInstance(questCompleteSound);
            instance.start();
            instance.release();
            Debug.Log("Sound should be playing now!");
        }
        else
        {
            Debug.LogError("Quest Complete Sound is NULL!");
        }
    }

    public void PlayQuestFail()
    {
        Debug.Log("PlayQuestFail called!");

        if (!questFailSound.IsNull)
        {
            RuntimeManager.PlayOneShot(questFailSound);
        }
        else
        {
            Debug.LogError("Quest Fail Sound is NULL!");
        }
    }
}