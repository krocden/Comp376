using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    // coordinates on the grid
    private readonly int x, z;
    public readonly Vector3 position;

    public int hScore;
    public int fScore;
    public int gScore;

    public WallSegment topWall, bottomWall, rightWall, leftWall;
    public PathNode topNode, bottomNode, rightNode, leftNode;

    public PathNode(int x, int z, Vector3 position)
    {
        this.x = x;
        this.z = z;
        this.position = position;
    }

    public override string ToString()
    {
        return "X: " + x + " z: " + z;
    }

    public void SetNeighbours(PathNode[,] grid, int width, int height)
    {
        if (z < height)
        {
            topNode = grid[x, z + 1];
        }
        
        if (z > 0)
        {
            bottomNode = grid[x, z - 1];
        }

        if (x < width - 1)
        {
            rightNode = grid[x + 1, z];
        }

        if (x > 0)
        {
            leftNode = grid[x - 1, z];
        }
    }

    public void SetWalls(WallSegment[,] wallsX, WallSegment[,] wallsZ, int width, int height)
    {
        Debug.Log(ToString());

        if(z > 0)
            bottomWall = wallsZ[x, z];
        if(z < height - 1)
            topWall = wallsZ[x, z + 1];
        if (x > 0)
            leftWall = wallsX[x, z];
        if (x < width - 1)
            rightWall = wallsX[x + 1, z];
    }

    public List<PathNode> GetAvailableNeighbours()
    {
        List<PathNode> neighbours = new List<PathNode>();
        // temp null check
        if (topNode != null && (topWall == null || topWall.GetWallState() == WallAutomata.WallState.Empty))
            neighbours.Add(topNode);
        if (bottomNode != null && (bottomWall == null || bottomWall.GetWallState() == WallAutomata.WallState.Empty))
            neighbours.Add(bottomNode);
        if (rightNode != null && (rightWall == null || rightWall.GetWallState() == WallAutomata.WallState.Empty))
            neighbours.Add(rightNode);
        if (leftNode != null && (leftWall == null || leftWall.GetWallState() == WallAutomata.WallState.Empty))
            neighbours.Add(leftNode);

        return neighbours;
    }





}
