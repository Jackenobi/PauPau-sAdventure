using UnityEngine;
using FMODUnity;

public class DialogueLine : MonoBehaviour
{
    [TextArea]
    public string text;
    public DialogueChoice[] choices;

    [Header("Speaker")]
    public string speakerName;

    [Header("Voice (FMOD)")]
    public EventReference voiceEvent;


    // Portraits
    public Sprite npcPortrait;
    public Sprite playerPortrait;
    public DialogueLine nextLine;
    public bool player;
    

    public bool hasChoices
    {
        get
        {
            return choices != null && choices.Length > 0;
        }
    }
}
