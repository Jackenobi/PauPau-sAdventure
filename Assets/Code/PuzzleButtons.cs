using System.Collections;
using UnityEngine;

public class PuzzleButton : Interactable
{
    public int buttonIndex;
    public bool isFloorButton;
    public Material baseMaterial;
    public Material glowMaterial;
    public AudioSource audioSource;

    private MeshRenderer rend;
    private SimonPuzzleManager manager;

    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        manager = FindObjectOfType<SimonPuzzleManager>();
        rend.material = baseMaterial;
    }

    public void LightUp()
    {
        StartCoroutine(LightRoutine());
    }

    IEnumerator LightRoutine()
    {
        rend.material = glowMaterial;
        if (audioSource != null)
            audioSource.Play();
        yield return new WaitForSeconds(0.5f);
        rend.material = baseMaterial;
    }

    //FehlerAnzeige
    public void ShowError(Material errorMat, AudioClip errorClip)
    {
        StartCoroutine(ErrorRoutine(errorMat, errorClip));
    }

    IEnumerator ErrorRoutine(Material errorMat, AudioClip errorClip)
    {
        rend.material = errorMat;

        if (audioSource != null && errorClip != null)
            audioSource.PlayOneShot(errorClip);

        yield return new WaitForSeconds(0.5f);
        rend.material = baseMaterial;
    }

    public override void Interact()
    {
        base.Interact();

        if (!isFloorButton)
            return;

        LightUp();

        if (manager != null)
            manager.PlayerPressedButton(this);
    }
}