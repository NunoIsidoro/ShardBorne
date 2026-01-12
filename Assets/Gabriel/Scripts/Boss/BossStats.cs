using System.Collections;
using UnityEngine;

public class BossStats : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float health = 100f;

    private float maxHealth;
    private bool isInvulnerable = false;

    [Header("Flash Settings")]
    [SerializeField] private float flashDuration = 0.15f;
    [SerializeField, Range(0, 1)] private float flashStrength = 0.8f;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private Material flashMaterial;
    private Material defaultMaterial;
    private SpriteRenderer spriteRenderer;

    public float CurrentHealth => health;
    public float MaxHealth => maxHealth;

    private void Start()
    {
        maxHealth = health;
        
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            defaultMaterial = spriteRenderer.material;
        }
    }

    public void SetInvulnerable(bool state)
    {
        isInvulnerable = state;
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;

        health -= damage;
        Debug.Log($"Boss took {damage} damage. HP: {health}/{maxHealth}");

        if (health > 0)
        {
            StartCoroutine(FlashEffect());
        }
        else
        {
            Die();
        }
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

    private void Die()
    {
        Debug.Log("Boss Died");

        var sm = GetComponent<BossStateMachine>();
        if (sm != null)
        {
            sm.ChangeBossState(BossStateMachine.BossState.Death);
        }
        else
        {
            Destroy(transform.root.gameObject);
        }
    }
}