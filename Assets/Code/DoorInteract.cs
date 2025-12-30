using UnityEngine;

public class DoorInteract : Interactable
{
    public FinalQuestSimon quest;

    public override void Interact()
    {
        base.Interact();
        quest.StartQuest();
    }
}
