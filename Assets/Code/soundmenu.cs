using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class soundmenu : MonoBehaviour
{

    public Slider master;
    public Slider SFX;
    public Slider music;

    //public InputActionReference lookAction; // für Mausempfindlichkeit

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        master.onValueChanged.AddListener((newValue) =>
        {
            PlayerPrefs.SetFloat("masterVolume", newValue);
        });
        master.value = PlayerPrefs.GetFloat("masterVolume", 0.5f);

        SFX.onValueChanged.AddListener((newValue) =>
        {
            PlayerPrefs.SetFloat("sfxVolume", newValue);
        });
        SFX.value = PlayerPrefs.GetFloat("sfxVolume", 0.5f);

        music.onValueChanged.AddListener((newValue) =>
        {
            PlayerPrefs.SetFloat("musicVolume", newValue);
        });
        music.value = PlayerPrefs.GetFloat("musicVolume", 0.5f);
        //to do: fmod volume anpassen
        //Stimmen hinzufügen als slider und hier
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
