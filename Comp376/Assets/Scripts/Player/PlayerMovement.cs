using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float gravity;
    
    private CharacterController controller;
    private Camera firstPersonCamera;


    public bool grounded;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        firstPersonCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = controller.isGrounded;
        
        if (!controller.isGrounded)
        {
            Vector3 gravitationalForce = new Vector3(0, gravity * Time.deltaTime, 0);
            controller.Move(gravitationalForce);
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 forward = firstPersonCamera.transform.forward;
        Vector3 right = firstPersonCamera.transform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 movement = forward * z + right * x;

        controller.Move(movement * Time.deltaTime * speed);

    }
}
