using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rifle : Gun
{
    public float damage;
    public int currentAmmo;
    public int maxAmmo;
    public int magazineSize;
    public float fireRate;
    public float fireCd;
    public float range;
    public bool automatic;
    public bool canShoot;
    public float headshotMultiplier;
    bool isReloading = false;

    public GameObject player;
    Animator anim;
    [SerializeField] AudioClip shootShound;
    [SerializeField] AudioClip reloadSound;
    [SerializeField] AudioClip emptyMagSound;

    public GameObject impactParticle;
    public TrailRenderer bulletTrail;

    [SerializeField] Vector3 gunEffectPos;

    void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void reloadFinish()
    {
        isReloading = false;
    }

    public override void reload()
    {
        if (maxAmmo > 0)
        {
            if (currentAmmo != magazineSize)
            {
                if (maxAmmo < magazineSize)
                {
                    currentAmmo = maxAmmo;
                    maxAmmo = 0;
                }
                else
                {
                    if (currentAmmo > 0)
                    {
                        maxAmmo -= (magazineSize - currentAmmo);
                    }
                    else
                    {
                        maxAmmo -= magazineSize;
                    }
                    currentAmmo = magazineSize;
                }

                isReloading = true; //no Animation yet...

                anim.SetTrigger("reload");
                AudioManager.Instance.PlaySFX(reloadSound);
            }
        }
    }
    void checkAmmo(int ammo)
    {
        if (currentAmmo <= 0)
        {
            canShoot = false;
            AudioManager.Instance.PlaySFX(emptyMagSound);
            reload();
        }
        else
        {
            if (!isReloading)
            {
                canShoot = true;
                currentAmmo -= ammo;
            }
        }

    }

    public override bool shoot()
    {
        //RaycastHit bulletHit;

        if (automatic)
        {
            if (Input.GetButton("Fire1"))
            {
                if (Time.time > fireCd)
                {
                    fireCd = Time.time + fireRate;
                    checkAmmo(1);
                    if (canShoot && !isReloading)
                    {
                        Ray bullet = new Ray(player.transform.position, Camera.main.transform.forward);
                        Debug.DrawRay(player.transform.position, Camera.main.transform.forward * 1000, Color.white, 1);

                        RaycastHit[] hits = Physics.RaycastAll(bullet).OrderBy(x => x.distance).ToArray();
                        RaycastHit actualHit = hits[0];

                        if (hits.Length > 0)    //hits will always have atleast 1, but to be safe
                        {
                            for (int i = 0; i < hits.Length; i++)
                            {
                                if (hits[i].collider.tag == "Wall")
                                    if (hits[i].collider.GetComponent<WallSegment>().GetWallState() == WallAutomata.WallState.Barrier)
                                        continue;

                                Debug.Log("Hit: " + hits[i].collider.tag);
                                if (hits[i].collider.tag == "Enemy")
                                {
                                    GameObject hitObject = hits[i].transform.gameObject;
                                    Monster enemy = hitObject.GetComponent<Monster>();

                                    enemy.TakeDamage((i + 1) * damage);
                                    CurrencyManager.Instance.AddCurrency(currencyPerHit);
                                    Debug.Log("Health: " + enemy.health);
                                    Debug.Log("Damage: " + damage + ", Boost: " + (i + 1) + "x");
                                    actualHit = hits[i];
                                    break;
                                }
                                else
                                {
                                    actualHit = hits[i];
                                    break;
                                }
                            }
                        }

                        Vector3 gunSpritePos = player.transform.position + (Camera.main.transform.rotation * gunEffectPos);
                        TrailRenderer trail = Instantiate(bulletTrail, gunSpritePos, Quaternion.identity);
                        StartCoroutine(spawnBulletTrail(trail, actualHit));

                        //anim.SetTrigger("shoot");
                        anim.SetBool("shooting", true);

                        AudioManager.Instance.PlaySFX(shootShound);

                        return true;
                    }
                }
                
            }
            else
            {
                anim.SetBool("shooting", false);
            }
        }/*
        else
        {
            // Semi-Automatic
            if (Input.GetButtonDown("Fire1") && Time.time > fireCd)
            {
                fireCd = Time.time + fireRate;
                Ray bullet = new Ray(player.transform.position, Camera.main.transform.forward);
                Debug.DrawRay(player.transform.position, Camera.main.transform.forward * 1000, Color.white, 1);
                if (Physics.Raycast(bullet, out bulletHit))
                {
                    Debug.Log("Hit: " + bulletHit.collider.tag);
                    if (bulletHit.collider.tag == "Enemy")
                    {
                        GameObject hitObject = bulletHit.transform.gameObject;
                        Monster enemy = hitObject.GetComponent<Monster>();
                        enemy.takeDamage(damage);
                        Debug.Log("Health: " + enemy.health);
                    }
                }
            }
        }*/
        return false;
    }
    IEnumerator spawnBulletTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        trail.transform.position = hit.point;
        Instantiate(impactParticle.GetComponentInChildren<ParticleSystem>(), hit.point, Quaternion.LookRotation(hit.normal));

        Destroy(trail.gameObject, trail.time);
    }

    public override void getAmmo(out int currentAmmo, out int maxAmmo)
    {
        currentAmmo = this.currentAmmo;
        maxAmmo = this.maxAmmo;
    }
    public override void updateAnim()
    {
        isReloading = false;
        anim.Rebind();
        anim.Update(0f);
    }
}
