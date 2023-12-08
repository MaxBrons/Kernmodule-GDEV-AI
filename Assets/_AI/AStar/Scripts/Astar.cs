using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Astar
{
    /// <summary>
    /// Returns a list of Vector2Int positions which describes a path from the startPos to the endPos.
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        if (grid.Length < 1)
            return null;

        List<Node> openList = new();
        List<Node> closedList = new();

        // Initialize the openlist with the startpos.
        openList.Add(new Node(startPos, null, 0, 0));

        // While the open list is not empty...
        while (openList.Count > 0) {
            int lowestScoreIndex = GetLowestFScoreInOpenList(openList);

            // Pop it from the open list.
            Node parentNode = openList[lowestScoreIndex];
            openList.RemoveAt(lowestScoreIndex);

            Cell currentCel = GetCellFromNodePosition(grid, parentNode);
            List<Node> successors = GetSuccessorNodes(grid, parentNode, currentCel);

            foreach (var successor in successors) {
                // If the successor is the endposition, return the path.
                if (successor.position == endPos)
                    return CalculatePath(successor);

                // Compute the g + h values.
                successor.GScore = parentNode.GScore + Vector2Int.Distance(currentCel.gridPosition, successor.position);
                successor.HScore = Mathf.Abs(Vector2Int.Distance(currentCel.gridPosition, endPos));

                // Continue if the successor is already in the open list.
                if (openList.Find(x => x.position == successor.position) != null)
                    continue;

                // Continue if the successor is already in the closed list
                // and has a lower F score.
                if (closedList.Find(x => x.position == successor.position && x.FScore < successor.FScore) != null)
                    continue;

                openList.Add(successor);
            }
            closedList.Add(parentNode);
        }

        return null;
    }

    /// <summary>
    /// Returns a valid A* path.
    /// </summary>
    /// <param name="successor"></param>
    /// <returns></returns>
    private List<Vector2Int> CalculatePath(Node successor)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node parentRoot = successor;

        path.Add(parentRoot.position);

        // Retrace the path from end to beginning and
        // return the inverse of the list as the path.
        while (parentRoot.parent != null) {
            parentRoot = parentRoot.parent;
            path.Add(parentRoot.position);
        }

        path.Reverse();
        return path;
    }

    /// <summary>
    /// Return a list of possible nodes to add to the A* path result.
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="parentNode"></param>
    /// <param name="currentCel"></param>
    /// <returns></returns>
    private List<Node> GetSuccessorNodes(Cell[,] grid, Node parentNode, Cell currentCel)
    {
        var neighbours = currentCel.GetNeighbours(grid);
        var successors = neighbours.Where(x => {

            // Get the direction to the neighbouring cell (as a wall, just for cardinal directions)
            Wall direction = new Wall();
            Vector2Int dir = currentCel.gridPosition - x.gridPosition;

            if (dir.x != 0)
                direction = dir.x < 0 ? Wall.LEFT : Wall.RIGHT;
            if (dir.y != 0)
                direction = dir.y < 0 ? Wall.DOWN : Wall.UP;

            // Check if the neighbouring cell and the current cell have similar walls.
            // If so, do not add this cell to the path.
            if ((x.walls & direction) != 0)
                return false;

            // Otherwise, do add it to the path.
            return true;

        });

        // Convert the list to nodes.
        return successors.ToList().ConvertAll(x => new Node(x.gridPosition, parentNode, 0, 0));
    }

    /// <summary>
    /// Return the index of the lowest scoring node in the open list.
    /// </summary>
    /// <param name="openList"></param>
    /// <returns></returns>
    private int GetLowestFScoreInOpenList(List<Node> openList)
    {
        int lowestScoreIndex = 0;

        // Find the node with the lowest cost in the open list.
        openList.ForEach(x => {
            if (x.FScore < openList[lowestScoreIndex].FScore)
                lowestScoreIndex = openList.IndexOf(x);
        });
        return lowestScoreIndex;
    }

    /// <summary>
    /// Return the cell in the grid, based on a given node.
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    private Cell GetCellFromNodePosition(Cell[,] grid, Node other)
    {
        foreach (var cell in grid) {
            if (cell.gridPosition == other.position) {
                return cell;
            }
        }
        return null;
    }
}
