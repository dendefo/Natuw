using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

abstract public class Creature : MonoBehaviour
{
    [SerializeField] float MoveVelocity;
    [SerializeField] float JumpVelocity;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected SpriteRenderer SRenderer;
    [SerializeField] bool isTouchingFloor;

    [SerializeField] protected bool isInDash = false;
    [SerializeField] protected float DashSpeed;
    [SerializeField] protected float DashTime;
    private float DashTimer;
    protected bool isDashReady = false;
    virtual protected void FixedUpdate()
    {
        CalculateJump();
        if (isInDash)
        {
            rb.velocity = Vector2.right * DashSpeed * (SRenderer.flipX ? -1 : 1);

        }
        if (isInDash && Time.time - DashTimer > DashTime)
        {
            isInDash = false;
            rb.velocity = Vector2.zero;
        }
    }

    protected void Stop()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }
    protected void Jump()
    {
        if (!isTouchingFloor) return;
        rb.velocity = new Vector2(rb.velocity.x, JumpVelocity);

    }
    protected void CalculateJump()
    {
        if (rb.velocity.y == 0) isTouchingFloor = true;
        else isTouchingFloor = false;
        if (isTouchingFloor && !isInDash) isDashReady = true;
    }
    protected float CalculateJumpHeight() => (-Mathf.Pow(JumpVelocity, 2)) / (2 *Physics2D.gravity.y*rb.gravityScale);
    protected void Dash()
    {
        DashTimer = Time.time;
        isInDash = true;
        isDashReady = false;

    }
    protected void Move(bool isRight)
    {
        rb.velocity = new Vector2(MoveVelocity * (isRight ? 1 : -1), rb.velocity.y);
        SRenderer.flipX = !isRight;
    }
    /// <summary>
    /// Pathfinding algorithm for enemies
    /// </summary>
    /// <param name="level"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public Node Pathfinder(Tilemap level, Vector2Int target)
    {
        var unitPos = (Vector2Int)(level.WorldToCell(transform.position));
        List<Node> reachable = new()
            {
                new Node(unitPos,null,Vector2Int.Distance(unitPos,target))
            }; //Adding List of Nodes that will be used in algorithm as Nodes that actor is possibly can get to and adding starting node (Of Actor)

        List<Node> visited = new List<Node>(); //List of Nodes that we already checked


        while (reachable.Count != 0) //If reachable is 0 and algothm didn't made return, enemy can't reach the player
        {
            Node node = reachable[0];

            foreach (Node node1 in reachable)
            {
                if (node1.Distance < node.Distance) node = node1; //Taking Node with less distance to the target
            }

            if (node.Coor == target) //If node's coor-s are same to player's, than we found a path and it's the shortest one
            {

                return node;
                //node = Node.BuildPath(node); //Algorith that gives us next node to move to
                //CollisionLogic.CollisionCheck(level, node.Coor - Coor, this); //Moving the enemy
            }

            visited.Add(node); //Already been there, don't need to check nodes twice
            reachable.Remove(node);

            Tile.ColliderType entity;
            foreach (int y in new List<int> { -1, 0, 1 }) //For y axis
            {
                foreach (int x in new List<int> { -1, 0, 1 }) //For X axis
                {
                    Vector2Int newCoords = new(x, y);
                    newCoords += node.Coor;
                    if (y * y == x * x || visited.Exists(n => n.Coor == newCoords)) continue; //If it is diagonal or 0,0 OR if it is already been checked
                                                                                              //It leaves only four directions (0,1)(0,-1)(1,0)(-1,0). No diagonal moving

                    entity = level.GetColliderType((Vector3Int)newCoords); //Looking what's on the Square



                    if (entity != Tile.ColliderType.None) continue; //If it's not walkable, then continue

                    Node adjacent = new(newCoords, node, Vector2Int.Distance(newCoords, target)); //Create new node to check
                    if (reachable.Exists(n => n.Coor == adjacent.Coor)) continue; //If it's already awaits for check then continue

                    reachable.Add(adjacent); // Add to List
                }
            }

        }

        return null; //There is no path
    }
}
public class Node //Representation of map as graph of walkable Nodes, that starts at Actor's node. We search for Target node at that tree
{
    public Vector2Int Coor; //Coordinates of Node
    public Node previous; //Node that linked us to this one. 
    public float Distance;
    public Node(Vector2Int coor, Node node, float distance)
    {
        previous = node;
        Coor = coor;
        Distance = distance;
    }
    static public List<Node> BuildPath(Node to_node) //Gives node that is next to enemy, that he need to move to
    {
        if (to_node == null) return null; //If there is no path
        List<Node> path = new();
        while (to_node != null)
        {
            path.Add(to_node);
            to_node = to_node.previous;
        } //Making a list of nodes that starts at player Node and ends on Enemy node. It is a shortest way
        return path;
    }
}
