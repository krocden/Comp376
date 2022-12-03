using UnityEngine;
using UnityEngine.UI;
using static GunUpgrade;
using TMPro;
using System.Collections.Generic;

public class Upgrade : MonoBehaviour
{
    public int id;
    public bool unlocked;
    public int cost;
    public int upgradePreRequisite;
    public GameObject tooltip;
    TextMeshProUGUI tooltipText;
    readonly List<int> twoLinkId = new List<int>() { 1, 6, 10, 19, 23, 28, 36 };

    private void Start()
    {
        tooltipText = tooltip.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        tooltipText.text = "Cost: " + cost.ToString() + "$";
    }

    public void buy()
    {
        bool valid = gunUpgrade.upgradeList[upgradePreRequisite].unlocked;

        if (valid && !unlocked && CurrencyManager.Instance.Currency >= cost)
        {
            bool validBuy = CurrencyManager.Instance.SubtractCurrency(cost);
            if (validBuy)
            {
                unlocked = true;

                gunUpgrade.updateUpgrades(id, this);
                updateUI();
            }
        }
    }
    
    void updateUI()
    {
        Image img = GetComponent<Image>();
        img.color = Color.green;

        if (upgradePreRequisite != -1)
        {
            Image link = transform.GetChild(3).GetComponent<Image>();
            link.color = Color.green;

            if (twoLinkId.Contains(id))
            {
                Image link2 = transform.GetChild(4).GetComponent<Image>();
                link2.color = Color.green;
            }
        }
    }

    public void enterHover()
    {
        tooltip.SetActive(true);
    }
    public void exitHover()
    {
        tooltip.SetActive(false);
    }
}
