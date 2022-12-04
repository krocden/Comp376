using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Raygun : Gun
{
    public float damage;
    public float headshotMultiplier;

    public GameObject player;
    Animator anim;
    AudioSource shootShound;

    //public GameObject impactParticle;
    public TrailRenderer bulletTrail;

    [SerializeField] Vector3 gunEffectPos;

    void Start()
    {
        anim = GetComponent<Animator>();
        shootShound = GetComponents<AudioSource>()[0];
    }

    public override void reload()
    {

    }

    public override bool shoot()
    {
        if (Input.GetButton("Fire1"))
        {
            Ray bullet = new Ray(player.transform.position, Camera.main.transform.forward);
            
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

                        Debug.Log("Health: " + enemy.health);
                        Debug.Log("Damage: " + damage + ", Boost: " + (i + 1) + "x");

                        actualHit = hits[i];
                        //break;
                    }
                    else
                    {
                        actualHit = hits[i];
                        //break;
                    }
                }
            }

            Vector3 gunSpritePos = player.transform.position + (Camera.main.transform.rotation * gunEffectPos);
            TrailRenderer trail = Instantiate(bulletTrail, gunSpritePos, Quaternion.identity);
            StartCoroutine(spawnBulletTrail(trail, actualHit));

            //anim.SetTrigger("shoot");

            if (!shootShound.isPlaying)
            {
                shootShound.Play();
            }

            return true;
        }
        else
        {
            if (shootShound.isPlaying)
            {
                shootShound.Stop();
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
        //Instantiate(impactParticle.GetComponentInChildren<ParticleSystem>(), hit.point, Quaternion.LookRotation(hit.normal));

        Destroy(trail.gameObject, trail.time);
    }

    public override void getAmmo(out int currentAmmo, out int maxAmmo)
    {
        currentAmmo = 0;
        maxAmmo = 0;
    }
}
