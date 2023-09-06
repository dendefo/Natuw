using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Creature
{
    private List<Node> _path;
    private int _updateRates = 1; //Counter of FixedUpdates for better AI performance
    [SerializeField] private int XpOndeath = 50;
    #region UnityFunctions
    private void Start()
    {
        LevelManager.Instance.EnemyList.Add(this);
    }
    protected override void FixedUpdate()
    {
        if ((int)((1 / Time.fixedDeltaTime) + LevelManager.Instance.EnemyList.IndexOf(this)) % _updateRates == 1)
        {
            StartCoroutine(CalculatePath());
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
            Gizmos.DrawLine(LevelManager.Instance.TileMap.CellToWorld((Vector3Int)_path[i].Coor), LevelManager.Instance.TileMap.CellToWorld((Vector3Int)_path[i - 1].Coor));
        }
    }
    private void OnDestroy()
    {
        WorldManager.Instance.PlayerXP += XpOndeath;
    }
    #endregion
    #region Movement

    private IEnumerator CalculatePath()
    {

        if (Vector3.Distance(LevelManager.Instance.Player.transform.position, transform.position) > 25) { _path = null; yield return null; }
        _path = Node.BuildPath(Pathfinder(LevelManager.Instance.TileMap, (Vector2Int)(LevelManager.Instance.TileMap.WorldToCell(LevelManager.Instance.Player.gameObject.transform.position) + Vector3Int.up)));
        yield return null;
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
            if (!_path[i - 1].isGround) Jump();
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
        LevelManager.Instance.EnemyList.Remove(this);
        base.Die();
    }
    #endregion
}