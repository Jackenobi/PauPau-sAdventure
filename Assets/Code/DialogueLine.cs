using UnityEngine;

public class DialogueLine : MonoBehaviour
{
    public string text;
    public DialogueChoice[] choices; //array of choices
    public Sprite portraits;
    public DialogueLine nextLine; //wenn keine choices, dann gehts hier weiter //OPTIONAL
}
