using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    // coordinates on the grid
    private readonly int x, z;
    public readonly float nodeSize;
    public readonly Vector3 position;

    public int hCost;
    public int fCost;
    public int gCost;

    public WallSegment topWall, bottomWall, rightWall, leftWall;
    public PathNode topNode, bottomNode, rightNode, leftNode, topRightNode, topLeftNode, bottomRightNode, bottomLeftNode;

    public PathNode parentNode;

    public PathNode(int x, int z, Vector3 position, float nodeSize)
    {
        this.x = x;
        this.z = z;
        this.position = position;
        this.nodeSize = nodeSize;
    }

    public override string ToString()
    {
        return "X: " + x + " z: " + z;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    /// <summary>
    /// Configure all the neighbouring nodes of a given node
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void SetNeighbours(PathNode[,] grid, int width, int height)
    {
        if (z < height)
        {
            topNode = grid[x, z + 1];
            if (x < width)
                topRightNode = grid[x + 1, z + 1];
            if(x > 0)
                topLeftNode = grid[x - 1, z + 1];
        }

        if (z > 0)
        {
            bottomNode = grid[x, z - 1];
            if (x < width)
                bottomRightNode = grid[x + 1, z - 1];
            if (x > 0)
                bottomLeftNode = grid[x - 1, z - 1];
        }

        if (x < width)
        {
            rightNode = grid[x + 1, z];
        }

        if (x > 0)
        {
            leftNode = grid[x - 1, z];
        }
    }

    /// <summary>
    /// Associate the correspoding walls to a path node,
    /// wall grid must be of same size
    /// </summary>
    /// <param name="wallsX"> grid of vertical walls</param>
    /// <param name="wallsZ"> grid of horizontal walls</param>
    /// <param name="width"> width of the grids</param>
    /// <param name="height"> height of the grids</param>
    public void SetWalls(WallSegment[,] wallsX, WallSegment[,] wallsZ, int width, int height)
    {
        if(z > 0)
            bottomWall = wallsZ[x, z];
        if(z < height - 1)
            topWall = wallsZ[x, z + 1];
        if (x > 0)
            leftWall = wallsX[x, z];
        if (x < width - 1)
            rightWall = wallsX[x + 1, z];
    }

    /// <summary>
    /// Get the neighbours that are not blocked by a wall
    /// </summary>
    /// <returns></returns>
    public List<PathNode> GetAvailableNeighbours()
    {
        List<PathNode> neighbours = new List<PathNode>();
        if (topNode != null && (topWall == null || topWall.GetWallState() == WallAutomata.WallState.Empty))
            neighbours.Add(topNode);
        if (bottomNode != null && (bottomWall == null || bottomWall.GetWallState() == WallAutomata.WallState.Empty))
            neighbours.Add(bottomNode);
        if (rightNode != null && (rightWall == null || rightWall.GetWallState() == WallAutomata.WallState.Empty))
            neighbours.Add(rightNode);
        if (leftNode != null && (leftWall == null || leftWall.GetWallState() == WallAutomata.WallState.Empty))
            neighbours.Add(leftNode);


        // ___|___|_x_ 
        // ___|_x_|___
        //    |   |   
   
        if (topRightNode != null && 
            (topRightNode.leftWall == null || topRightNode.leftWall.GetWallState() == WallAutomata.WallState.Empty) && (topRightNode.bottomWall == null || topRightNode.bottomWall.GetWallState() == WallAutomata.WallState.Empty) &&
            (rightWall == null || rightWall.GetWallState() == WallAutomata.WallState.Empty) && (topWall == null || topWall.GetWallState() == WallAutomata.WallState.Empty))
        {
            neighbours.Add(topRightNode);
        }

        // _x_|___|___ 
        // ___|_x_|___
        //    |   |   

        if (topLeftNode != null &&
            (topLeftNode.rightWall == null || topLeftNode.rightWall.GetWallState() == WallAutomata.WallState.Empty) && (topLeftNode.bottomWall == null || topLeftNode.bottomWall.GetWallState() == WallAutomata.WallState.Empty) &&
            (leftWall == null || leftWall.GetWallState() == WallAutomata.WallState.Empty) && (topWall == null || topWall.GetWallState() == WallAutomata.WallState.Empty))
        {
            neighbours.Add(topLeftNode);
        }


        // ___|___|___ 
        // ___|_x_|___
        //    |   | x  
        if (bottomRightNode != null &&
            (bottomRightNode.leftWall == null || bottomRightNode.leftWall.GetWallState() == WallAutomata.WallState.Empty) && (bottomRightNode.topWall == null || bottomRightNode.topWall.GetWallState() == WallAutomata.WallState.Empty) &&
            (rightWall == null || rightWall.GetWallState() == WallAutomata.WallState.Empty) && (bottomWall == null || bottomWall.GetWallState() == WallAutomata.WallState.Empty))
        {
            neighbours.Add(bottomRightNode);
        }

        // ___|___|___ 
        // ___|_x_|___
        //  x |   |    
        if (bottomLeftNode != null && 
            (bottomLeftNode.rightWall == null || bottomLeftNode.rightWall.GetWallState() == WallAutomata.WallState.Empty) && (bottomLeftNode.topWall == null || bottomLeftNode.topWall.GetWallState() == WallAutomata.WallState.Empty) &&
            (leftWall == null || leftWall.GetWallState() == WallAutomata.WallState.Empty) && (bottomWall == null || bottomWall.GetWallState() == WallAutomata.WallState.Empty))
        {
            neighbours.Add(bottomLeftNode);
        }

        return neighbours;
    }

    public void ResetNode()
    {
        gCost = int.MaxValue;
        CalculateFCost();
        parentNode = null;
    }
}
