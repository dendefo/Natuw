using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Creature
{
    List<Node> path;
    int UpdateRates = 0;
    private void Start()
    {
        SceneManager.Instance.EnemyList.Add(this);
    }
    protected override void FixedUpdate()
    {
        if (UpdateRates % ((1/Time.fixedDeltaTime)+ SceneManager.Instance.EnemyList.IndexOf(this)) == 1)
        {
            CalculatePath();
            UpdateRates = 0;
        }
        base.FixedUpdate();

        EnemyMovement();
        UpdateRates++;

       // PlayAnimation("EnemySpeed");//, "EnemyJumpSpeed"
    }
    private void CalculatePath()
    {
        if (Vector3.Distance(SceneManager.Instance.Player.transform.position, transform.position) > 25) { path = null; return; }
        path = Node.BuildPath(Pathfinder(SceneManager.Instance.TileMap, (Vector2Int)SceneManager.Instance.TileMap.WorldToCell(SceneManager.Instance.Player.gameObject.transform.position)));

    }
    private void EnemyMovement()
    {
        Vector2Int direction;
        if (path == null) return;
        path.Reverse();
        for (int i = 1; i < path.Count; i++)
        {
            direction = path[i].Coor - path[i - 1].Coor;
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
            Gizmos.DrawLine(SceneManager.Instance.TileMap.CellToWorld((Vector3Int)path[i].Coor), SceneManager.Instance.TileMap.CellToWorld((Vector3Int)path[i - 1].Coor));
        }
    }

}
