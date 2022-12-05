using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shotgun : Gun
{
    public float damage;
    public int currentAmmo;
    public int maxAmmo;
    public int magazineSize;
    public float fireRate;
    public float fireCd;
    public float range;
    public float spread;
    public int pelletPerRound;
    public bool automatic;
    public bool canShoot;
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
    void reloadFinish()
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
            /*
            // Automatic
            if (Input.GetButton("Fire1") && Time.time > fireCd)
            {
                fireCd = Time.time + fireRate;

                for (int i = 0; i <= pelletPerRound; i++)
                {
                    Vector3 bulletDirection = Camera.main.transform.forward + 
                        new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread));
                    Ray bullet = new Ray(player.transform.position, bulletDirection);
                    Debug.DrawRay(player.transform.position, bulletDirection * 1000, Color.white, 1);

                    if (Physics.Raycast(bullet, out bulletHit))
                    {
                        if (bulletHit.collider.tag == "Enemy")
                        {
                            GameObject hitObject = bulletHit.transform.gameObject;
                            Monster enemy = hitObject.GetComponent<Monster>();
                            float finalDamage = damage;
                            if (bulletHit.distance > range)
                            {
                                float rangeDiff = bulletHit.distance - range;
                                finalDamage *= Mathf.Clamp((1 - (rangeDiff * 0.02f)), 0.001f, 1);
                            }
                            enemy.takeDamage(finalDamage);
                            //Debug.Log("Health: " + enemy.health);
                        }
                    }
                }
            }
            */
        }
        else
        {
            // Semi-Automatic
            if (Input.GetButtonDown("Fire1") && Time.time > fireCd)
            {
                checkAmmo(1);
                if (canShoot && !isReloading)
                {
                    fireCd = Time.time + fireRate;

                    for (int i = 1; i <= pelletPerRound; i++)
                    {
                        Vector3 bulletDirection = Camera.main.transform.forward +
                            new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread));
                        Ray bullet = new Ray(player.transform.position, bulletDirection);
                        Debug.DrawRay(player.transform.position, bulletDirection * 1000, Color.white, 1);

                        RaycastHit[] hits = Physics.RaycastAll(bullet).OrderBy(x => x.distance).ToArray();
                        RaycastHit actualHit = hits[0];

                        if (hits.Length > 0)    //hits will always have atleast 1, but to be safe
                        {
                            for (int j = 0; j < hits.Length; j++)
                            {
                                if (hits[j].collider.tag == "Wall")
                                    if (hits[j].collider.GetComponent<WallSegment>().GetWallState() == WallAutomata.WallState.Barrier)
                                        continue;

                                Debug.Log("Hit: " + hits[j].collider.tag);
                                if (hits[j].collider.tag == "Enemy")
                                {
                                    GameObject hitObject = hits[j].transform.gameObject;
                                    Monster enemy = hitObject.GetComponent<Monster>();

                                    float finalDamage = (j + 1) * damage;
                                    if (hits[j].distance > range)
                                    {
                                        float rangeDiff = hits[j].distance - range;
                                        finalDamage *= Mathf.Clamp((1 - (rangeDiff * 0.02f)), 0.001f, 1);
                                    }
                                    enemy.TakeDamage(finalDamage);
                                    CurrencyManager.Instance.AddCurrency(currencyPerHit);
                                    Debug.Log("Health: " + enemy.health);
                                    Debug.Log("Damage: " + damage + ", Boost: " + (j + 1) + "x");
                                    actualHit = hits[j];
                                    break;
                                }
                                else
                                {
                                    actualHit = hits[j];
                                    break;
                                }
                            }
                        }

                        Vector3 gunSpritePos = player.transform.position + (Camera.main.transform.rotation * gunEffectPos);
                        TrailRenderer trail = Instantiate(bulletTrail, gunSpritePos, Quaternion.identity);
                        StartCoroutine(spawnBulletTrail(trail, actualHit));

                    }

                    anim.SetTrigger("shoot");
                    AudioManager.Instance.PlaySFX(shootShound);

                    return true;
                }
            }
        }
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

    public override void getAmmo(out int currAmmo, out int maxAmmo)
    {
        currAmmo = currentAmmo;
        maxAmmo = this.maxAmmo;
    }

    public override void updateAnim()
    {
        isReloading = false;
        anim.Rebind();
        anim.Update(0f);
    }
}
