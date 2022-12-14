using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherProjectile : MonoBehaviour
{
    public GameObject impactParticle;
    public float impactDuration;
    public float damageRadius;
    public Launcher gun;
    public int currencyPerHit;
    private int boost = 1;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "Player")
        {
            GameObject impact = Instantiate(impactParticle, collision.GetContact(0).point, Quaternion.identity);

            Collider[] enemyList = Physics.OverlapSphere(transform.position, damageRadius);
            foreach (Collider enemy in enemyList)
            {
                if (enemy.tag == "Enemy")
                {
                    Monster mob = enemy.GetComponent<Monster>();
                    mob.TakeDamage(boost * gun.damage);
                    Debug.Log("Health: " + mob.health);
                    Debug.Log("Dmg: " + gun.damage + ", Boost: "+boost+"x");
                    CurrencyManager.Instance.AddCurrency(currencyPerHit);
                }
            }

            Destroy(impact, impactDuration);
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall") {
            boost++;
        }
    }
}
