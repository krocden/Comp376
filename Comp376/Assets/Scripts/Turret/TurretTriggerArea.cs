using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurretTriggerArea : MonoBehaviour
{
    public WallAutomata.TurretState state;

    public UnityEvent TeleportIn, TeleportOut;

    public List<Monster> monstersInArea = new List<Monster>();

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Enemy") {
            Monster m = col.gameObject.GetComponent<Monster>();
            monstersInArea.Add(col.gameObject.GetComponent<Monster>());
            if (state == WallAutomata.TurretState.SlowTurret)
                m.ApplySlow();
        }

        if (col.gameObject.tag == "Player") {
            if (state == WallAutomata.TurretState.BuffTurret)
                col.gameObject.GetComponent<PlayerMovement>().ApplyBuff();
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

        if (col.gameObject.tag == "Player") {
            if (state == WallAutomata.TurretState.BuffTurret)
                col.gameObject.GetComponent<PlayerMovement>().RemoveBuff();
            else if (state == WallAutomata.TurretState.PortalTurret)
                TeleportOut.Invoke();
        }
    }
}
