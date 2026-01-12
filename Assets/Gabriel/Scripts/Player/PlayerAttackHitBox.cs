using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damage;

    [Header("Player Knockback Settings")]
    [SerializeField] private float playerKnockbackForce = 2f;
    [SerializeField] private float playerKnockbackDuration = 0.1f;

    [Header("Audio")]
    [Tooltip("Som quando acerta num inimigo (Impacto)")]
    [SerializeField] private AudioClip hitEnemySound;
    private AudioSource audioSource;

    private Player player;
    private Rigidbody2D playerRb;

    private void Start()
    {
        audioSource = GetComponentInParent<AudioSource>();
        player = GetComponentInParent<Player>();
        
        if (player != null)
        {
            playerRb = player.physicsControl.rb;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool hitSomething = false;

        EnemyStats enemy = collision.GetComponent<EnemyStats>();
        if (enemy != null)
        {
            Debug.Log("Hit enemy for " + damage + " damage.");
            enemy.TakeDamage(damage, transform);
            hitSomething = true;
            ApplyPlayerKnockback(collision.transform);
        }

        BossStats boss = collision.GetComponent<BossStats>();
        if (boss != null)
        {
            Debug.Log("Hit boss for " + damage + " damage.");
            boss.TakeDamage(damage);
            hitSomething = true;
            ApplyPlayerKnockback(collision.transform);
        }

        if (hitSomething && audioSource != null && hitEnemySound != null)
        {
            audioSource.PlayOneShot(hitEnemySound);
        }
    }

    private void ApplyPlayerKnockback(Transform enemyTransform)
    {
        if (playerRb == null || player == null) return;

        Vector2 knockbackDirection = (transform.position - enemyTransform.position).normalized;
        knockbackDirection.y = 0;
        
        playerRb.linearVelocity = new Vector2(
            knockbackDirection.x * playerKnockbackForce,
            playerRb.linearVelocity.y
        );
    }
}