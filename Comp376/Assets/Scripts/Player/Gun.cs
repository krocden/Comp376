using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Gun
{
    /*
    float damage { get; }
    float magazineSize { get; }
    float magazineCount { get; }
    float fireRate { get; } 
    float range { get; }
    bool automatic { get; }
    */
    bool shoot();
    void reload();
    void getAmmo(out int currentAmmo, out int maxAmmo);
}
