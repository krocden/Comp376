using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private Image[] handhelds;
    private int currentHandheld = 0;

    public bool IsHoldingWrench => currentHandheld > 0;
    public bool IsDamageWrench => currentHandheld == 1;
    public bool IsSupportWrench => currentHandheld == 2;
    public bool IsDefenseWrench => currentHandheld == 3;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Image handheld in handhelds)
            handheld.enabled = false;

        handhelds[currentHandheld].enabled = true;
    }

    void FixedUpdate()
    {
        Ray bullet = new Ray(transform.position, Camera.main.transform.forward);
        RaycastHit bulletHit;

        if (Input.GetButton("Fire1"))
        {
            Debug.DrawRay(transform.position, Camera.main.transform.forward * 1000, Color.white, 1);
            if (Physics.Raycast(bullet, out bulletHit))
            {
                Debug.Log("Hit: " + bulletHit.collider.tag);
                if (bulletHit.collider.tag == "Enemy")
                {
                    GameObject hitObject = bulletHit.transform.gameObject;
                    Monster enemy = hitObject.GetComponent<Monster>();
                    enemy.health -= damage;
                    Debug.Log("Health: " + enemy.health);
                }
            }
        }
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.DrawRay(transform.position, Camera.main.transform.forward * 1000, Color.black, 1);
            Debug.DrawRay(transform.position + new Vector3(1, 0, 0), Camera.main.transform.forward * 1000, Color.black, 1);
            Debug.DrawRay(transform.position + new Vector3(0, 1, 0), Camera.main.transform.forward * 1000, Color.black, 1);
            Debug.DrawRay(transform.position + new Vector3(1, 1, 0), Camera.main.transform.forward * 1000, Color.black, 1);
            if (Physics.Raycast(bullet, out bulletHit))
            {
                //Debug.Log(bulletHit);
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (currentHandheld == handhelds.Length - 1)
                currentHandheld = 0;
            else
                currentHandheld++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (currentHandheld == 0)
                currentHandheld = handhelds.Length - 1;
            else
                currentHandheld--;
        }

        foreach (Image handheld in handhelds)
            handheld.enabled = false;

        handhelds[currentHandheld].enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
