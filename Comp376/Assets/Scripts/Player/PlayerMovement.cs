using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float speed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float verticalVelocity;
    
    private Vector3 movement;
    
    private CharacterController controller;
    private Camera firstPersonCamera;

    public bool grounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        firstPersonCamera = Camera.main;
        speed = walkSpeed;
    }

    void Update()
    {
        if (GameStateManager.Instance.BlockInput)
            return;

        grounded = controller.isGrounded;

        if (Input.GetKeyDown(KeyCode.LeftShift) && grounded)
        {
            speed = runningSpeed;
        } 
        if (Input.GetKeyUp(KeyCode.LeftShift)) 
        {
            if (grounded)
            {
                speed = walkSpeed;
            } 
            else 
            {
                StartCoroutine(landing());
            }
        }

        if (grounded)
        {
            verticalVelocity = 0;
        }

        verticalVelocity += gravity * Time.deltaTime;

        if (Input.GetButton("Jump"))
        {
            if (grounded)
            {
                verticalVelocity += Mathf.Sqrt(jumpHeight * -2 * gravity);
            }
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 forward = firstPersonCamera.transform.forward;
        Vector3 right = firstPersonCamera.transform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        movement = forward * z + right * x;
        
        movement.y = verticalVelocity;
        controller.Move(movement * Time.deltaTime * speed);
    }

    void handleInput()
    {
        //Move code here
    }

    IEnumerator landing()
    {
        yield return new WaitUntil(() => grounded);
        speed = walkSpeed;
    }

}
