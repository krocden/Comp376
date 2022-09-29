using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    // coordinates on the grid
    private readonly int x, y;

    public int hScore;
    public int fScore;
    public int gScore;

    public WallSegment topWall, bottomWall, rightWall, leftWall;
    public PathNode topNode, bottomNode, rightNode, leftNode;

    public PathNode(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetNeighbours(PathNode[,] grid, int width, int height)
    {
        if (y < height)
        {
            topNode = grid[x, y + 1];
        }
        
        if (y > 0)
        {
            bottomNode = grid[x, y - 1];
        }

        if (x < width)
        {
            rightNode = grid[x + 1, y];
        }

        if (x > 0)
        {
            leftNode = grid[x - 1, y];
        }
    }

    public List<PathNode> GetAvailableNeighbours()
    {
        List<PathNode> neighbours = new List<PathNode>();

        if (topWall.GetWallState() == WallAutomata.WallState.Empty)
            neighbours.Add(topNode);
        if (bottomWall.GetWallState() == WallAutomata.WallState.Empty)
            neighbours.Add(bottomNode);
        if (rightWall.GetWallState() == WallAutomata.WallState.Empty)
            neighbours.Add(rightNode);
        if (leftWall.GetWallState() == WallAutomata.WallState.Empty)
            neighbours.Add(leftNode);

        return neighbours;
    }





}
