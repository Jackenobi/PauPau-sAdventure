using UnityEngine;

public class Interactable : MonoBehaviour
{

    public event System.Action onInteracted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void Interact()
    {
        Debug.Log("Interacted with " + gameObject);
        onInteracted?.Invoke(); //? ist ein null check, wie if(onInteracted != null)
    }
}
