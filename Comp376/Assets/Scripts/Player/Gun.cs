using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public const int currencyPerHit = 1;
    public abstract bool shoot();
    public abstract void reload();
    public abstract void getAmmo(out int currentAmmo, out int maxAmmo);
}
