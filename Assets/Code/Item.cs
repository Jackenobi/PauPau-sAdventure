using UnityEngine;

public class Item : Interactable
{
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
    }
}
