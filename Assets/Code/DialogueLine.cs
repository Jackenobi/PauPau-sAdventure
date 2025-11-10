using UnityEngine;

public class DialogueLine : MonoBehaviour
{
    [TextArea]
    public string text;
    public DialogueChoice[] choices;

    // Portraits
    public Sprite npcPortrait;
    public Sprite playerPortrait;

    public DialogueLine nextLine;
    public bool player;
}
