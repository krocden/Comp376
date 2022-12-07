using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class wallGun : MonoBehaviour
{
    public Gun selectedGun;
    //SpriteRenderer spriteRenderer;
    public TextMeshProUGUI text;
    public int cost;
    public GameObject player;
    public float buyDistance;

    void Start()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        text.text = "Press F to refill ammo [Cost: " + cost.ToString() + "$]";
    }

    private void OnMouseOver()
    {
        if (GameStateManager.Instance.GetCurrentGameState() == GameState.Shooting || GameStateManager.Instance.GetCurrentGameState() == GameState.Transition)
        {
            if (Vector3.Distance(player.transform.position, transform.position) <= buyDistance)
            {
                text.gameObject.SetActive(true);

                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (CurrencyManager.Instance.SubtractCurrency(cost))
                    {
                        selectedGun.refillAmmo();
                    }
                }
            }
            else
            {
                text.gameObject.SetActive(false);
            }
        } 
        else 
        {
            text.gameObject.SetActive(false);
        }
        
    }

    private void OnMouseExit()
    {
        text.gameObject.SetActive(false);
    }
}
