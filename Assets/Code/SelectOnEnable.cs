using UnityEngine;
using UnityEngine.EventSystems;

public class SelectOnEnable : MonoBehaviour
{
    public GameObject objectToSelect;

    //wenn dieses GameObject aktiviert wird, wird das angegebene Objekt ausgewählt
    public void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(objectToSelect);
    }
}
