using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Creature
{
    List<Node> path;
    private void Update()
    {
        EnemyMovement();
    }

    private void EnemyMovement()
    {
        path = Node.BuildPath(Pathfinder(SceneManager.Instance.GameGrid, (Vector2Int)SceneManager.Instance.GameGrid.WorldToCell(SceneManager.Instance.Player.gameObject.transform.position)));
        Vector2Int nextNode;
        path.Reverse();
        foreach (Node node in path)
        {
            nextNode = node.Coor - (Vector2Int)SceneManager.Instance.GameGrid.WorldToCell(transform.position);
            Debug.Log(nextNode);
            if (nextNode.x == -1) Move(false);
            else if (nextNode.x == 1) Move(true);
            else if (nextNode.y == 1) Jump();
            else continue;
            return;
        }
    }
    private void OnDrawGizmos()
    {
        if (path == null) return;
        for (int i = 1; i < path.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(SceneManager.Instance.GameGrid.CellToWorld((Vector3Int)path[i].Coor), SceneManager.Instance.GameGrid.CellToWorld((Vector3Int)path[i - 1].Coor));
        }
    }

}
