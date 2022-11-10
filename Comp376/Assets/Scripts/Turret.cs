using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static WallAutomata.TurretState;

public class Turret : MonoBehaviour
{
    [SerializeField] private WallAutomata.TurretState _currentState;
    private MeshRenderer rend;
    [SerializeField] private TMPro.TMP_Text desc;
    [SerializeField] Material[] turretMaterials;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<MeshRenderer>();
    }

    public void SetMode(WallAutomata.TurretState state)
    {
        _currentState = state;

        rend.enabled = true;

        switch (state) {
            case EmptyTurret:
                rend.enabled = false;
                desc.text = "Empty turret";
                break;
            case GunTurret:
                rend.material = turretMaterials[0];
                desc.text = "Gun turret";
                break;
            case CannonTurret:
                desc.text = "Cannon turret";
                break;
            case PortalTurret:
                desc.text = "Portal turret";
                break;
            case BuffTurret:
                desc.text = "Buff turret";
                break;
            case WallTurret:
                desc.text = "Wall turret";
                break;
            case SlowTurret:
                desc.text = "Slow turret";
                break;
            default:
                break;
        }
    }
}
