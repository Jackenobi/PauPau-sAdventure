using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Variabeln
    public PlayerInput input;
    private InputAction moveAction;
    private InputAction interactAction;

    public CharacterController controller;
    public float speed = 5f;

    public Transform referenceCamera;

    public Interactable interactable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = input.actions.FindAction("Move");
        interactAction = input.actions.FindAction("Interact");

        interactAction.performed += InteractAction_performed;
    }

    private void InteractAction_performed(InputAction.CallbackContext obj)
    {
        if(interactable != null)
        {
            interactable.Interact();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputDirection = moveAction.ReadValue<Vector2>();

        Vector3 forward = referenceCamera.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 right = referenceCamera.right;

        //wie viel auf forward + wie viel auf right
        Vector3 moveDirection = forward * inputDirection.y + right * inputDirection.x;
        moveDirection.y = 0f; // keine höhenveränderung
        moveDirection.Normalize();

        controller.Move(moveDirection * Time.deltaTime * speed);
        if (!controller.isGrounded)
            controller.Move(new Vector3(0, -1, 0)); // gravity

        if (inputDirection != Vector2.zero)
        {
            //transform (klein) verweist auf "mein" Transform
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, 0.1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Interactable inter = other.GetComponent<Interactable>(); //sucht nach Interactable Script auf "other" Object

        //wenn ein "Interactable" gefunden wird, ist die Variabel nicht null
        if(inter != null)
        interactable = inter;
    }

    private void OnTriggerExit(Collider other)
    {
     Interactable inter = other.GetComponent<Interactable>();

        if (inter != null)
            interactable = null;
    }

}
