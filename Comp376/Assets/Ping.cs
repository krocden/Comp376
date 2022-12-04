using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ping : MonoBehaviour
{
    [SerializeField] private float duration = 5f;
    public GameObject ring;
    public GameObject icon;
    public GameObject pointer;
    Vector3 initialScale; 
    public Vector3 initialPointerScale;
    public Vector3 initialRotation;

    private void Start()
    {
        initialScale = ring.transform.localScale;
        initialPointerScale = pointer.transform.localScale;
        initialRotation = transform.eulerAngles;
        Destroy(gameObject, duration);
    }

    // Update is called once per frame
    void Update()
    {
        ring.transform.localScale = initialScale * (Mathf.Sin(Time.time * 3f) * 0.25f) + Vector3.one;
    }

    private void OnDestroy()
    {
        NotificationManager.Instance.RemovePing(this);
    }
}
