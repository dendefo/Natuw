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
    protected bool IsTouchingFloor { get; private set; }
    protected MovingPlatform _currentConnectedPlatform = null;
    protected PassingThroughPlatform _currentPassingThroughPlatform = null;

    [SerializeField] protected ParticleSystem DustParticles;
    #endregion

    #region UnityFunctions
    protected virtual void FixedUpdate()
    {
        if (rb.velocity.y < 0) EndJump();
    }
    virtual protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("TileMap")) //Collision with a map
        {
            foreach (var contact in collision.contacts)
            {
                if (IsTouchingFloor) break;

                if (contact.normal.y >= 0.707)
                {
                    IsTouchingFloor = true;
                    SecondJumpReady = true;
                    animator.SetBool("InAir", false);
                    DustParticles.Play();
                    //DustParticles.gameObject.SetActive(true);
                }
                if (contact.normal.y <= -0.707) EndJump();
                //This number is SqrRoot(2)/2 it means that contact is counted only if happened between 45 and 135 degrees
            }
        }
        if (collision.collider.CompareTag("Platform"))
        {
            if (collision.contacts[0].normal.y >= 0.707)
            {
                animator.SetBool("InAir", false);
                DustParticles.Play();
                //DustParticles.gameObject.SetActive(true);
                rb.interpolation = RigidbodyInterpolation2D.Extrapolate;
                IsTouchingFloor = true;
                SecondJumpReady = true;
                collision.collider.TryGetComponent<MovingPlatform>(out _currentConnectedPlatform);

            }
            collision.collider.TryGetComponent<PassingThroughPlatform>(out _currentPassingThroughPlatform);

        }
    }
    virtual protected void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("TileMap")) //Collision with a map
        {
            IsTouchingFloor = false;
            foreach (var contact in collision.contacts)
            {
                if (IsTouchingFloor) return;
                if (contact.normal.y >= 0.707) { SecondJumpReady = true; IsTouchingFloor = true; }
                if (contact.normal.y <= -0.707) EndJump();
            }
        }
        if (collision.collider.CompareTag("Platform"))
        {

            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            if (_currentConnectedPlatform != null && collision.gameObject == _currentConnectedPlatform.gameObject)
            {
                var vel = collision.rigidbody.velocity - rb.velocity;
                if (vel.magnitude >= _currentConnectedPlatform.speed / 2 && vel.magnitude <= _currentConnectedPlatform.speed * 2) { Stop(); }
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        rb.freezeRotation = true;
        if (collision.collider.CompareTag("TileMap")) IsTouchingFloor = false; //Collision with a map 
        if (collision.collider.CompareTag("Platform"))
        {
            //Here could be possible bug, but i don't care too much right now
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            IsTouchingFloor = false;
            _currentConnectedPlatform = null;
            if (((_currentPassingThroughPlatform.effector.colliderMask >> 3) & 1) == 1) _currentPassingThroughPlatform = null;
        }
        DustParticles.Stop();
        //DustParticles.gameObject.SetActive(false);
    }

    #endregion

    #region Movement 
    protected void Stop()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        rb.freezeRotation = true;
        rb.velocity = new Vector2(0, rb.velocity.y) + (_currentConnectedPlatform == null ? Vector2.zero : _currentConnectedPlatform.rb.velocity);
    }
    protected void StartJump()
    {
        if (isInJump) return;
        if (IsTouchingFloor || (SecondJumpAvalible && SecondJumpReady))
        {
            rb.velocity = Vector2.right * rb.velocity;
            rb.gravityScale /= 1.5f;
            if (!IsTouchingFloor) SecondJumpReady = false;
            isInJump = true;
            rb.AddForce(Attributes.JumpVelocity * Vector2.up, ForceMode2D.Impulse);
            if (_currentConnectedPlatform != null) rb.velocity = new Vector2(rb.velocity.x - _currentConnectedPlatform.rb.velocity.x, rb.velocity.y);
        }
        else isInJump = false;
        animator.SetBool("InAir", true);
    }
    protected void EndJump()
    {
        if (!isInJump) return;
        isInJump = false;
        rb.gravityScale *= 1.5f;
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2 * (rb.velocity.y > 0 ? 1 : 2));
    }
    protected float CalculateJumpHeight() => (-Mathf.Pow(Attributes.JumpVelocity, 2)) / (2 * Physics2D.gravity.y * rb.gravityScale / 3);

    protected virtual void Move(bool isRight)
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = new Vector2(Attributes.MoveVelocity * (isRight ? 1 : -1) + (_currentConnectedPlatform == null ? Vector2.zero : _currentConnectedPlatform.rb.velocity).x, rb.velocity.y);
        if (SRenderer != null) SRenderer.flipX = !isRight;
        else transform.GetChild(0).eulerAngles = new Vector3(0, isRight ? 0 : 180, 0);
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
    public PathFindingNode Pathfinder(Tilemap level, Vector2Int target)
    {
        var unitPos = (Vector2Int)(level.WorldToCell(transform.position));
        List<PathFindingNode> reachable = new()
            {
                new PathFindingNode(unitPos,null,Vector2Int.Distance(unitPos,target),IsTouchingFloor?0:8,IsTouchingFloor)
            }; //Adding List of Nodes that will be used in algorithm as Nodes that actor is possibly can get to and adding starting node (Of Actor)

        List<PathFindingNode> visited = new(); //List of Nodes that we already checked

        int LoopCount = 0;
        while (reachable.Count != 0) //If reachable is 0 and algothm didn't made return, enemy can't reach the player
        {
            if (LoopCount > 1000) return visited[^1];
            PathFindingNode node = reachable[0];

            foreach (PathFindingNode node1 in reachable)
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

                    PathFindingNode adjacent = new(newCoords, node, Vector2Int.Distance(newCoords, target), newJumpValue, level.GetColliderType((Vector3Int)(newCoords + Vector2Int.down)) != Tile.ColliderType.None); //Create new node to check
                    if (reachable.Exists(n => n.Coor == adjacent.Coor)) continue; //If it's already awaits for check then continue


                    reachable.Add(adjacent); // Add to List
                }
            }

        }

        return null; //There is no path
    }
    #endregion
}
public class PathFindingNode //Representation of map as graph of walkable Nodes, that starts at Actor's node. We search for Target node at that tree
{
    public Vector2Int Coor; //Coordinates of Node
    public PathFindingNode previous; //Node that linked us to this one. 
    public float Distance;
    public int JumpValue;
    public bool isGround;
    public PathFindingNode(Vector2Int coor, PathFindingNode node, float distance, int jumpValue, bool isGround)
    {
        previous = node;
        Coor = coor;
        Distance = distance;
        JumpValue = jumpValue;
        this.isGround = isGround;
    }
    static public List<PathFindingNode> BuildPath(PathFindingNode to_node) //Gives node that is next to enemy, that he need to move to
    {
        if (to_node == null) return null; //If there is no path
        List<PathFindingNode> path = new();
        while (to_node != null)
        {
            path.Add(to_node);
            to_node = to_node.previous;
        } //Making a list of nodes that starts at player Node and ends on Enemy node. It is a shortest way
        return path;
    }
}