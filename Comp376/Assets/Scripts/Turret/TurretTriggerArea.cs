using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurretTriggerArea : MonoBehaviour
{
    public WallAutomata.TurretState state;

    public UnityEvent TeleportIn, TeleportOut;
    public float modifier = 1f;
    public int rangeX, rangeZ = 1;

    private Vector3 baseScale;

    public List<Monster> monstersInArea = new List<Monster>();

    public void Start()
    {
        baseScale = this.transform.localScale;
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            Monster m = col.gameObject.GetComponent<Monster>();
            monstersInArea.Add(col.gameObject.GetComponent<Monster>());
            if (state == WallAutomata.TurretState.SlowTurret)
                m.ApplySlow(modifier);
        }

        if (col.gameObject.tag == "Player")
        {
            if (state == WallAutomata.TurretState.BuffTurret)
                col.gameObject.GetComponent<PlayerMovement>().ApplyBuff(modifier);
            else if (state == WallAutomata.TurretState.PortalTurret)
            {
                TeleportIn.Invoke();
            }
        }

    }

    public void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            Monster m = col.gameObject.GetComponent<Monster>();
            monstersInArea.Remove(m);

            if (state == WallAutomata.TurretState.SlowTurret)
                m.RemoveSlow();
        }

        if (col.gameObject.tag == "Player")
        {
            if (state == WallAutomata.TurretState.BuffTurret)
                col.gameObject.GetComponent<PlayerMovement>().RemoveBuff();
            else if (state == WallAutomata.TurretState.PortalTurret)
                TeleportOut.Invoke();
        }
    }

    public void SetArea(float rangeZ, int rangeX)
    {
        //Set forward (starting from turret)
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, rangeZ * baseScale.z);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, rangeZ * 16.5f);

        transform.localScale = new Vector3(rangeX * baseScale.x, transform.localScale.y, transform.localScale.z);
    }
}
