using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretTriggerArea : MonoBehaviour
{
    public List<Monster> monstersInArea = new List<Monster>();

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Enemy") {
            monstersInArea.Add(col.gameObject.GetComponent<Monster>());
        }
    }

    public void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            monstersInArea.Remove(col.gameObject.GetComponent<Monster>());
        }
    }
}
