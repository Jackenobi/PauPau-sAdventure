using System.Collections;
using UnityEngine;

public class PuzzleButton : MonoBehaviour
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
        audioSource.Play();
        yield return new WaitForSeconds(0.4f);
        rend.material = baseMaterial;
    }

    private void OnMouseDown()
    {
        if (!isFloorButton)
            return;

        LightUp();
        manager.PlayerPressedButton(this);
    }
}
