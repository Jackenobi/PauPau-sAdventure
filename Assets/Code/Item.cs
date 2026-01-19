using UnityEngine;
using System.Collections;
using FMODUnity;

public class Item : Interactable
{
    public ItemType type;

    [Header("FMOD")]
    public EventReference pickupEvent;

    [Header("Pickup Settings")]
    [Tooltip("Wie lange der Player eingefroren ist (in Sekunden)")]
    public float freezeDuration = 1f;

    public override void Interact()
    {
        base.Interact();

        // Player finden
        Player player = GameObject.FindFirstObjectByType<Player>();
        if (player != null)
        {
            // Coroutine starten für Pickup-Sequenz
            player.StartCoroutine(PickupSequence(player));
        }
        else
        {
            // Fallback wenn kein Player gefunden
            CompletePickup();
        }
    }

    private IEnumerator PickupSequence(Player player)
    {
        // 1. Player Movement einfrieren
        player.FreezeMovement(true);

        // 2. Animation triggern
        if (player.animator != null)
        {
            player.animator.SetTrigger("Pickup");
        }

        // 3. FMOD Sound abspielen
        if (!pickupEvent.IsNull)
        {
            RuntimeManager.PlayOneShot(pickupEvent, transform.position);
        }

        // 4. Item SOFORT ins Inventar und verschwinden lassen
        CompletePickup();

        // 5. Warten (Player bleibt eingefroren)
        yield return new WaitForSeconds(freezeDuration);

        // 6. Player Movement wieder freigeben
        player.FreezeMovement(false);
    }

    private void CompletePickup()
    {
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