using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static WallAutomata.TurretState;

public class Turret : MonoBehaviour
{
    [SerializeField] private WallAutomata.TurretState _currentState;
    private MeshRenderer rend;
    public float rateOfFire = 1f;
    public int damage = 10;

    [SerializeField] Material[] turretMaterials;
    [SerializeField] TurretTriggerArea turretArea;
    [SerializeField] GameObject turretGunBulletPrefab;

    void Start()
    {
        rend = GetComponent<MeshRenderer>();
    }

    public void SetMode(WallAutomata.TurretState state)
    {
        _currentState = state;

        rend.enabled = true;
        CancelInvoke();

        switch (state)
        {
            case EmptyTurret:
                rend.enabled = false;
                break;
            case GunTurret:
                rend.material = turretMaterials[0];
                InvokeRepeating(nameof(ShootGunTurret), 1f, rateOfFire);
                break;
            case CannonTurret:
                break;
            case PortalTurret:
                break;
            case BuffTurret:
                break;
            case WallTurret:
                break;
            case SlowTurret:
                break;
            default:
                break;
        }
    }

    public static string GetTurretText(WallAutomata.TurretState state) {
        switch (state)
        {
            case EmptyTurret:
                return "Empty turret";
            case GunTurret:
                return "Gun turret";
            case CannonTurret:
                return "Cannon turret";
            case PortalTurret:
                return "Portal turret";
            case BuffTurret:
                return "Buff turret";
            case WallTurret:
                return "Wall turret";
            case SlowTurret:
                return "Slow turret";
            default:
                return string.Empty;
        }
    }

    private void ShootGunTurret()
    {
        if (turretArea.monstersInArea.Count == 0) return;

        Monster monster = turretArea.monstersInArea[0];
        while (monster == null && turretArea.monstersInArea.Count > 1)
        {
            turretArea.monstersInArea.RemoveAt(0);
            monster = turretArea.monstersInArea[0];
        }
        if (monster == null) return;

        GunTurretBullet bullet = Instantiate(turretGunBulletPrefab, transform.position, Quaternion.identity).GetComponent<GunTurretBullet>();
        bullet.target = monster;
        bullet.turret = this;
    }

    public void GunTurretMonsterHit(Monster monster)
    {
        if (monster == null) return;

        if (monster.TakeDamage(damage))
            turretArea.monstersInArea.Remove(monster);
    }
}
