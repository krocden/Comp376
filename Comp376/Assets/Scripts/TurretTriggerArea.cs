using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretTriggerArea : MonoBehaviour
{
    List<Monster> monstersInArea = new List<Monster>();

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Enemy") { 
            
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
