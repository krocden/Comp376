using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] float sensitivity;
    Vector2 rotation = new Vector2(0, 0);

    [SerializeField] GameObject upgradeTree;
    public bool usingMenu;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //upgradeTree = GameObject.Find("Gun Upgrades Canvas").transform.GetChild(0).gameObject;
        upgradeTree.SetActive(false);
        usingMenu = false;
    }

    void Update()
    {
        if (GameStateManager.Instance.BlockInput)
        { 
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        if (Input.GetKey(KeyCode.N))
        {
            //Time.timeScale = 0f; Prevent movement while using menu ?
            Cursor.lockState = CursorLockMode.None;
            upgradeTree.SetActive(true);
            usingMenu = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            upgradeTree.SetActive(false);
            usingMenu = false;
        }

        if (!usingMenu)
        {
            rotation.x += Input.GetAxis("Mouse X") * sensitivity;
            rotation.y -= Input.GetAxis("Mouse Y") * sensitivity;
            rotation.y = Mathf.Clamp(rotation.y, -89, 89);

            Quaternion localRotation = Quaternion.AngleAxis(rotation.x, Vector3.up) * Quaternion.AngleAxis(rotation.y, Vector3.right);
            transform.rotation = localRotation;

            Vector3 eulerRotation = localRotation.eulerAngles;
            eulerRotation.x = 0;
            transform.parent.rotation = Quaternion.Euler(eulerRotation);
        }
    }
}
