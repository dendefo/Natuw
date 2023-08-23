using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Creature
{
    List<Node> path;
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        CalculatePath();
        EnemyMovement();
    }
    private void CalculatePath()
    {
        path = Node.BuildPath(Pathfinder(SceneManager.Instance.GameGrid, (Vector2Int)SceneManager.Instance.GameGrid.WorldToCell(SceneManager.Instance.Player.gameObject.transform.position)));

    }
    private void EnemyMovement()
    {
        Vector2Int direction;
        path.Reverse();
        for (int i = 1; i < path.Count; i++)
        {
            direction = path[i].Coor - path[i-1].Coor;
            if (i > 2 && direction.y != -1)
            {
                if (direction.x != -1) Move(false);
                else Move(true);
                return;
            }
            if (direction.x == -1) Move(false);
            else if (direction.x == 1) Move(true);
            else if (direction.y == 1) Jump();
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
