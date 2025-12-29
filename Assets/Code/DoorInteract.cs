using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    public SimonPuzzleManager puzzleManager;
    private bool started = false;

    public void Interact()
    {
        if (started)
            return;

        started = true;
        puzzleManager.StartPuzzle();
    }
}
