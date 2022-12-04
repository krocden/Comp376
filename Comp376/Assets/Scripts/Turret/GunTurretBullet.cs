using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunTurretBullet : MonoBehaviour
{
    public Monster target;
    public float speed;
    public Turret turret;
    public int damage;

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            turret.GunTurretMonsterHit(col.GetComponent<Monster>(), damage);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (target == null || target.gameObject == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    }
}
