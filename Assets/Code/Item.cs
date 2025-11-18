using UnityEngine;

public class Item : Interactable

{

    public ItemType type;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    //override erweitert/überschreibt die basis Funktion
    public override void Interact()
    {
        base.Interact();
        Debug.Log("Interacted with Item ");
        Destroy(gameObject);
        GameObject.FindFirstObjectByType<Inventory>().AddItem(this);

        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/ItemPickup", gameObject); //sound ohne emitter
    }
}


public enum ItemType //enumeration = Aufzählung
{
    Tape, Egg, Map, Kid
}