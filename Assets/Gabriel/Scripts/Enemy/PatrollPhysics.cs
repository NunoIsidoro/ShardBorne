using UnityEngine;

public class PatrollPhysics : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Detected Ground and Walls")]
    [SerializeField] private float checkRadius;
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask whatToDetect;
    public bool groundDetected;
    public bool wallDetected;

    [Header("Colliders")]
    [SerializeField] private BoxCollider2D attackDetectionCol;
    [SerializeField] private PolygonCollider2D attackCol;
    public bool inAttackRange;
    private void FixedUpdate()
    {
        groundDetected = Physics2D.OverlapCircle(groundCheckPoint.position, checkRadius, whatToDetect);
        wallDetected = Physics2D.OverlapCircle(wallCheckPoint.position, checkRadius, whatToDetect);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckPoint.position, checkRadius);
        Gizmos.DrawWireSphere(wallCheckPoint.position, checkRadius);
    }
    public void NegateForces()
    {
        rb.linearVelocity = Vector2.zero;
    }
    public void ActivateAttackColl()
    {
        attackCol.enabled = true;
    }
    public void DeactivateAttackColl()
    {
        attackCol.enabled = false;
    }
}
