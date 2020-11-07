using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject Inventory;
    public static bool isUIEnabled;
    // Start is called before the first frame update
    void Start()
    {
        Inventory.SetActive(false);
        isUIEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Inventory.SetActive(!Inventory.activeSelf);
            if (Inventory.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                   isUIEnabled = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                   isUIEnabled = false;
            }
        }
    }
}
