using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour, Gun
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

    public void reload()
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

                //isReloading = true; no Animation yet...

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
            if (!emptyMagSound.isPlaying && !reloadSound.isPlaying)
            {
                emptyMagSound.Play();
            }
        }
        else
        {
            canShoot = true;
            currentAmmo -= ammo;
        }

    }
    public bool shoot()
    {
        RaycastHit bulletHit;

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
                                enemy.TakeDamage(finalDamage);
                            }
                        }
                        Vector3 gunSpritePos = player.transform.position + (Camera.main.transform.rotation * gunEffectPos);
                        TrailRenderer trail = Instantiate(bulletTrail, gunSpritePos, Quaternion.identity);
                        StartCoroutine(spawnBulletTrail(trail, bulletHit));

                    }

                    anim.SetTrigger("shoot");
                    shootShound.Play();

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

    public void getAmmo(out int currAmmo, out int maxAmmo)
    {
        currAmmo = currentAmmo;
        maxAmmo = this.maxAmmo;
    }
}
