using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    public PlayerInput input;
    private InputAction moveAction;

    public CharacterController controller;
    public float speed = 5f;

    public Transform referenceCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = input.actions.FindAction("Move");
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

        if (inputDirection != Vector2.zero)
        {
            //transform (klein) verweist auf "mein" Transform
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, 0.1f);
        }
    }
}
