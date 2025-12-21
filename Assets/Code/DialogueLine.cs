using UnityEngine;

public class DialogueLine : MonoBehaviour
{
    [TextArea]
    public string text;
    public DialogueChoice[] choices;
   
    [Header("Speaker")]
    public string speakerName;


    // Portraits
    public Sprite npcPortrait;
    public Sprite playerPortrait;
    public DialogueLine nextLine;
    public bool player;

    // Property das prüft ob WIRKLICH Choices existieren
    public bool hasChoices
    {
        get
        {
            // Prüft ob Array existiert UND mindestens 1 Element hat
            return choices != null && choices.Length > 0;
        }
    }
}
