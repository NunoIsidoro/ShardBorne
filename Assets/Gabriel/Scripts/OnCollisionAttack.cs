using UnityEngine;

public class OnCollisionAttack : MonoBehaviour
{
    [SerializeField] private float damage;

    [Header("KnockBack Settings")]
    [SerializeField] private float knockBackDuration;
    [SerializeField] private Vector2 knockBackForce;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Tenta obter o Knockback (no objeto ou nos pais)
        KnockBackAbility knockBack = collision.GetComponentInParent<KnockBackAbility>();

        // Só executa se encontrou o componente
        if (knockBack != null)
        {
            knockBack.StartKnockBack(knockBackDuration, knockBackForce, transform);
        }

        // Tenta obter os Stats (no objeto ou nos pais, para garantir)
        PlayerStats stats = collision.GetComponent<PlayerStats>();
        if (stats == null) stats = collision.GetComponentInParent<PlayerStats>();

        // Só dá dano se encontrou o componente
        if (stats != null)
        {
            stats.DamagePlayer(damage);
        }
    }
}