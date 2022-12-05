using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : Gun
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
    public float explosionRadius;
    bool isReloading = false;

    public GameObject player;
    Animator anim;
    [SerializeField] AudioClip shootShound;
    [SerializeField] AudioClip reloadSound;
    [SerializeField] AudioClip emptyMagSound;

    public ParticleSystem impactParticle;
    public TrailRenderer bulletTrail;
    public GameObject bulletProjectile;
    public float bulletSpeed;

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

                //isReloading = true; no Animation yet...

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
        RaycastHit bulletHit;

        if (automatic)
        {
            /*
            // Automatic
            if (Input.GetButton("Fire1") && Time.time > fireCd)
            {
                fireCd = Time.time + fireRate;
                Ray bullet = new Ray(player.transform.position, Camera.main.transform.forward);
                Debug.DrawRay(player.transform.position, Camera.main.transform.forward * 1000, Color.white, 1);
                if (Physics.Raycast(bullet, out bulletHit))
                {
                    //Debug.Log("Hit: " + bulletHit.collider.tag);
                    if (bulletHit.collider.tag == "Enemy")
                    {
                        GameObject hitObject = bulletHit.transform.gameObject;
                        Monster enemy = hitObject.GetComponent<Monster>();
                        enemy.health -= damage;
                        Debug.Log("Health: " + enemy.health);
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
                    
                    Ray bullet = new Ray(player.transform.position, Camera.main.transform.forward);

                    Debug.DrawRay(player.transform.position, Camera.main.transform.forward * 1000, Color.white, 1);
                    if (Physics.Raycast(bullet, out bulletHit))
                    {
                        Vector3 bulletDestination = bulletHit.point;
                        instantiateProjectile(bulletDestination);
                    }

                    anim.SetTrigger("shoot");
                    AudioManager.Instance.PlaySFX(shootShound);

                    return true;
                }
            }
        }
        return false;
    }

    void instantiateProjectile(Vector3 bulletDestination)
    {
        //Vector3 gunSpritePos = player.transform.position + (Camera.main.transform.rotation * gunEffectPos);
        GameObject bullet = Instantiate(bulletProjectile, player.transform.position, Quaternion.identity); ;
        bullet.GetComponent<LauncherProjectile>().damageRadius = explosionRadius;
        bullet.GetComponent<Rigidbody>().velocity = (bulletDestination - player.transform.position).normalized * bulletSpeed;
        Physics.IgnoreCollision(bullet.GetComponent<Collider>(), player.GetComponent<Collider>());
    }

    public override void getAmmo(out int currAmmo, out int maxAmmo)
    {
        currAmmo = currentAmmo;
        maxAmmo = this.maxAmmo;
    }
}
