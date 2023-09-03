using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Creature
{
    private List<Node> _path;
    private int _updateRates = 1; //Counter of FixedUpdates for better AI performance
    #region UnityFunctions
    private void Start()
    {
        SceneManager.Instance.EnemyList.Add(this);
    }
    protected override void FixedUpdate()
    {
        if ((int)((1 / Time.fixedDeltaTime) + SceneManager.Instance.EnemyList.IndexOf(this)) % _updateRates == 1)
        {
            CalculatePath();
            _updateRates = 1;
        }
        base.FixedUpdate();

        EnemyMovement();
        _updateRates++;

        PlayAnimation("EnemySpeed");//, "EnemyJumpSpeed"
    }
    override protected void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (_path == null) return;
        for (int i = 1; i < _path.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(SceneManager.Instance.TileMap.CellToWorld((Vector3Int)_path[i].Coor), SceneManager.Instance.TileMap.CellToWorld((Vector3Int)_path[i - 1].Coor));
        }
    }
    #endregion
    #region Movement

    private void CalculatePath()
    {
        if (Vector3.Distance(SceneManager.Instance.Player.transform.position, transform.position) > 25) { _path = null; return; }
        _path = Node.BuildPath(Pathfinder(SceneManager.Instance.TileMap, (Vector2Int)(SceneManager.Instance.TileMap.WorldToCell(SceneManager.Instance.Player.gameObject.transform.position) + Vector3Int.up)));
    }
    private void EnemyMovement()
    {
        Vector2Int direction;
        if (_path == null) return;
        for (int i = _path.Count - 1; i > 1; i--)
        {
            direction = _path[i - 1].Coor - _path[i].Coor;
            if (i < _path.Count - 3 && direction.y != -1)
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
    #endregion
    #region Battle
    protected override void Die()
    {
        SceneManager.Instance.EnemyList.Remove(this);
        base.Die();
    }
    #endregion
}