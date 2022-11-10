using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GunType selectedGun;
    [SerializeField] private GameObject gunHolder;
    private enum GunType { Pistol, Rifle, Shotgun, Launcher }
    private Gun currentGun;
    private Pistol pistol;
    private Rifle rifle;
    private Shotgun shotgun;
    private Launcher launcher;

    [SerializeField] private Text ammoText;
    [SerializeField] GunUpgrade upgrade;

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

        pistol = gunHolder.transform.GetChild(0).GetComponent<Pistol>();
        pistol.player = this.gameObject;

        rifle = gunHolder.transform.GetChild(1).GetComponent<Rifle>();
        rifle.player = this.gameObject;

        
        shotgun = gunHolder.transform.GetChild(2).GetComponent<Shotgun>();
        shotgun.player = this.gameObject;

        launcher = gunHolder.transform.GetChild(3).GetComponent<Launcher>();
        launcher.player = this.gameObject;
        
        currentGun = pistol;
        updateUI();
    }

    // Update is called once per frame
    void Update()
    {
        //Call this when changing gun

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            changeGun(GunType.Pistol);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            changeGun(GunType.Rifle);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            changeGun(GunType.Shotgun);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            changeGun(GunType.Launcher);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (currentHandheld == handhelds.Length - 1)
                currentHandheld = 0;
            else
                currentHandheld++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (currentHandheld == 0)
                currentHandheld = handhelds.Length - 1;
            else
                currentHandheld--;
        }

        foreach (Image handheld in handhelds)
            handheld.enabled = false;

        handhelds[currentHandheld].enabled = true;

        if (!this.gameObject.GetComponentInChildren<CameraHandler>().usingMenu)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                currentGun.reload();
                updateUI();
            }
            
            if (!IsHoldingWrench)
            {
                if (currentGun.shoot())
                {
                    updateUI();
                }
            }
            
        }
        
    }

    void updateUI()
    {
        int currAmmo;
        int maxAmmo;
        currentGun.getAmmo(out currAmmo, out maxAmmo);
        ammoText.text = string.Format("{0} / {1}", currAmmo, maxAmmo);
    }

    void changeGun(GunType gunType)
    {
        if (gunType == GunType.Pistol)
        {
            deactivatePreviousGun();
            currentGun = pistol;
            gunHolder.transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (gunType == GunType.Rifle)
        {
            if (upgrade.upgradeList[1].unlocked)
            {
                deactivatePreviousGun();
                currentGun = rifle;
                gunHolder.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
        else if (gunType == GunType.Shotgun)
        {
            if (upgrade.upgradeList[2].unlocked)
            {
                deactivatePreviousGun();
                currentGun = shotgun;
                gunHolder.transform.GetChild(2).gameObject.SetActive(true);
            }
        }
        else if (gunType == GunType.Launcher)
        {
            if (upgrade.upgradeList[3].unlocked)
            {
                deactivatePreviousGun();
                currentGun = launcher;
                gunHolder.transform.GetChild(3).gameObject.SetActive(true);
            }
        }

        updateUI();
    }

    void deactivatePreviousGun()
    {
        foreach (Transform gun in gunHolder.transform)
        {
            if (gun.gameObject.activeInHierarchy)
            {
                gun.gameObject.SetActive(false);
            }
        }
    }
}
