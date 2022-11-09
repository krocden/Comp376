using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nexus : MonoBehaviour
{
    [SerializeField] private Transform nexusCrystal;
    public Transform nexusBase;

    Vector3 cubeBasePosition;
    private void Start()
    {
        cubeBasePosition = nexusCrystal.position;
    }

    private void Update()
    {
        nexusCrystal.position = cubeBasePosition + Vector3.up * Mathf.Sin(Time.time);
        nexusCrystal.Rotate(Vector3.right + Vector3.up, 1);
    }
}
