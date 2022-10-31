using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ArenaSetup : MonoBehaviour
{
    [SerializeField] private WallSegment _wallSegment;
    [SerializeField] private Transform _floor;
    [SerializeField] private Transform _zAxis;
    [SerializeField] private Transform _xAxis;
    [SerializeField] private Transform _pathDebugger;

    //temp
    [SerializeField] private PathRenderer pathRenderer;

    private int _buildingLayer;

    private WallSegment[,] wallSegmentsX;
    private WallSegment[,] wallSegmentsZ;
    private PathNode[,] pathNodeGrid;

    private int maxX;
    private int maxZ;

    private Pathfinding pathfinding;

    List<Tuple<Vector2Int, Vector2Int>> pathStartEndCoordinatesList = new List<Tuple<Vector2Int, Vector2Int>>();
    List<List<PathNode>> paths = new List<List<PathNode>>();

    void Start()
    {
        maxX = (int)_floor.localScale.x;
        maxZ = (int)_floor.localScale.z;

        InstantiateWallSegments();
        CreatePathNodes();

        pathfinding = new Pathfinding(pathNodeGrid);

        AddBuildingChecks();

        pathStartEndCoordinatesList.Add(new Tuple<Vector2Int, Vector2Int>(new Vector2Int(0, 0), new Vector2Int(19, 19)));
        pathStartEndCoordinatesList.Add(new Tuple<Vector2Int, Vector2Int>(new Vector2Int(4, 0), new Vector2Int(4, 16)));

        UpdatePaths();
    }

    private bool CheckPathValidity()
    {
        List<bool> pathsValid = new List<bool>();
        foreach (Tuple<Vector2Int, Vector2Int> coords in pathStartEndCoordinatesList)
        {
            pathsValid.Add(pathfinding.TryFindPath(coords.Item1.x, coords.Item1.y, coords.Item2.x, coords.Item2.y));
        }
        return pathsValid.All(x => x);
    }


    private void AddBuildingChecks()
    {
        // Add all the building conditions when trying to place a wall
        foreach (WallSegment wall in wallSegmentsX)
        {
            if (wall != null)
            {
                wall.tryCalculatePaths = CheckPathValidity;
                wall.createNewPaths = UpdatePaths;                
            }
        }

        foreach (WallSegment wall in wallSegmentsZ)
        {
            if (wall != null)
            {
                wall.tryCalculatePaths = CheckPathValidity;
                wall.createNewPaths = UpdatePaths;
            }
        }
    }
    private void UpdatePaths()
    {
        paths.Clear();
        foreach (Tuple<Vector2Int, Vector2Int> coords in pathStartEndCoordinatesList)
        {
            paths.Add(pathfinding.FindPath(coords.Item1.x, coords.Item1.y, coords.Item2.x, coords.Item2.y));
        }

        pathRenderer.SetPathNodes(paths);
    }

    private void DebugPaths()
    {
        for (int i = 0; i < _pathDebugger.childCount; i++)
            Destroy(_pathDebugger.GetChild(i).gameObject);

        int index = 1;

        foreach (List<PathNode> path in paths)
        {
            Color rand = Random.ColorHSV();
            foreach (PathNode node in path)
            {
                GameObject cubeNode = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cubeNode.transform.localScale = new Vector3(5, 1, 5);
                cubeNode.transform.position = node.position;
                cubeNode.transform.parent = _pathDebugger;
                cubeNode.name = "Path " + index + " " + node.ToString();
                cubeNode.GetComponent<MeshRenderer>().material.color = rand;
            }
            index++;
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
