using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    private float xRot;

void Start()  {
    Cursor.lockState = CursorLockMode.Locked;
}  void Update()
    {
        if(UIController.isUIEnabled) return;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity ;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity ;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);
        this.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX );

    }
}
