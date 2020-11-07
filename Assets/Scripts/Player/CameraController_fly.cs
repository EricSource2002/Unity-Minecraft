using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController_fly : MonoBehaviour
{
    public GameObject centerPoint;
    public float rotSpeed = 150f;
    public float speed = 100f;
    public BlockType[,,] blocks = new BlockType[16 + 2, 256, 16 + 2];
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            centerPoint.transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            centerPoint.transform.Translate(Vector3.back * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            centerPoint.transform.Translate(Vector3.right * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            centerPoint.transform.Translate(Vector3.left * speed * Time.deltaTime);
        }



        if (Input.GetKey(KeyCode.A))
        {
            centerPoint.transform.Rotate(Vector3.up * rotSpeed, Space.World);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            centerPoint.transform.Rotate(Vector3.down * rotSpeed, Space.World);
        }
    }
}
