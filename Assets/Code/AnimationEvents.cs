using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AnimationEvents : MonoBehaviour 
{

    private EventInstance footstepInstance;

    public EventReference footstepReference;
    public LayerMask groundMaterialLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //wir sagen FMOD, dass wir später dieses Event abspielen wollen
        footstepInstance = RuntimeManager.CreateInstance(footstepReference);
    }

    public void Footstep()
    {
        if(Physics.Raycast(transform.position + new Vector3(0, 2, 0), Vector3.down, out RaycastHit hitinfo, 2, groundMaterialLayer))
        {
            footstepInstance.setParameterByNameWithLabel("NameVomParameterinFMOD", hitinfo.transform.tag);
        }
        else
        {
            footstepInstance.setParameterByNameWithLabel("NameVomParameterinFMOD", "BasicFootstepswennnichtsanderesdaist");
        }


            footstepInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        footstepInstance.start();
    }

}
