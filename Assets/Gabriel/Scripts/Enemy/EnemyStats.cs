using System.Collections;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float health;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;
    private Rigidbody2D rb;
    private bool isKnockedBack = false;

    [Header("Flash Settings")]
    [SerializeField] private float flashDuration = 0.15f;
    [SerializeField, Range(0, 1)] private float flashStrength = 0.8f;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private Material flashMaterial;
    private Material defaultMaterial;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            defaultMaterial = spriteRenderer.material;
        }
    }

    public void TakeDamage(float damage, Transform attackerTransform)
    {
        Debug.Log("Enemy took " + damage + " damage.");
        health -= damage;

        if (health > 0)
        {
            StartCoroutine(HitEffect(attackerTransform));
        }
        else
        {
            Die();
        }
    }

    private IEnumerator HitEffect(Transform attackerTransform)
    {
        StartCoroutine(FlashEffect());

        if (rb != null && !isKnockedBack)
        {
            StartCoroutine(KnockbackEffect(attackerTransform));
        }

        yield return null;
    }

    private IEnumerator FlashEffect()
    {
        if (spriteRenderer != null && flashMaterial != null)
        {
            spriteRenderer.material = flashMaterial;
            flashMaterial.SetColor("_FlashColor", flashColor);
            flashMaterial.SetFloat("_FlashAmount", flashStrength);
        }

        yield return new WaitForSeconds(flashDuration);

        if (spriteRenderer != null && defaultMaterial != null)
        {
            spriteRenderer.material = defaultMaterial;
        }
    }

    private IEnumerator KnockbackEffect(Transform attackerTransform)
    {
        isKnockedBack = true;

        Vector2 knockbackDirection = (transform.position - attackerTransform.position).normalized;
        knockbackDirection.y = 0.3f;
        
        rb.linearVelocity = knockbackDirection * knockbackForce;

        yield return new WaitForSeconds(knockbackDuration);

        isKnockedBack = false;
    }

    private void Die()
    {
        Destroy(transform.root.gameObject);
    }
}

