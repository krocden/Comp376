using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Nexus : MonoBehaviour
{
    [SerializeField] private Transform nexusCrystal;
    public Transform nexusBase;
    [SerializeField] private float maxNexusHealth = 50;
    private float nexusHealth;

    public UnityEvent OnNexusExploded;

    Vector3 cubeBasePosition;
    private void Start()
    {
        cubeBasePosition = nexusCrystal.position;
        nexusHealth = maxNexusHealth;
        UpdateNexusUI();
    }

    private void Update()
    {
        nexusCrystal.position = cubeBasePosition + Vector3.up * Mathf.Sin(Time.time);
        nexusCrystal.Rotate(Vector3.right + Vector3.up, 1);
    }

    public void TakeDamage(float damage)
    {
        nexusHealth -= damage;

        NotificationManager.Instance.PlayPositionnalNotification(NotificationType.NexusUnderAttack, transform.position, true);

        if (nexusHealth <= 0)
        {
            nexusHealth = 0;
            Explode();
        }

        UpdateNexusUI();
    }

    private void Explode()
    {
        Destroy(gameObject);

        OnNexusExploded?.Invoke();
    }

    private void UpdateNexusUI()
    {
        GameStateManager.Instance.UpdateNexusHealth(nexusHealth, maxNexusHealth);
    }
}
