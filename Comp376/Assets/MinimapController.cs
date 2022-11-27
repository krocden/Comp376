using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [SerializeField] private GameObject minimap;
    [SerializeField] private GameObject mapOverlay;

    private bool showMinimap;

    private void Update()
    {
        mapOverlay.SetActive(Input.GetKey(KeyCode.Tab));
    }
}
