using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static RangedWeapon;

abstract public class Creature : MonoBehaviour
{
    #region Fields

    [Header("Battle")]
    [SerializeField] public RangedWeapon weapon;
    [SerializeField] public CreatureAttributes Attributes;

    [Header("Components")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected SpriteRenderer SRenderer;
    [SerializeField] Animator animator;

    private float DashTimer;
    private float LastShotTime;
    private Creature Target;
    [SerializeField] private bool isTouchingFloor;
    protected bool isDashReady = false;
    protected bool isInDash = false;
    MovingPlatform _currentConnectedPlatform = null;
    PassingThroughPlatform _currentPassingThroughPlatform = null;
    private bool SecondJumpReady = false;
    private bool SecondJumpAvalible = false;
    [SerializeField] private float currentJumpMaxHeightPosition;
    [SerializeField] private bool isInJump;

    #endregion
    #region UnityFunctions
    private void OnLevelWasLoaded(int level)
    {
        LastShotTime = 0;
    }
    virtual protected void Update()
    {
        if (weapon == null) return;
        if (LevelManager.Instance.inGameTimer - LastShotTime >= Attributes.AttackSpeed)
        {
            weapon.Shoot(Attributes.CalculateProjectileDamage(), Attributes.BulletFlightSpeed);
            LastShotTime = LevelManager.Instance.inGameTimer;
        }
    }
    virtual protected void FixedUpdate()
    {
        CalculateJump();
        if (isInDash)
        {
            rb.velocity = Vector2.right * Attributes.DashSpeed * (SRenderer.flipX ? -1 : 1) + (_currentConnectedPlatform == null ? Vector2.zero : _currentConnectedPlatform.rb.velocity);

        }
        if (isInDash && Time.time - DashTimer > Attributes.DashTime)
        {
            isInDash = false;
            rb.velocity = Vector2.zero;
        }
        if (weapon != null)
        {

            Target = weapon.ChoseTarget(LevelManager.Instance.Player == this);
            Aim(Target);
        }
    }
    virtual protected void OnDrawGizmos()
    {
        if (weapon == null) return;
        if (Target == null) return;
        Gizmos.color = UnityEngine.Color.blue;
        Gizmos.DrawLine(Target.transform.position, weapon.transform.position);
        Gizmos.DrawWireSphere(Target.transform.position, 1);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "TileMap") //Collision with a map
        {
            foreach (var contact in collision.contacts)
            {
                if (isTouchingFloor) break;
                if (contact.normal.y >= 0.707) { isTouchingFloor = true; SecondJumpReady = true; }
                //This number is SqrRoot(2)/2 it means that contact is counted only if happened between 45 and 125 degrees
            }
        }
        if (collision.collider.tag == "Platform")
        {
            if (collision.contacts[0].normal.y >= 0.707)
            {
                isTouchingFloor = true;
                SecondJumpReady = true;
                collision.collider.TryGetComponent<MovingPlatform>(out _currentConnectedPlatform);

            }
            collision.collider.TryGetComponent<PassingThroughPlatform>(out _currentPassingThroughPlatform);

        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "TileMap") //Collision with a map
        {
            isTouchingFloor = false;
            foreach (var contact in collision.contacts)
            {
                if (isTouchingFloor) return;
                if (contact.normal.y >= 0.707) { SecondJumpReady = true; isTouchingFloor = true; }
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "TileMap") isTouchingFloor = false; //Collision with a map 
        if (collision.collider.tag == "Platform")
        {
            //Here could be possible bug, but i don't care too much right now
            isTouchingFloor = false;
            _currentConnectedPlatform = null;
            if (((_currentPassingThroughPlatform.effector.colliderMask >> 3) & 1) == 1) _currentPassingThroughPlatform = null;
        }
    }
    #endregion
    #region BattleFunctions
    virtual public void Aim(Creature target)
    {
        if (target == null) return;
        void Rotate(Transform toRotate, Vector3 toMove)
        {
            var norm = Vector3.Normalize(toRotate.position - toMove);
            var Acos = Mathf.Acos(norm.y);
            var z = Acos / Mathf.PI * (toRotate.position.x > toMove.x ? -180 : 180);

            toRotate.localEulerAngles = new Vector3(0, 0, z - 90);
            weapon.WeaponSprite.flipY = Mathf.Abs(z - 90) > 90;
        }
        Rotate(weapon.transform, target.transform.position);


    }
    public void GetDamage(float _damage)
    {
        Attributes.GetDamage(_damage);
        if (Attributes.HP <= 0) Die();
    }
    virtual protected void Die()
    {
        Destroy(gameObject);
    }
    #endregion
    #region Movement 
    protected void Stop()
    {
        rb.velocity = new Vector2(0, rb.velocity.y) + (_currentConnectedPlatform == null ? Vector2.zero : _currentConnectedPlatform.rb.velocity);
    }
    protected void StartJump()
    {
        if (isTouchingFloor || (SecondJumpAvalible && SecondJumpReady))
        {
            SecondJumpReady = false;
            isInJump = true;
            currentJumpMaxHeightPosition = transform.position.y + CalculateJumpHeight();
        }
        else isInJump = false;
    }
    protected void Jump()
    {
        if (!isInJump) return;
        if (currentJumpMaxHeightPosition < transform.position.y) { EndJump(); }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, Attributes.JumpVelocity);
            isTouchingFloor = false;
        }


    }
    protected void EndJump()
    {
        currentJumpMaxHeightPosition = -100000000;
        isInJump = false;
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Sqrt(Mathf.Abs(rb.velocity.y))*((rb.velocity.y>0)?1:-1));
    }
    protected void CalculateJump()
    {
        //if (rb.velocity.y == 0) isTouchingFloor = true;
        //else isTouchingFloor = false;
        if (isTouchingFloor && !isInDash) isDashReady = true;
    }
    protected float CalculateJumpHeight() => (-Mathf.Pow(Attributes.JumpVelocity, 2)) / (2 * Physics2D.gravity.y * rb.gravityScale);
    protected void Dash()
    {
        DashTimer = Time.time;
        isInDash = true;
        isDashReady = false;

    }
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

    protected void PlayAnimation(string speedParameter = null, string jumpParameter = null)
    {
        if (speedParameter != null) animator.SetFloat(speedParameter, Mathf.Abs((rb.velocity - (_currentConnectedPlatform == null ? Vector2.zero : _currentConnectedPlatform.rb.velocity)).x));
        if (jumpParameter != null) animator.SetFloat(jumpParameter, Mathf.Abs((rb.velocity - (_currentConnectedPlatform == null ? Vector2.zero : _currentConnectedPlatform.rb.velocity)).y));
    }
    #endregion
    #region Misc
    public float Distance(Creature creature) => Vector3.Distance(transform.position, creature.transform.position);

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

    public struct PlayerSaveData
    {
        public WeaponSaveData weapon;
        public CreatureAttributes attributes;
    }
    public PlayerSaveData GetPlayerData()
    {
        var data = new PlayerSaveData();
        data.weapon = weapon.GetSaveData();
        data.attributes = Attributes;
        return data;
    }
    public void LoadPlayerData(PlayerSaveData data)
    {
        weapon.LoadSaveData(data.weapon);
        Attributes = data.attributes;
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

[System.Serializable]
public struct CreatureAttributes
{
    [Min(0)] public float MaxHP;
    [Min(0)] public float HP;
    [Min(0)] public float DMG;
    [Min(0)] public float AttackSpeed;
    [Min(0)] public float BulletFlightSpeed;
    [Range(0, 1)] public float CritChance;
    [Min(0)] public float CritDamageMultiplier;
    [Header("Movement")]
    public float MoveVelocity;
    public float JumpVelocity;
    public float DashTime;
    public float DashSpeed;

    public float CalculateProjectileDamage()
    {
        var rand = Random.Range(0, 1.0f);
        if (rand <= CritChance) return DMG * CritDamageMultiplier;
        else return DMG;
    }
    public void GetDamage(float incomingDamage)
    {
        if (incomingDamage < 0) return;
        HP -= incomingDamage;
        if (HP < 0) HP = 0;
    }

    public void UpgradeMaxHealth()
    {
        MaxHP *= 1.5f;
        HP *= 1.5f;
    }
    public void UpgradeMovementSpeed()
    {
        MoveVelocity *= 1.1f;
    }

    public void AttackSpeedUpgrade()
    {
        AttackSpeed /= 1.25f;
    }
    public void DMGUpgrade()
    {
        DMG *= 1.25f;
    }

}