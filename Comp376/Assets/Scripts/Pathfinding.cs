using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding
{
    private readonly int STRAIGHT_MOVE_COST = 10;
    private readonly int DIAG_MOVE_COST = 14;

    private PathNode[,] pathNodeGrid;

    public Pathfinding(PathNode[,] grid) 
    {
        this.pathNodeGrid = grid;
    }

    public bool TryFindPath(int startX, int startZ, int endX, int endZ)
    {
        return FindPath(startX, startZ, endX, endZ) != null;
    }

    public List<PathNode> FindPath(int startX, int startZ, int endX, int endZ)
    {
        PathNode startNode = pathNodeGrid[startX, startZ];
        PathNode endNode = pathNodeGrid[endX, endZ];

        List<PathNode> openNodes = new List<PathNode> { startNode };
        List<PathNode> closedNodes = new List<PathNode>();

        List<PathNode> solution = new List<PathNode>();

        ResetGrid();

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openNodes.Count > 0)
        {
            PathNode currentNode = openNodes.OrderBy(x => x.fCost).First();

            if (currentNode == endNode)
            {
                // found the end node, trace back with parent nodes
                return CalculatePath(endNode);
            }

            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            List<PathNode> neighbours = currentNode.GetAvailableNeighbours();

            foreach (PathNode neighbour in neighbours)
            {
                if (closedNodes.Contains(neighbour)) continue;

                int tempGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbour);

                if (tempGCost < neighbour.gCost)
                {
                    neighbour.parentNode = currentNode;
                    neighbour.gCost = tempGCost;
                    neighbour.hCost = CalculateDistanceCost(neighbour, endNode);
                    neighbour.CalculateFCost();

                    if (!openNodes.Contains(neighbour))
                        openNodes.Add(neighbour);
                }
            }
        }

        // no path found
        return null;
    }

    private List<PathNode> CalculatePath(PathNode node)
    {
        PathNode currentNode = node;
        List<PathNode> nodes = new List<PathNode> { currentNode };

        while (currentNode.parentNode != null)
        {
            nodes.Insert(0, currentNode.parentNode);
            currentNode = currentNode.parentNode;
        }

        return nodes;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = (int)Math.Abs(a.position.x - b.position.x);
        int zDistance = (int)Math.Abs(a.position.z - b.position.z);
        int straightDistanceCost = STRAIGHT_MOVE_COST * Math.Abs(xDistance - zDistance);
        int diagDistanceCost = DIAG_MOVE_COST * Math.Min(xDistance, zDistance);
        return straightDistanceCost + diagDistanceCost;
    }

    private void ResetGrid()
    {
        foreach (PathNode node in pathNodeGrid)
        {
            node.ResetNode();
        }
    }
}