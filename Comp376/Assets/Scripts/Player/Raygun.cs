using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raygun : Gun
{
    public float damage;
    //public int currentAmmo;
    //public int maxAmmo;
    //public int magazineSize;
    //public float fireRate;
    //public float fireCd;
    //public float range;
    //public bool automatic;
    //public bool canShoot;
    public float headshotMultiplier;
    //bool isReloading = false;

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

    public override void reload()
    {

    }

    public override bool shoot()
    {
        RaycastHit bulletHit;

        if (Input.GetButton("Fire1"))
        {
            Ray bullet = new Ray(player.transform.position, Camera.main.transform.forward);
            Debug.DrawRay(player.transform.position, Camera.main.transform.forward * 1000, Color.white, 1);
            if (Physics.Raycast(bullet, out bulletHit))
            {
                if (bulletHit.collider.tag == "Enemy")
                {
                    GameObject hitObject = bulletHit.transform.gameObject;
                    Monster enemy = hitObject.GetComponent<Monster>();
                    enemy.TakeDamage(damage);
                    CurrencyManager.Instance.AddCurrency(currencyPerHit);
                    Debug.Log("Health: " + enemy.health);
                }
            }

            Vector3 gunSpritePos = player.transform.position + (Camera.main.transform.rotation * gunEffectPos);
            TrailRenderer trail = Instantiate(bulletTrail, gunSpritePos, Quaternion.identity);
            StartCoroutine(spawnBulletTrail(trail, bulletHit));

            anim.SetTrigger("shoot");

            if (!shootShound.isPlaying)
            {
                shootShound.Play();
            }

            return true;
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

    public override void getAmmo(out int currentAmmo, out int maxAmmo)
    {
        currentAmmo = 0;
        maxAmmo = 0;
    }
}
