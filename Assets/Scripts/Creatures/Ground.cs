using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Ground : Creature
{
    #region Fields
    [SerializeField] private bool SecondJumpAvalible = false;
    private bool isInJump;
    private bool SecondJumpReady = false;
    protected bool isTouchingFloor { get; private set; }
    protected MovingPlatform _currentConnectedPlatform = null;
    protected PassingThroughPlatform _currentPassingThroughPlatform = null;
    #endregion

    #region UnityFunctions
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (rb.velocity.y < 0) EndJump();
    }
    virtual protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "TileMap") //Collision with a map
        {
            foreach (var contact in collision.contacts)
            {
                if (isTouchingFloor) break;

                if (contact.normal.y >= 0.707) { isTouchingFloor = true; SecondJumpReady = true; }
                if (contact.normal.y <= -0.707) EndJump();
                //This number is SqrRoot(2)/2 it means that contact is counted only if happened between 45 and 125 degrees
            }
        }
        if (collision.collider.tag == "Platform")
        {
            if (collision.contacts[0].normal.y >= 0.707)
            {
                rb.interpolation = RigidbodyInterpolation2D.Extrapolate;
                isTouchingFloor = true;
                SecondJumpReady = true;
                collision.collider.TryGetComponent<MovingPlatform>(out _currentConnectedPlatform);

            }
            collision.collider.TryGetComponent<PassingThroughPlatform>(out _currentPassingThroughPlatform);

        }
    }
    virtual protected void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "TileMap") //Collision with a map
        {
            isTouchingFloor = false;
            foreach (var contact in collision.contacts)
            {
                if (isTouchingFloor) return;
                if (contact.normal.y >= 0.707) { SecondJumpReady = true; isTouchingFloor = true; }
                if (contact.normal.y <= -0.707) EndJump();
            }
        }
        if (collision.collider.tag == "Platform")
        {
            if (_currentConnectedPlatform != null && collision.gameObject == _currentConnectedPlatform.gameObject)
            {
                var vel = collision.rigidbody.velocity - rb.velocity;
                if (vel.magnitude >= _currentConnectedPlatform.speed / 2 && vel.magnitude <= _currentConnectedPlatform.speed * 2) { Stop(); }
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "TileMap") isTouchingFloor = false; //Collision with a map 
        if (collision.collider.tag == "Platform")
        {
            //Here could be possible bug, but i don't care too much right now
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            isTouchingFloor = false;
            _currentConnectedPlatform = null;
            if (((_currentPassingThroughPlatform.effector.colliderMask >> 3) & 1) == 1) _currentPassingThroughPlatform = null;
        }
    }

    #endregion

    #region Movement 
    protected void Stop()
    {
        rb.velocity = new Vector2(0, rb.velocity.y) + (_currentConnectedPlatform == null ? Vector2.zero : _currentConnectedPlatform.rb.velocity);
    }
    protected void StartJump()
    {
        if (isInJump) return;
        if (isTouchingFloor || (SecondJumpAvalible && SecondJumpReady))
        {
            rb.gravityScale /= 1.5f;
            SecondJumpReady = false;
            isInJump = true;
            rb.AddForce(Attributes.JumpVelocity * Vector2.up, ForceMode2D.Impulse);
            if (_currentConnectedPlatform != null) rb.velocity -= _currentConnectedPlatform.rb.velocity;
        }
        else isInJump = false;
    }
    protected void EndJump()
    {
        if (!isInJump) return;
        isInJump = false;
        rb.gravityScale *= 1.5f;
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2 * (rb.velocity.y > 0 ? 1 : 2));
    }
    protected float CalculateJumpHeight() => (-Mathf.Pow(Attributes.JumpVelocity, 2)) / (2 * Physics2D.gravity.y * rb.gravityScale / 3);

    protected void Move(bool isRight)
    {
        rb.velocity = new Vector2(Attributes.MoveVelocity * (isRight ? 1 : -1), rb.velocity.y) + (_currentConnectedPlatform == null ? Vector2.zero : _currentConnectedPlatform.rb.velocity);
        SRenderer.flipX = !isRight;
    }
    public void UpgradeDoubleJump()
    {
        SecondJumpAvalible = true;
    }

    #endregion

    #region VisualAndSound
    protected override void PlayAnimation(string speedParameter = null, string jumpParameter = null)
    {
        if (speedParameter != null) animator.SetFloat(speedParameter, Mathf.Abs((rb.velocity - (_currentConnectedPlatform == null ? Vector2.zero : _currentConnectedPlatform.rb.velocity)).x));
        if (jumpParameter != null) animator.SetFloat(jumpParameter, Mathf.Abs((rb.velocity - (_currentConnectedPlatform == null ? Vector2.zero : _currentConnectedPlatform.rb.velocity)).y));
    }
    #endregion

    #region Misc
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
                new Node(unitPos,null,Vector2Int.Distance(unitPos,target),isTouchingFloor?0:8,isTouchingFloor)
            }; //Adding List of Nodes that will be used in algorithm as Nodes that actor is possibly can get to and adding starting node (Of Actor)

        List<Node> visited = new List<Node>(); //List of Nodes that we already checked

        int LoopCount = 0;
        while (reachable.Count != 0) //If reachable is 0 and algothm didn't made return, enemy can't reach the player
        {
            if (LoopCount > 1000) return visited[visited.Count - 1];
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
                    LoopCount++;
                    Vector2Int newCoords = new(x, y);

                    int newJumpValue = node.JumpValue;
                    if (newCoords.y == 1)
                    {
                        newJumpValue++;
                        if (newJumpValue % 2 == 1) newJumpValue++;
                    }
                    else if (!node.isGround && newCoords.y != 1) newJumpValue++;
                    else if (node.isGround && newCoords.y != 1) newJumpValue = 0;

                    if (newJumpValue > 7) continue;

                    newCoords += node.Coor;
                    if (y * y == x * x || visited.Exists(n => n.Coor == newCoords)) continue; //If it is diagonal or 0,0 OR if it is already been checked
                                                                                              //It leaves only four directions (0,1)(0,-1)(1,0)(-1,0). No diagonal moving

                    entity = level.GetColliderType((Vector3Int)newCoords); //Looking what's on the Square



                    if (entity != Tile.ColliderType.None) continue; //If it's not walkable, then continue

                    Node adjacent = new(newCoords, node, Vector2Int.Distance(newCoords, target), newJumpValue, level.GetColliderType((Vector3Int)(newCoords + Vector2Int.down)) != Tile.ColliderType.None); //Create new node to check
                    if (reachable.Exists(n => n.Coor == adjacent.Coor)) continue; //If it's already awaits for check then continue


                    reachable.Add(adjacent); // Add to List
                }
            }

        }

        return null; //There is no path
    }
    #endregion
}
public class Node //Representation of map as graph of walkable Nodes, that starts at Actor's node. We search for Target node at that tree
{
    public Vector2Int Coor; //Coordinates of Node
    public Node previous; //Node that linked us to this one. 
    public float Distance;
    public int JumpValue;
    public bool isGround;
    public Node(Vector2Int coor, Node node, float distance, int jumpValue, bool isGround)
    {
        previous = node;
        Coor = coor;
        Distance = distance;
        JumpValue = jumpValue;
        this.isGround = isGround;
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

