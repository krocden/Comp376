using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GunType selectedGun;
    [SerializeField] private GameObject gunHolder;
    [SerializeField] private GameObject wrench;
    [SerializeField] private GameObject currentGunUI;
    private enum GunType { Pistol, Rifle, Shotgun, Launcher, Wrench, Raygun }
    private Gun currentGun;
    private Pistol pistol;
    private Rifle rifle;
    private Shotgun shotgun;
    private Launcher launcher;
    private Raygun raygun;
    private bool holdingWrench;
    private bool hasRaygun;

    [SerializeField] private TextMeshProUGUI ammoText;
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

        raygun = gunHolder.transform.GetChild(4).GetComponent<Raygun>();
        raygun.player = this.gameObject;

        currentGun = pistol;
        selectedGun = GunType.Pistol;
        //selectedGun = GunType.Raygun;
        holdingWrench = true;
        hasRaygun = false;

        updateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStateManager.Instance.BlockInput)
            return;

        if (gameObject.GetComponent<PlayerMovement>().isDead)
            return;

        if (GameStateManager.Instance.GetCurrentGameState() == GameState.Shooting || GameStateManager.Instance.GetCurrentGameState() == GameState.Transition)
        {
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
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (hasRaygun)
                    changeGun(GunType.Raygun);
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
                    currentGun.shoot();
                    updateUI();
                }
            }
        }

        if (IsHoldingWrench)
        {
            holdingWrench = true;
            wrench.SetActive(true);
            deactivatePreviousGun();
        }
        else
        {
            if (holdingWrench)
            {
                wrench.SetActive(false);
                holdingWrench = false;
                changeGun(selectedGun);

                //Fix bugs if animation ongoing while round ends...
                currentGun.updateAnim();

                //StartCoroutine(gotRaygun(9999));          // uncomment to test raygun
            }
        }
        
    }

    public IEnumerator gotRaygun(int raygunTimer)
    {
        hasRaygun = true;
        changeGun(GunType.Raygun);
        yield return new WaitForSeconds(raygunTimer);
        changeGun(GunType.Pistol);
        hasRaygun = false;
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
        //Fix bugs if animation ongoing while round ends...
        currentGun.updateAnim();

        if (gunType == GunType.Pistol)
        {
            deactivatePreviousGun();
            currentGun = pistol;
            selectedGun = GunType.Pistol;
            gunHolder.transform.GetChild(0).gameObject.SetActive(true);

            Image img = currentGunUI.transform.GetChild(0).GetComponent<Image>();
            Color active = Color.white;
            active.a = 0.75f;
            img.color = active;
        }
        else if (gunType == GunType.Rifle)
        {
            if (upgrade.upgradeList[1].unlocked)
            {
                deactivatePreviousGun();
                currentGun = rifle;
                selectedGun = GunType.Rifle;
                gunHolder.transform.GetChild(1).gameObject.SetActive(true);

                Image img = currentGunUI.transform.GetChild(1).GetComponent<Image>();
                Color active = Color.white;
                active.a = 0.75f;
                img.color = active;
            }
        }
        else if (gunType == GunType.Shotgun)
        {
            if (upgrade.upgradeList[14].unlocked)
            {
                deactivatePreviousGun();
                currentGun = shotgun;
                selectedGun = GunType.Shotgun;
                gunHolder.transform.GetChild(2).gameObject.SetActive(true);

                Image img = currentGunUI.transform.GetChild(2).GetComponent<Image>();
                Color active = Color.white;
                active.a = 0.75f;
                img.color = active;
            }
        }
        else if (gunType == GunType.Launcher)
        {
            if (upgrade.upgradeList[27].unlocked)
            {
                deactivatePreviousGun();
                currentGun = launcher;
                selectedGun = GunType.Launcher;
                gunHolder.transform.GetChild(3).gameObject.SetActive(true);

                Image img = currentGunUI.transform.GetChild(3).GetComponent<Image>();
                Color active = Color.white;
                active.a = 0.75f;
                img.color = active;
            }
        }
        else if (gunType == GunType.Raygun)
        {
            deactivatePreviousGun();
            currentGun = raygun;
            selectedGun = GunType.Raygun;
            gunHolder.transform.GetChild(4).gameObject.SetActive(true);

            Image img = currentGunUI.transform.GetChild(4).GetComponent<Image>();
            Color active = Color.white;
            active.a = 0.75f;
            img.color = active;
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
        
        foreach (Transform gun in currentGunUI.transform)
        {
            Image img = gun.GetComponent<Image>();
            Color active = Color.black;
            active.a = 0.39215f;
            img.color = active;
        }
    }
}
