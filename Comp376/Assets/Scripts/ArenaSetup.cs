using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSetup : MonoBehaviour
{
    [SerializeField] private GameObject _wallSegment;
    [SerializeField] private Transform _floor;
    [SerializeField] private Transform _zAxis;
    private Transform _xAxis;
    private int _buildingLayer;

    void Start()
    {
        int maxX = ((int)_floor.localScale.x * 10 / 2 - 5);
        int maxZ = ((int)_floor.localScale.z * 10 / 2);
        _buildingLayer = LayerMask.NameToLayer("Building");

        for (int x = -maxX; x < maxX + 5; x += 10)
        {
            for (int z = -maxZ + 10; z < maxZ; z += 10)
            {
                GameObject o = Instantiate(_wallSegment, new Vector3(x, 0, z), Quaternion.identity, _zAxis);
                o.name = "Wall (" + x + "," + z + ")";
                o.layer = _buildingLayer;
            }
        }

        _xAxis = Instantiate(_zAxis, Vector3.zero, Quaternion.Euler(0, 90, 0), transform);
        _xAxis.name = "X-Axis";
    }
}
