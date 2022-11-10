using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class ArenaSetup : MonoBehaviour
{
    [SerializeField] private WallSegment _wallSegment;
    [SerializeField] private Transform _floor;
    [SerializeField] private Transform _zAxis;
    [SerializeField] private Transform _xAxis;
    [SerializeField] private Transform _pathDebugger;
    [SerializeField] private Transform _player;

    //temp
    [SerializeField] private PathRenderer pathRenderer;

    [SerializeField] private WallDestroyerMonster monsterWallDestroyerPrefab;

    public Nexus nexus;

    private int _buildingLayer;

    private WallSegment[,] wallSegmentsX;
    private WallSegment[,] wallSegmentsZ;
    private PathNode[,] pathNodeGrid;

    private int maxX;
    private int maxZ;
    private int nodeSize = 10;

    private Pathfinding pathfinding;

    public List<Vector4> pathStartEndCoordinatesList = new List<Vector4>();
    public List<List<PathNode>> paths = new List<List<PathNode>>();

    public void InitializeArena()
    {
        maxX = (int)_floor.localScale.x;
        maxZ = (int)_floor.localScale.z;

        InstantiateWallSegments();
        CreatePathNodes();

        pathfinding = new Pathfinding(pathNodeGrid);

        AddBuildingChecks();
    }

    public async Task AddPath()
    {
        List<Vector2Int> possiblePathEnds = new List<Vector2Int>() 
        { 
            new Vector2Int(maxX / 2 - 1, maxZ / 2 - 1), // bottom left
            new Vector2Int(maxX / 2, maxZ / 2 - 1), // bottom right
            new Vector2Int(maxX / 2 - 1, maxZ / 2), // top left
            new Vector2Int(maxX / 2, maxZ / 2) // top right
        };

        int rowOrCol = Random.Range(0, 2);

        // x is random, z is min or max
        Vector4 newPath;

        int randomX;
        int randomZ;
        do
        {
            if (rowOrCol == 0)
            {
                randomX = Random.Range(0, maxX);
                randomZ = Random.Range(0, 2) * (maxZ - 1);
            }
            else
            {
                randomX = Random.Range(0, 2) * (maxX - 1);
                randomZ = Random.Range(0, maxZ);
            }
        }
        while (PathAlreadyExists(new Vector2Int(randomX, randomZ)));

        Vector2Int pathStart = new Vector2Int(randomX, randomZ);
        Vector2Int pathEnd = possiblePathEnds[0];
        for (int i = 0; i < possiblePathEnds.Count; i++)
        {
            if (Vector2.Distance(pathStart, possiblePathEnds[i]) < Vector2.Distance(pathStart, pathEnd))
                pathEnd = possiblePathEnds[i];
        }

        newPath = new Vector4(pathStart.x, pathStart.y, pathEnd.x, pathEnd.y);

        if (!pathfinding.TryFindPath(newPath))
        {
            List<PathNode> pathToDestroy = pathfinding.FindPath(newPath, ignoreWalls: true);
            Vector3 spawnPoint = pathToDestroy[0].position + Vector3.up;
            WallDestroyerMonster monster = Instantiate(monsterWallDestroyerPrefab, spawnPoint, Quaternion.identity);
            monster.Initialize(pathToDestroy, nexus);

            while (monster != null)
            {
                await Task.Yield();
            }
        }

        pathStartEndCoordinatesList.Add(newPath);
        UpdatePaths();
    }

    private bool PathAlreadyExists(Vector2Int pathStart)
    {
        foreach (Vector4 pathCoord in pathStartEndCoordinatesList)
        {
            if ((int)pathCoord.x == pathStart.x && (int)pathCoord.y == pathStart.y)
                return true;
        }
        return false;
    }

    private bool CheckPathValidity()
    {
        List<bool> pathsValid = new List<bool>();
        foreach (Vector4 coords in pathStartEndCoordinatesList)
        {
            pathsValid.Add(pathfinding.TryFindPath(coords));
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
        foreach (Vector4 coords in pathStartEndCoordinatesList)
        {
            paths.Add(pathfinding.FindPath(coords));
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
        int xOffset = maxX * nodeSize / 2 - (nodeSize / 2) ;
        int zOffset = maxZ * nodeSize / 2;

        int width = maxX / 2;
        int height = maxZ / 2;

        wallSegmentsX = new WallSegment[maxX, maxZ];
        wallSegmentsZ = new WallSegment[maxX, maxZ];

        _buildingLayer = LayerMask.NameToLayer("Building");

        // Vertical

        for (int x = 1; x < maxX; x++)
        {
            for (int z = 0; z < maxZ; z++)
            {
                if (x >= width - 1 && z >= height - 2 && x <= width + 1 && z <= height + 1)
                    continue;

                Vector3 wallPosition = new Vector3(x * nodeSize - xOffset, transform.position.y, z * nodeSize - zOffset) - Vector3.back * 5f + Vector3.left * 5;
                WallSegment o = Instantiate(_wallSegment, wallPosition, Quaternion.Euler(0, 90, 0), _xAxis);
                o.gameObject.name = "Wall (" + x + "," + z + ")";
                o.gameObject.layer = _buildingLayer;
                o.PlayerTransform = _player;
                wallSegmentsX[x, z] = o;
            }
        }

        // Horizontal

        for (int x = 0; x < maxX; x++)
        {
            for (int z = 1; z < maxZ; z++)
            {
                if (x >= width - 2 && z >= height - 1 && x <= width + 1 && z <= height + 1)
                    continue;

                Vector3 wallPosition = new Vector3(x * nodeSize - xOffset, transform.position.y, z * nodeSize - zOffset);
                WallSegment o = Instantiate(_wallSegment, wallPosition, Quaternion.identity, _zAxis);
                o.gameObject.name = "Wall (" + x + "," + z + ")";
                o.gameObject.layer = _buildingLayer;
                o.PlayerTransform = _player;
                wallSegmentsZ[x, z] = o;
            }
        }
    }

    private void CreatePathNodes()
    {
        int xOffset = maxX * nodeSize / 2 - (nodeSize / 2);
        int zOffset = maxZ * nodeSize / 2 - (nodeSize / 2);

        pathNodeGrid = new PathNode[maxX, maxZ];

        for (int x = 0; x < maxX; x++)
        {
            for (int z = 0; z < maxZ; z++)
            {
                Vector3 nodePosition = new Vector3(x * nodeSize - xOffset, transform.position.y, z * nodeSize - zOffset);
                PathNode node = pathNodeGrid[x, z] = new PathNode(x, z, nodePosition, nodeSize);

                node.SetWalls(wallSegmentsX, wallSegmentsZ, maxX, maxZ);
            }
        }

        foreach (PathNode node in pathNodeGrid)
        {
            node.SetNeighbours(pathNodeGrid, maxX - 1, maxZ - 1);
        }
    }
}
