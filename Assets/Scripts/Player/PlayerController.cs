using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    public float speed = 3f;
    public float runSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -20f;

    public Transform groundCheck;
    private float groundRadius = .3f;
    public LayerMask groundMask;
    public bool isGrounded;
    private Vector3 velocity;

    public Text positionText;

    public Text chunkText;

    void Start()
    {
        characterController = this.gameObject.transform.GetComponent<CharacterController>();
    }

    void Update()
    {
        positionText.text = this.transform.position.ToString();
        chunkText.text = (Mathf.FloorToInt(this.transform.position.x / 16f) * 16).ToString() + " " + (Mathf.FloorToInt(this.transform.position.z / 16f) * 16).ToString();
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3();
        if (!UIController.isUIEnabled)
        {
            moveDirection = transform.right * horizontal + transform.forward * vertical;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            characterController.Move(moveDirection * runSpeed * Time.deltaTime);
        }
        else
        {
            characterController.Move(moveDirection * speed * Time.deltaTime);
        }
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}

