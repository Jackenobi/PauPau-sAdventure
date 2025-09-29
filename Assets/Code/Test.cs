using UnityEngine;

public class Test : MonoBehaviour
{

    //public! variables können im inspector zugewiesen werden
    public GameObject objectToDestroy;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("start");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("update");
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Destroy(gameObject); //this

        //Destroy(collision.gameObject); //das andere Objekt

        Destroy(objectToDestroy);
    }
    
}
