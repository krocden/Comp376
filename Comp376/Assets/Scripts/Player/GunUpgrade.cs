using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunUpgrade : MonoBehaviour
{
    public static GunUpgrade gunUpgrade;

    public string[] upgradeName;
    public string[] upgradeDescription;
    public int upgradePoints;

    public List<Upgrade> upgradeList;
    public GameObject upgradeHolder;

    private void Awake()
    {
        gunUpgrade = this;
    }

    private void Start()
    {
        upgradePoints = 33;

        upgradeName = new[] { "Pistol", "Assault Rifle", "Shotgun", "Launcher" };
        upgradeDescription = new[]
        {
            "Pistol",
            " 1",
            " 2",
            " 3"
        };

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
    }
}
