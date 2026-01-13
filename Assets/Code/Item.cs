using UnityEngine;
using FMODUnity;

public class Item : Interactable
{
    public ItemType type;

    [Header("FMOD")]
    public EventReference pickupEvent;

    public override void Interact()
    {
        base.Interact();

        //  FMOD Sound (2D oder 3D – je nach Event)
        if (!pickupEvent.IsNull)
        {
            RuntimeManager.PlayOneShot(pickupEvent, transform.position);
        }

        //  Ins Inventar
        Inventory inventory = GameObject.FindFirstObjectByType<Inventory>();
        if (inventory != null)
        {
            inventory.AddItem(this);
        }

        //  Item entfernen
        Destroy(gameObject);
    }
}

public enum ItemType
{
    Tape,
    Egg,
    Map,
    Kid
}
