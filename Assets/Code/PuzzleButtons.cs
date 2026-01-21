using System.Collections;
using UnityEngine;

public class PuzzleButton : Interactable
{
    public int buttonIndex;
    public bool isFloorButton;

    public Material baseMaterial;
    public Material glowMaterial;

    // AudioSource wird nicht mehr benötigt, da wir FMOD nutzen
    // aber wir lassen es für Kompatibilität drin
    public AudioSource audioSource;

    private MeshRenderer rend;
    private SimonPuzzleManager manager;
    private QuestSoundManager soundManager;

    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        manager = Object.FindFirstObjectByType<SimonPuzzleManager>();
        soundManager = Object.FindFirstObjectByType<QuestSoundManager>();
        rend.material = baseMaterial;
    }

    // Für Tür-Buttons (Sequenz-Anzeige)
    public void LightUp()
    {
        StartCoroutine(LightRoutine());
    }

    IEnumerator LightRoutine()
    {
        rend.material = glowMaterial;

        // Sound für Tür-Licht
        if (soundManager != null)
            soundManager.PlayDoorLight();

        yield return new WaitForSeconds(0.5f);
        rend.material = baseMaterial;
    }

    // Fehler-Anzeige
    public void ShowError(Material errorMat, AudioClip errorClip)
    {
        StartCoroutine(ErrorRoutine(errorMat));
    }

    IEnumerator ErrorRoutine(Material errorMat)
    {
        rend.material = errorMat;

        // Fehler-Sound wird vom FinalQuestSimon abgespielt
        // Hier nicht nochmal abspielen, um Dopplung zu vermeiden

        yield return new WaitForSeconds(0.5f);
        rend.material = baseMaterial;
    }

    // Wenn Spieler einen Boden-Button drückt
    public override void Interact()
    {
        base.Interact();

        if (!isFloorButton)
            return;

        LightUp();

        // Sound für Button-Press
        if (soundManager != null)
            soundManager.PlayButtonPress();

        if (manager != null)
            manager.PlayerPressedButton(this);
    }
}