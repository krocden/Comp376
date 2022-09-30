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
    private int _buildingLayer;

    private WallSegment[,] wallSegmentsX;
    private WallSegment[,] wallSegmentsZ;
    private PathNode[,] pathNodeGrid;

    private int maxX;
    private int maxZ;

    void Start()
    {
        maxX = (int)_floor.localScale.x;
        maxZ = (int)_floor.localScale.z;

        InstantiateWallSegments();
        CreatePathNodes();
        DebugNodeWalls();
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

        //_xAxis = Instantiate(_zAxis, Vector3.zero, Quaternion.Euler(0, 90, 0), transform);
        //_xAxis.name = "X-Axis";
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

                //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //cube.transform.position = node.position;
                //cube.name = "Node " + node.ToString();
                //cube.transform.parent = transform;
            }
        }

        foreach (PathNode node in pathNodeGrid)
        {
            node.SetNeighbours(pathNodeGrid, maxX - 1, maxZ - 1);
        }
    }


    private void DebugNodeWalls()
    {
        foreach (PathNode node in pathNodeGrid)
        {
            Color wallColor = new Color(UnityEngine.Random.Range(0F, 1F), UnityEngine.Random.Range(0, 1F), UnityEngine.Random.Range(0, 1F)); ;
            GameObject cubeNode = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeNode.transform.position = node.position;
            cubeNode.GetComponent<MeshRenderer>().material.color = wallColor;
            cubeNode.name = node.ToString();


            if (node.topWall != null)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = node.topWall.transform.position - Vector3.forward;
                cube.name = "top wall";
                cube.GetComponent<MeshRenderer>().material.color = wallColor;
            }

            if (node.bottomWall != null)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = node.bottomWall.transform.position - Vector3.back;
                cube.name = "bottom wall";
                cube.GetComponent<MeshRenderer>().material.color = wallColor;
            }

            if (node.rightWall != null)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = node.rightWall.transform.position - Vector3.right;
                cube.name = "right wall";
                cube.GetComponent<MeshRenderer>().material.color = wallColor;
            }


            if (node.leftWall != null)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = node.leftWall.transform.position - Vector3.left;
                cube.name = "left wall";
                cube.GetComponent<MeshRenderer>().material.color = wallColor;
            }
        }
    }
}
