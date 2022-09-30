using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSetup : MonoBehaviour
{
    [SerializeField] private WallSegment _wallSegment;
    [SerializeField] private Transform _floor;
    [SerializeField] private Transform _zAxis;
    [SerializeField] private Transform _xAxis;
    [SerializeField] private Transform _pathDebugger;
    private int _buildingLayer;

    private WallSegment[,] wallSegmentsX;
    private WallSegment[,] wallSegmentsZ;
    private PathNode[,] pathNodeGrid;

    private int maxX;
    private int maxZ;

    private Pathfinding pathfinding;

    void Start()
    {
        maxX = (int)_floor.localScale.x;
        maxZ = (int)_floor.localScale.z;

        InstantiateWallSegments();
        CreatePathNodes();

        pathfinding = new Pathfinding(pathNodeGrid);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            List<PathNode> path = pathfinding.FindPath(0, 0, 19, 19);
            DebugPath(path);
        }
    }

    private void DebugNodeNeighbours(PathNode pathNode)
    {
        var nodes = pathNode.GetAvailableNeighbours();
        foreach (PathNode node in nodes)
        {
            GameObject cubeNode = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeNode.transform.localScale = new Vector3(5, 1, 5);
            cubeNode.transform.position = node.position;
        }

    }

    private void DebugPath(List<PathNode> path)
    {
        for (int i = 0; i < _pathDebugger.childCount; i++)
            Destroy(_pathDebugger.GetChild(i).gameObject);

        foreach (PathNode node in path)
        {
            GameObject cubeNode = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeNode.transform.localScale = new Vector3(5, 1, 5);
            cubeNode.transform.position = node.position;
            cubeNode.transform.parent = _pathDebugger;
            cubeNode.name = node.ToString();
        }
    }

    private void InstantiateWallSegments()
    {
        int xOffset = maxX * 10 / 2 - 5;
        int zOffset = maxZ * 10 / 2;

        wallSegmentsX = new WallSegment[maxX, maxZ];
        wallSegmentsZ = new WallSegment[maxX, maxZ];

        _buildingLayer = LayerMask.NameToLayer("Building");

        for (int x = 1; x < maxX; x++)
        {
            for (int z = 0; z < maxZ; z++)
            {
                Vector3 wallPosition = new Vector3(x * 10 - xOffset, transform.position.y, z * 10 - zOffset) - Vector3.back * 5f + Vector3.left * 5;
                WallSegment o = Instantiate(_wallSegment, wallPosition, Quaternion.Euler(0, 90, 0), _xAxis);
                o.gameObject.name = "Wall (" + x + "," + z + ")";
                o.gameObject.layer = _buildingLayer;
                wallSegmentsX[x, z] = o;
            }
        }

        for (int x = 0; x < maxX; x++)
        {
            for (int z = 1; z < maxZ; z++)
            {
                Vector3 wallPosition = new Vector3(x * 10 - xOffset, transform.position.y, z * 10 - zOffset);
                WallSegment o = Instantiate(_wallSegment, wallPosition, Quaternion.identity, _zAxis);
                o.gameObject.name = "Wall (" + x + "," + z + ")";
                o.gameObject.layer = _buildingLayer;
                wallSegmentsZ[x, z] = o;
            }
        }
    }

    private void CreatePathNodes()
    {
        int xOffset = maxX * 10 / 2 - 5;
        int zOffset = maxZ * 10 / 2 - 5;

        pathNodeGrid = new PathNode[maxX, maxZ];

        for (int x = 0; x < maxX; x++)
        {
            for (int z = 0; z < maxZ; z++)
            {
                Vector3 nodePosition = new Vector3(x * 10 - xOffset, transform.position.y, z * 10 - zOffset);
                PathNode node = pathNodeGrid[x, z] = new PathNode(x, z, nodePosition);

                node.SetWalls(wallSegmentsX, wallSegmentsZ, maxX, maxZ);
            }
        }

        foreach (PathNode node in pathNodeGrid)
        {
            node.SetNeighbours(pathNodeGrid, maxX - 1, maxZ - 1);
        }
    }
}
