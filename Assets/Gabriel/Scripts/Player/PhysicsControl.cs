using UnityEngine;

public class PhysicsControl : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteSetTime;
    public float coyoteTimer;

    [Header("Ground")]
    [SerializeField] private float groundRayDistance;
    [SerializeField] private Transform leftGroundPoint;
    [SerializeField] private Transform rightGroundPoint;
    [SerializeField] private LayerMask whatToDetect;
    public bool grounded;
    private RaycastHit2D hitInfoLeft;
    private RaycastHit2D hitInfoRight;

    [Header("Wall")]
    [SerializeField] private float wallRayDistance;
    [SerializeField] private Transform wallCheckPointUper;
    [SerializeField] private Transform wallCheckPointLower;
    public bool wallDetected;
    private RaycastHit2D hitInfoWallUper;
    private RaycastHit2D hitInfoWallLower;

    private float gravityValue;

    public float GetGravity() { return gravityValue; }

    void Start()
    {
        gravityValue = rb.gravityScale;
    }
    private bool CheckWall()
    {
        hitInfoWallUper = Physics2D.Raycast(wallCheckPointUper.position, transform.right, wallRayDistance, whatToDetect);
        hitInfoWallLower = Physics2D.Raycast(wallCheckPointLower.position, transform.right, wallRayDistance, whatToDetect);
        Debug.DrawRay(wallCheckPointUper.position, transform.right * wallRayDistance, Color.blue);
        Debug.DrawRay(wallCheckPointLower.position, transform.right * wallRayDistance, Color.blue);
        if (hitInfoWallUper || hitInfoWallLower)
        {
            return true;
        }
        return false;

    }

    private bool CheckGround()
    {
        hitInfoLeft = Physics2D.Raycast(leftGroundPoint.position, Vector2.down, groundRayDistance, whatToDetect);
        hitInfoRight = Physics2D.Raycast(rightGroundPoint.position, Vector2.down, groundRayDistance, whatToDetect);

        Debug.DrawRay(leftGroundPoint.position, Vector2.down * groundRayDistance, Color.red);
        Debug.DrawRay(rightGroundPoint.position, Vector2.down * groundRayDistance, Color.red);

        if (hitInfoLeft || hitInfoRight)
        {
            return true;
        }
        return false;
    }
    public void DisableGravity()
    {
        rb.gravityScale = 0;
    }

    public void EnableGravity()
    {
        rb.gravityScale = gravityValue;
    }

    public void ResetVelocity()
    {
        rb.linearVelocity = Vector2.zero;
    }

    // Update is called once per frame
    public void Update()
    {
        if (!grounded)
        {
            coyoteTimer -= Time.deltaTime;
        }
        else
        {
            coyoteTimer = coyoteSetTime;
        }
    }

    private void FixedUpdate()
    {
        grounded = CheckGround();
        wallDetected = CheckWall();
    }
}
