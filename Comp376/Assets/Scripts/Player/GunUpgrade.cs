using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunUpgrade : MonoBehaviour
{
    public static GunUpgrade gunUpgrade;

    public List<Upgrade> upgradeList;
    public GameObject upgradeHolder;

    public Rifle rifle;
    public Shotgun shotgun;
    public Launcher launcher;

    private void Awake()
    {
        gunUpgrade = this;
    }

    private void Start()
    {
        initializeUpgrades();
    }

    void initializeUpgrades()
    {
        foreach (Upgrade upgrade in upgradeHolder.transform.GetChild(0).GetComponentsInChildren<Upgrade>(true))
        {
            upgradeList.Add(upgrade);
        }
    }

    public void updateUpgrades(int id, Upgrade upgrade)
    {
        upgradeList.RemoveAt(id);
        upgradeList.Insert(id, upgrade);

        updateGunStat(id);
    }

    void updateGunStat(int id)
    {
        switch (id)
        {
            case 2: rifle.damage *= 1.15f; break;    // Rifle id 2-13
            case 3: rifle.damage *= 1.20f; break;
            case 4: rifle.damage *= 1.25f; break;
            case 5: rifle.damage *= 1.30f; break;
            case 6: rifle.criticalChance = 0.025f; break;
            case 7: rifle.criticalChance = 0.05f; break;
            case 8: rifle.criticalChance = 0.075f; break;
            case 9: rifle.criticalChance = 0.10f; break;
            case 10: case 11: case 12: case 13: rifle.magazineSize += 4; rifle.maxAmmo += 40; break;
            case 15: shotgun.damage *= 1.15f; break;    // Shotgun id 15-26
            case 16: shotgun.damage *= 1.20f; break;
            case 17: shotgun.damage *= 1.25f; break;
            case 18: shotgun.damage *= 1.30f; break;
            case 19: shotgun.fireRate = 0.64f; break;
            case 20: shotgun.fireRate = 0.56f; break;
            case 21: shotgun.fireRate = 0.48f; break;
            case 22: shotgun.fireRate = 0.40f; break;
            case 23: case 24: case 25: case 26: shotgun.magazineSize += 1; shotgun.maxAmmo += 12; break;
            case 28: launcher.explosionRadius = 4f; break;    // launcher id 28-39
            case 29: launcher.explosionRadius = 5.5f; break;
            case 30: launcher.explosionRadius = 7f; break;
            case 31: launcher.explosionRadius = 8.5f; break;
            case 32: launcher.damage *= 1.15f; break;
            case 33: launcher.damage *= 1.20f; break;
            case 34: launcher.damage *= 1.25f; break;
            case 35: launcher.damage *= 1.3f; break;
            case 36: case 37: case 38: case 39: launcher.magazineSize += 1; launcher.maxAmmo += 8; break;
            default:
                break;
        }
    }
}
