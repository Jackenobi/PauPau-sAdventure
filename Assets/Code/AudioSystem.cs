using FMODUnity;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{

    public static bool musicStarted;
    public static AudioSystem instance;

    public StudioEventEmitter musicEmitter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!musicStarted)
        {
            DontDestroyOnLoad(gameObject);
            musicEmitter.Play();
            musicStarted = true;
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
