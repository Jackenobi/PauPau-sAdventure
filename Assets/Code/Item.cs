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

        // FMOD Sound
        if (!pickupEvent.IsNull)
        {
            RuntimeManager.PlayOneShot(pickupEvent, transform.position);
        }

        // Player Animation triggern
        Player player = GameObject.FindFirstObjectByType<Player>();
        if (player != null && player.animator != null)
        {
            player.animator.SetTrigger("Pickup"); // Trigger im Animator
        }

        // Ins Inventar
        Inventory inventory = GameObject.FindFirstObjectByType<Inventory>();
        if (inventory != null)
        {
            inventory.AddItem(this);
        }

        // Item entfernen
        Destroy(gameObject);
    }
}

public enum ItemType
{
    Tape,
    Egg,
    Map,
    Kid,
    ShinyObject,
}
