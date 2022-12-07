using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public int currentAmmo;
    public int currentMaxAmmo;
    public int maxAmmo;
    public int magazineSize;

    public const int currencyPerHit = 1;
    public abstract bool shoot();
    public abstract void reload();
    public abstract void updateAnim();
    public abstract void getAmmo(out int currentAmmo, out int maxAmmo);

    public void refillAmmo()
    {
        currentMaxAmmo = maxAmmo;
        currentAmmo = magazineSize;
    }
}
