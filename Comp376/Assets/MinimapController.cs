using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [SerializeField] private GameObject minimap;
    [SerializeField] private GameObject mapOverlay;

    [SerializeField] private Camera minimapCamera;
    [SerializeField] private Camera mapOverlayCamera;

    private List<Ping> pings = new List<Ping>();

    bool showOverlay = false;

    private void Start()
    {
        NotificationManager.Instance.OnPingAdded.AddListener((ping) => OnPingAdded(ping));
        NotificationManager.Instance.OnPingRemoved.AddListener((ping) => OnPingRemoved(ping));
    }

    private void Update()
    {
        showOverlay = Input.GetKey(KeyCode.Tab);
        minimap.SetActive(!showOverlay);
        mapOverlay.SetActive(showOverlay);

        if (showOverlay)
        {
            for (int i = 0; i < pings.Count; i++)
            {
                if (pings[i] != null)
                {
                    pings[i].icon.transform.eulerAngles = pings[i].initialRotation;
                    pings[i].pointer.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < pings.Count; i++)
            {
                if (pings[i] != null)
                {
                    Vector3 screenPoint = minimapCamera.WorldToViewportPoint(pings[i].ring.transform.position);
                    bool isVisible = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
                    pings[i].pointer.gameObject.SetActive(!isVisible);
                    pings[i].icon.transform.eulerAngles = minimapCamera.transform.eulerAngles;
                    
                    if (!isVisible)
                    {
                        Vector3 to = pings[i].icon.transform.position;
                        to.y = minimapCamera.transform.position.y;
                        Vector3 from = minimapCamera.transform.position;
                        Vector3 direction = (to - from).normalized;
                        pings[i].pointer.transform.position = minimapCamera.transform.position + 0.9f * minimapCamera.orthographicSize * direction - Vector3.up;
                        pings[i].pointer.transform.localScale = pings[i].initialPointerScale * (1f + Mathf.Clamp(minimapCamera.orthographicSize * 2 / Vector3.Distance(from, to) / 2, 0f,1f));
                        pings[i].pointer.transform.up = direction;
                        pings[i].pointer.transform.eulerAngles = new Vector3(90, pings[i].pointer.transform.eulerAngles.y, pings[i].pointer.transform.eulerAngles.z);
                    }
                }
            }
        }
    }

    private void OnPingAdded(Ping ping)
    {
        pings.Add(ping);
    }

    private void OnPingRemoved(Ping ping)
    {
        pings.Remove(ping);
    }
}
