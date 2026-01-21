using UnityEngine;
using FMODUnity;

public class QuestSoundManager : MonoBehaviour
{
    [Header("FMOD Events")]
    public EventReference questCompleteSound;
    public EventReference questFailSound;

    [Header("Simon Puzzle Sounds")]
    public EventReference buttonPressSound;
    public EventReference doorLightSound;
    public EventReference errorSound;

    public void PlayQuestComplete()
    {
        if (!questCompleteSound.IsNull)
        {
            FMOD.Studio.EventInstance instance = RuntimeManager.CreateInstance(questCompleteSound);
            instance.start();
            instance.release();
        }
        else
        {
            Debug.LogError("Quest Complete Sound is NULL!");
        }
    }

    public void PlayQuestFail()
    {
        if (!questFailSound.IsNull)
        {
            RuntimeManager.PlayOneShot(questFailSound);
        }
        else
        {
            Debug.LogError("Quest Fail Sound is NULL!");
        }
    }

    // Neuer Sound für Button-Drücke
    public void PlayButtonPress()
    {
        if (!buttonPressSound.IsNull)
        {
            RuntimeManager.PlayOneShot(buttonPressSound);
        }
        else
        {
            Debug.LogWarning("Button Press Sound is NULL!");
        }
    }

    // Neuer Sound für Tür-Lichter
    public void PlayDoorLight()
    {
        if (!doorLightSound.IsNull)
        {
            RuntimeManager.PlayOneShot(doorLightSound);
        }
        else
        {
            Debug.LogWarning("Door Light Sound is NULL!");
        }
    }

    // Neuer Sound für Fehler
    public void PlayError()
    {
        if (!errorSound.IsNull)
        {
            RuntimeManager.PlayOneShot(errorSound);
        }
        else
        {
            Debug.LogWarning("Error Sound is NULL!");
        }
    }
}