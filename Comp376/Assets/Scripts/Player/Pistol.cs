using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pistol : Gun
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
    bool isReloading = false;

    public GameObject player;
    Animator anim;
    AudioSource shootShound;
    AudioSource reloadSound;
    AudioSource emptyMagSound;

    public GameObject impactParticle;
    public TrailRenderer bulletTrail;

    [SerializeField] Vector3 gunEffectPos;

    void Start()
    {
        anim = GetComponent<Animator>();
        shootShound = GetComponents<AudioSource>()[0];
        reloadSound = GetComponents<AudioSource>()[1];
        emptyMagSound = GetComponents<AudioSource>()[2];
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

                isReloading = true;

                anim.SetTrigger("reload");
                reloadSound.Play();
            }
        }
    }

    void checkAmmo(int ammo)
    {
        if (currentAmmo <= 0)
        {
            canShoot = false;
            emptyMagSound.Play();
        }
        else
        {
            canShoot = true;
            currentAmmo -= ammo;
        }
    }

    public override bool shoot()
    {
        //RaycastHit bulletHit;
        if (Input.GetButtonDown("Fire1") && Time.time > fireCd)
        {
            checkAmmo(1);
            if (canShoot && !isReloading)
            {
                fireCd = Time.time + fireRate;
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
                        CurrencyManager.Instance.AddCurrency(currencyPerHit);
                        if (hits[i].collider.tag == "Enemy")
                        {
                            GameObject hitObject = hits[i].transform.gameObject;
                            Monster enemy = hitObject.GetComponent<Monster>();

                            enemy.TakeDamage((i + 1) * damage);
                            Debug.Log("Health: " + enemy.health);
                            Debug.Log("Damage: " + damage + ", Boost: " + (i+1)+"x");
                            actualHit = hits[i];
                            break;
                        }
                        else {
                            actualHit = hits[i];
                            break;
                        }
                    }
                }

                Vector3 gunSpritePos = player.transform.position + (Camera.main.transform.rotation * gunEffectPos);
                TrailRenderer trail = Instantiate(bulletTrail, gunSpritePos, Quaternion.identity);
                StartCoroutine(spawnBulletTrail(trail, actualHit));

                anim.SetTrigger("shoot");
                shootShound.Play();

                return true;
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

        ParticleSystem particle = impactParticle.GetComponentInChildren<ParticleSystem>();
        particle.Stop();
        var main = particle.main;
        main.duration = 0.25f;
        main.stopAction = ParticleSystemStopAction.Destroy;
        main.loop = false;
        particle.Play();

        Instantiate(particle, hit.point, Quaternion.LookRotation(hit.normal));

        Destroy(trail.gameObject, trail.time);
    }

    public override void getAmmo(out int currentAmmo, out int maxAmmo)
    {
        currentAmmo = this.currentAmmo;
        maxAmmo = this.maxAmmo;
    }

}
