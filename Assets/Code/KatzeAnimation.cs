using UnityEngine;

public class NPCAnimationController : MonoBehaviour
{
    public Animator animator;
    public string idleParameter = "isIdle"; // Parameter im Animator
    public Quest questManager;

    void Update()
    {
        // Prüfe ob Nachbarn-Quest abgeschlossen
        if (questManager != null && questManager.IsNachbarnQuestDone())
        {
            animator.SetBool(idleParameter, true);
        }
        else
        {
            animator.SetBool(idleParameter, false);
        }
    }
}