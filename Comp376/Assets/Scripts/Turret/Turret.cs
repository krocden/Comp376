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
    int level = 1;
    private int ticks;

    [SerializeField] Material[] turretMaterials;
    [SerializeField] TurretTriggerArea turretArea;
    [SerializeField] GameObject turretGunBulletPrefab;
    [SerializeField] ParticleSystem cannonGunExplosion;
    [SerializeField] private SpriteRenderer PortalVisual;

    [SerializeField] private SpriteRenderer minimapTurretVisual;

    [SerializeField] private AudioClip gunTurretSFX;
    [SerializeField] private AudioClip cannonTurretSFX;
    [SerializeField] private AudioClip portalTurretSFX;

    private List<Color> turretColors = new List<Color>()
    {
        Color.red,
        Color.yellow,
        Color.magenta,
        Color.green,
        Color.blue,
    };

    void Start()
    {
        rend = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        turretArea.TeleportIn.AddListener(TeleportIn);
        turretArea.TeleportOut.AddListener(TeleportOut);
        GameStateManager.Instance.tick += HandleTick;
    }

    private void OnDisable()
    {
        turretArea.TeleportIn.RemoveListener(TeleportIn);
        turretArea.TeleportOut.RemoveListener(TeleportOut);
        GameStateManager.Instance.tick -= HandleTick;
    }

    public void HandleTick()
    {
        switch (_currentState)
        {
            case GunTurret:
                ticks++;
                if (ticks == rateOfFire)
                {
                    ShootGunTurret();
                    ticks = 0;
                }
                return;
            case CannonTurret:
                ticks++;
                if (ticks == rateOfFire)
                {
                    ShootCannonTurret();
                    ticks = 0;
                }
                return;
            default:
                return;
        }
    }

    public void SetLevel(int level)
    {
        this.level = level;

        switch (_currentState)
        {
            case GunTurret:
                if (level == 1) SetStats(2, 1, 4f, 4);
                if (level == 2) SetStats(4, 1, 2f, 4);
                if (level == 3) SetStats(6, 1, 1f, 4);
                break;
            case CannonTurret:
                if (level == 1) SetStats(1, 1, 8f, 4);
                if (level == 2) SetStats(3, 3, 8f, 5);
                if (level == 3) SetStats(3, 3, 4f, 6);
                break;
            case BuffTurret:
                if (level == 1) SetStats(1, 1, 1.5f, 0);
                if (level == 2) SetStats(3, 3, 1.75f, 0);
                if (level == 3) SetStats(5, 5, 2f, 0);
                break;
            case SlowTurret:
                if (level == 1) SetStats(1, 1, 0.80f, 0);
                if (level == 2) SetStats(3, 3, 0.65f, 0);
                if (level == 3) SetStats(5, 5, 0.50f, 0);
                break;
            case PortalTurret:
                SetStats(0.1f, 1, 0, 0);
                break;
            default:
                break;
        }
    }

    private void SetStats(float rangeZ, int rangeX, float rateOfFire, int power)
    {
        switch (_currentState)
        {
            case GunTurret:
                this.rateOfFire = rateOfFire;
                this.damage = power;
                this.ticks = 0;
                break;
            case CannonTurret:
                this.rateOfFire = rateOfFire;
                this.damage = power;
                this.ticks = 0;
                break;
            case BuffTurret:
            case SlowTurret:
                turretArea.modifier = rateOfFire;
                break;
        }
        turretArea.SetArea(rangeZ, rangeX);
    }

    public void SetMode(WallAutomata.TurretState state)
    {
        _currentState = state;

        for (int i = 0; i < activePortals.Length; i++)
            if (activePortals[i] == this)
                activePortals[i] = null;

        rend.enabled = true;
        PortalVisual.enabled = false;
        this.ticks = 0;

        turretArea.state = _currentState;
        if (!activePortals[0] || !activePortals[1])
        {
            if (activePortals[0])
                activePortals[0].turretArea.GetComponent<MeshRenderer>().enabled = false;
            if (activePortals[1])
                activePortals[1].turretArea.GetComponent<MeshRenderer>().enabled = false;
        }
        turretArea.GetComponent<MeshRenderer>().enabled = false;

        minimapTurretVisual.gameObject.SetActive(true);
        switch (state)
        {
            case EmptyTurret:
                minimapTurretVisual.gameObject.SetActive(false);
                rend.enabled = false;
                break;
            case GunTurret:
                rend.material = turretMaterials[0];
                minimapTurretVisual.color = turretColors[(int)state - 1];
                break;
            case CannonTurret:
                rend.material = turretMaterials[1];
                minimapTurretVisual.color = turretColors[(int)state - 1];
                break;
            case PortalTurret:
                if (activePortals[0] && activePortals[1])
                {
                    SetMode(EmptyTurret);
                    transform.parent.GetComponent<WallSegment>().RemoveAnyPortals();
                    break;
                }
                else if (!activePortals[0])
                    activePortals[0] = this;
                else
                    activePortals[1] = this;
                if (activePortals[0] && activePortals[1])
                {
                    activePortals[0].turretArea.GetComponent<MeshRenderer>().enabled = true;
                    activePortals[1].turretArea.GetComponent<MeshRenderer>().enabled = true;
                }
                minimapTurretVisual.color = turretColors[(int)state - 1];
                rend.enabled = false;
                PortalVisual.enabled = true;
                break;
            case BuffTurret:
                //Logic in turret trigger area (onenter), modifier controls strength (replaces damage)
                rend.material = turretMaterials[2];
                minimapTurretVisual.color = turretColors[(int)state - 1];
                break;
            case SlowTurret:
                //Logic in turret trigger area (onenter), modifier controls strength (replaces damage)
                rend.material = turretMaterials[3];
                minimapTurretVisual.color = turretColors[(int)state - 1];
                break;
            case BarrierTurret:
                minimapTurretVisual.gameObject.SetActive(false);
                rend.enabled = false;
                break;
            default:
                break;
        }
    }

    public static string GetUpgradeText(WallAutomata.TurretState state, int level)
    {
        switch (state)
        {
            case GunTurret:
                if (level == 1) return "Next upgrade:\nRange +2,Speed x2,Power +1";
                if (level == 2) return "Next upgrade:\nRange +2,Speed x2,Power +1";
                if (level == 3) return "Max Level";
                break;
            case CannonTurret:
                if (level == 1) return "Next upgrade:\nRange +1,Power +1";
                if (level == 2) return "Next upgrade:\nSpeed x2,Power +1";
                if (level == 3) return "Max Level";
                break;
            case BuffTurret:
                if (level == 1) return "Next upgrade: +75% speed boost";
                if (level == 2) return "Next upgrade: +100% speed boost";
                if (level == 3) return "Max Level";
                break;
            case SlowTurret:
                if (level == 1) return "Next upgrade: -35% slow";
                if (level == 2) return "Next upgrade: -50% slow";
                if (level == 3) return "Max Level";
                break;
            case EmptyTurret:
            case BarrierTurret:
                return string.Empty;
            default:
                return "Max Level";
        }
        return "sugma nuts :D";
    }

    public static string GetTurretText(WallAutomata.TurretState state, int level)
    {
        switch (state)
        {
            case GunTurret:
                return $"Gun turret \nLevel {level}";
            case CannonTurret:
                return $"Cannon turret \nLevel {level}";
            case PortalTurret:
                return $"Portal turret \nLevel {level}";
            case BuffTurret:
                return $"Buff turret \nLevel {level}";
            case BarrierTurret:
                return $"Barrier \nLevel {level}";
            case SlowTurret:
                return $"Slow turret \nLevel {level}";
            default:
                return string.Empty;
        }
    }

    private void ShootCannonTurret()
    {
        if (turretArea.monstersInArea.Count == 0) return;

        cannonGunExplosion.Play();
        AudioManager.Instance.PlaySFXAtPosition(cannonTurretSFX, transform.position);

        bool hasPlayedParticles = false;
        int maxCount = turretArea.monstersInArea.Count;
        Vector3 spawnPoint = Vector3.Lerp(transform.position, cannonGunExplosion.transform.position, 0.5f);
        for (int i = 0; i < maxCount; i++)
        {
            if (i > turretArea.monstersInArea.Count - 1) return;

            if (turretArea.monstersInArea[i] == null)
                turretArea.monstersInArea.RemoveAt(i);
            else
            {
                if (HasVisibility(spawnPoint, turretArea.monstersInArea[i]))
                {
                    if (!hasPlayedParticles)
                    {
                        cannonGunExplosion.Play();
                        hasPlayedParticles = true;
                    }
                    if (turretArea.monstersInArea[i].TakeDamage(damage))
                        turretArea.monstersInArea.RemoveAt(i);
                }
            }
        }
    }

    private void TeleportIn()
    {
        if (!activePortals[0] || !activePortals[1] || isTeleportDisabled) return;

        lastUsedPortal = this;
        isTeleportDisabled = true;

        PlayerMovement player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        Debug.Log($"Last Position: {player.transform.position}");

        int otherPortal = this == activePortals[0] ? 1 : 0;

        Vector3 pos = activePortals[otherPortal].turretArea.transform.position;
        Vector3 dir = activePortals[otherPortal].turretArea.transform.eulerAngles;

        Debug.DrawRay(pos, dir, Color.red, 10f);

        player.SetPositionAndRotation(pos,dir);

        AudioManager.Instance.PlaySFX(portalTurretSFX);

        Debug.Log($"New Position: {player.transform.position}");
    }

    private void TeleportOut()
    {
        isTeleportDisabled = false;
    }

    private void ShootGunTurret()
    {
        if (turretArea.monstersInArea.Count == 0) return;

        Monster monster = turretArea.monstersInArea[0];
        Vector3 spawnPoint = Vector3.Lerp(transform.position, cannonGunExplosion.transform.position, 0.5f);

        bool isVisible = HasVisibility(spawnPoint, monster);

        while (monster == null && turretArea.monstersInArea.Count > 1 && !isVisible)
        {
            turretArea.monstersInArea.RemoveAt(0);

            monster = turretArea.monstersInArea[0];
            isVisible = HasVisibility(spawnPoint, monster);
        }

        if (monster == null)
        {
            turretArea.monstersInArea.RemoveAt(0);
            return;
        }

        if (!HasVisibility(spawnPoint, monster)) return;

        GunTurretBullet bullet = Instantiate(turretGunBulletPrefab, spawnPoint, Quaternion.identity).GetComponent<GunTurretBullet>();
        bullet.target = monster;
        bullet.damage = damage;
        bullet.turret = this;

        AudioManager.Instance.PlaySFXAtPosition(gunTurretSFX, transform.position);
    }

    public void GunTurretMonsterHit(Monster monster, int damage)
    {
        if (monster == null) return;

        if (monster.TakeDamage(damage))
            turretArea.monstersInArea.Remove(monster);
    }

    public bool HasVisibility(Vector3 origin, Monster monster)
    {
        if (monster == null) return false;

        Debug.DrawRay(origin, origin - transform.position, Color.green, 10, false);
        RaycastHit hit;
        if (Physics.Raycast(origin, monster.transform.position - origin, out hit))
            return hit.transform.gameObject.tag != "Wall";

        return false;
    }
}
