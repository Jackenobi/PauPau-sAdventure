using UnityEngine;

public class NPCs : Interactable
{

    public string namenpc;
    public DialogueLine dialogue;
    public Sprite portrait;
    public DialogueScreen dialogueScreen;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact()
    {
        base.Interact();
        dialogueScreen.ShowDialogue(dialogue, namenpc);
    }


}
