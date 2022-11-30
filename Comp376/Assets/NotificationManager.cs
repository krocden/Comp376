using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] private AudioSource source;

    [SerializeField] private AudioClip[] notificationSounds; // has to match the notification enum

    private Queue<NotificationType> notificationQueue = new Queue<NotificationType>();
    private Dictionary<NotificationType, float> notificationTypeCooldown = new Dictionary<NotificationType, float>();

    [SerializeField] private float notificationCooldownDuration = 5f;
    [SerializeField] private Ping pingPrefab;

    public UnityEvent<Ping> OnPingAdded = new UnityEvent<Ping>();
    public UnityEvent<Ping> OnPingRemoved = new UnityEvent<Ping>();

    public static NotificationManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        for (int i = 0; i < Enum.GetValues(typeof(NotificationType)).Length; i++)
        {
            notificationTypeCooldown[(NotificationType)i] = notificationCooldownDuration;
        }
    }

    private void Update()
    {
        UpdateCooldowns();

        if (notificationQueue.Count > 0 && !source.isPlaying)
        {
            NotificationType notificationType = notificationQueue.Dequeue();
            notificationTypeCooldown[notificationType] = 0;
            source.clip = notificationSounds[(int)notificationType];
            source.Play();
        }
    }

    public void PlayNotificationSFX(NotificationType notificationType)
    {
        source.PlayOneShot(notificationSounds[(int)notificationType]);
    }

    public void PlayStandardNotification(NotificationType notificationType, bool waitToPlay = false)
    {
        if (!waitToPlay && notificationQueue.Count > 0)
            return;

        if (CanPlayNotification(notificationType))
            notificationQueue.Enqueue(notificationType);
    }

    public void PlayPositionnalNotification(NotificationType notificationType, Vector3 position, bool waitToPlay = false)
    {
        PlayStandardNotification(notificationType, waitToPlay);
        PingOnMap(position);
    }

    public void PingOnMap(Vector3 position)
    {
        Ping ping = Instantiate(pingPrefab, position, Quaternion.Euler(90,0,0));
        OnPingAdded.Invoke(ping);
    }

    public void RemovePing(Ping ping)
    {
        OnPingAdded.Invoke(ping);
    }

    private bool CanPlayNotification(NotificationType notificationType)
    {
        return !notificationQueue.Contains(notificationType) && notificationTypeCooldown[notificationType] > notificationCooldownDuration;
    }

    private void UpdateCooldowns()
    {
        for (int i = 0; i < Enum.GetValues(typeof(NotificationType)).Length; i++)
        {
            notificationTypeCooldown[(NotificationType)i] += Time.deltaTime;
        }
    }



}

public enum NotificationType
{ 
    NewSpawnerAdded,
    NewSpawnerNextRound,
    WaveIncoming,
    BossSpawned,
    WaveCompleted,
    NexusUnderAttack,
    PlayerDied,
    NotEnoughCurrency,
    GameOver,
}
