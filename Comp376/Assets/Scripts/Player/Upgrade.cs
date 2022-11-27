using UnityEngine;
using UnityEngine.UI;
using static GunUpgrade;

public class Upgrade : MonoBehaviour
{
    public int id;
    public bool unlocked;
    public int cost;
    public int upgradePreRequisite;

    public void buy()
    {
        bool valid = gunUpgrade.upgradeList[upgradePreRequisite].unlocked;

        if (valid && !unlocked && gunUpgrade.upgradePoints >= cost)
        {
            gunUpgrade.upgradePoints -= cost;
            unlocked = true;

            gunUpgrade.updateUpgrades(id, this);
            updateUI();
        }
    }
    
    void updateUI()
    {
        Image img = GetComponent<Image>();
        img.color = Color.green;
    }
}
