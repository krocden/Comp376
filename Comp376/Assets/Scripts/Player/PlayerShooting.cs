using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GunType selectedGun;
    [SerializeField] private GameObject gunHolder;
    [SerializeField] private GameObject wrench;
    private enum GunType { Pistol, Rifle, Shotgun, Launcher, Wrench }
    private Gun currentGun;
    private Pistol pistol;
    private Rifle rifle;
    private Shotgun shotgun;
    private Launcher launcher;

    [SerializeField] private Text ammoText;
    [SerializeField] GunUpgrade upgrade;

    //private bool isHoldingWrench = GameStateManager.Instance.GetCurrentGameState() == GameState.Building;

    public bool IsHoldingWrench => GameStateManager.Instance.GetCurrentGameState() == GameState.Planning;

    // Start is called before the first frame update
    void Start()
    {
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
        if (GameStateManager.Instance.BlockInput)
            return;
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

        if (IsHoldingWrench)
        {
            wrench.SetActive(true);
            deactivatePreviousGun();
        } 
        else
        {
            wrench.SetActive(false);
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
            if (upgrade.upgradeList[14].unlocked)
            {
                deactivatePreviousGun();
                currentGun = shotgun;
                gunHolder.transform.GetChild(2).gameObject.SetActive(true);
            }
        }
        else if (gunType == GunType.Launcher)
        {
            if (upgrade.upgradeList[27].unlocked)
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
