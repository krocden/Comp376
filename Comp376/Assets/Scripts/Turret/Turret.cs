using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static WallAutomata.TurretState;

public class Turret : MonoBehaviour
{
    public static Turret[] activePortals = new Turret[2];
    public static Turret lastUsedPortal;
    public static bool isTeleportDisabled = false;

    [SerializeField] private WallAutomata.TurretState _currentState;
    private MeshRenderer rend;
    public float rateOfFire = 1f;
    public int damage = 2;

    [SerializeField] Material[] turretMaterials;
    [SerializeField] TurretTriggerArea turretArea;
    [SerializeField] GameObject turretGunBulletPrefab;
    [SerializeField] ParticleSystem cannonGunExplosion;

    void Start()
    {
        rend = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        turretArea.TeleportIn.AddListener(TeleportIn);
        turretArea.TeleportOut.AddListener(TeleportOut);
    }

    private void OnDisable()
    {
        turretArea.TeleportIn.RemoveListener(TeleportIn);
        turretArea.TeleportOut.RemoveListener(TeleportOut);
    }

    public void SetMode(WallAutomata.TurretState state)
    {
        _currentState = state;

        for (int i = 0; i < activePortals.Length; i++)
            if (activePortals[i] == this)
                activePortals[i] = null;

        rend.enabled = true;
        CancelInvoke();

        turretArea.state = _currentState;

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
                rend.material = turretMaterials[0];
                InvokeRepeating(nameof(ShootCannonTurret), 1f, rateOfFire);
                break;
            case PortalTurret:
                if (activePortals[0] && activePortals[1])
                {
                    SetMode(EmptyTurret);
                    break;
                }
                else if (!activePortals[0])
                    activePortals[0] = this;
                else
                    activePortals[1] = this;
                rend.material = turretMaterials[0];
                break;
            case BuffTurret:
                //Logic in turret trigger area (onenter)
                rend.material = turretMaterials[0];
                break;
            case WallTurret:
                break;
            case SlowTurret:
                //Logic in turret trigger area (onenter)
                rend.material = turretMaterials[0];
                break;
            default:
                break;
        }
    }

    public static string GetTurretText(WallAutomata.TurretState state)
    {
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

    private void ShootCannonTurret()
    {
        if (turretArea.monstersInArea.Count == 0) return;

        cannonGunExplosion.Play();
        int maxCount = turretArea.monstersInArea.Count;

        for (int i = 0; i < maxCount; i++)
        {
            if (i > turretArea.monstersInArea.Count - 1) return;

            if (turretArea.monstersInArea[i] == null)
                turretArea.monstersInArea.RemoveAt(i);
            else
            if (turretArea.monstersInArea[i].TakeDamage(5))
                turretArea.monstersInArea.RemoveAt(i);
        }
    }

    private void TeleportIn()
    {
        if (!activePortals[0] || !activePortals[1] || isTeleportDisabled) return;

        lastUsedPortal = this;
        isTeleportDisabled = true;

        PlayerMovement player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        Debug.Log($"Last Position: {player.transform.position}");


        if (this == activePortals[0])
            player.SetPosition(activePortals[1].transform.position);
        else
            player.SetPosition(activePortals[0].transform.position);

        Debug.Log($"New Position: {player.transform.position}");
    }

    private void TeleportOut()
    {
        if (lastUsedPortal == this) return;
        isTeleportDisabled = false;
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

        if (monster.TakeDamage(2))
            turretArea.monstersInArea.Remove(monster);
    }
}
