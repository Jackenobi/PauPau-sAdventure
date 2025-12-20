using UnityEngine;

public class DialogueLine : MonoBehaviour
{
    [TextArea]
    public string text;

    public bool player;

    [Header("Choices")]
    public bool hasChoices; //  NEU
    public DialogueChoice[] choices;

    public Sprite npcPortrait;
    public Sprite playerPortrait;

    public DialogueLine nextLine;
}

