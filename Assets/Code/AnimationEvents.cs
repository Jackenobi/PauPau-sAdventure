using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    private EventInstance footstepInstance;
    public EventReference footstepReference;
    public LayerMask groundMaterialLayer;

    void Start()
    {
        // Wir sagen FMOD, dass wir sp‰ter dieses Event abspielen wollen
        footstepInstance = RuntimeManager.CreateInstance(footstepReference);
    }

    public void Footstep()
    {
        if (Physics.Raycast(transform.position + new Vector3(0, 2, 0), Vector3.down, out RaycastHit hitinfo, 2, groundMaterialLayer))
        {
            // Parameter-Name aus FMOD: "Footstep_ground"
            // Wert: Tag vom GameObject (z.B. "Straﬂe", "Forest", "Sand")
            footstepInstance.setParameterByNameWithLabel("Footstep_ground", hitinfo.transform.tag);
        }
        else
        {
            // Fallback wenn kein Boden getroffen wird
            footstepInstance.setParameterByNameWithLabel("Footstep_ground", "Straﬂe");
        }

        footstepInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        footstepInstance.start();
    }
}