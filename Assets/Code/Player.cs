using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;

public class Player : MonoBehaviour
{
    public PlayerInput input;
    private InputAction moveAction;
    private InputAction interactAction;
    public CharacterController controller;
    public Animator animator;
    public float speed = 5f;
    public Transform referenceCamera;
    public Interactable interactable;
    public InteractHintUI interactHint;

    [Header("FMOD Audio")]
    public EventReference pickupEvent;

    void Start()
    {
        moveAction = input.actions.FindAction("Move");
        interactAction = input.actions.FindAction("Interact");
        interactAction.performed += InteractAction_performed;
    }

    private void InteractAction_performed(InputAction.CallbackContext obj)
    {
        if (interactable != null)
        {
            interactable.Interact();

            // FMOD Pickup Sound NUR für Items
            Item item = interactable.GetComponent<Item>();
            if (item != null && !pickupEvent.IsNull)
            {
                RuntimeManager.PlayOneShot(pickupEvent, transform.position);
            }

            // Hint ausblenden NUR wenn es ein Item war (wird zerstört)
            // NPCs bleiben, also Hint bleibt auch
            if (item != null)
            {
                if (interactHint != null)
                    interactHint.Hide();
                interactable = null;
            }
            // Wenn es ein NPC ist, bleibt interactable gesetzt!
        }
    }

    void Update()
    {
        Vector2 inputDirection = moveAction.ReadValue<Vector2>();
        Vector3 forward = referenceCamera.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 right = referenceCamera.right;
        Vector3 moveDirection = forward * inputDirection.y + right * inputDirection.x;
        moveDirection.y = 0f;
        moveDirection.Normalize();
        controller.Move(moveDirection * Time.deltaTime * speed);

        if (!controller.isGrounded)
            controller.Move(Vector3.down);

        if (inputDirection != Vector2.zero)
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, 0.1f);

        animator.SetFloat("speed", moveDirection.magnitude * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (interactHint == null) return;

        Interactable inter = other.GetComponent<Interactable>();
        if (inter != null)
        {
            interactable = inter;
            interactHint.Show();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (interactHint == null) return;

        Interactable inter = other.GetComponent<Interactable>();
        if (inter != null && inter == interactable)
        {
            interactable = null;
            interactHint.Hide();
        }
    }
}