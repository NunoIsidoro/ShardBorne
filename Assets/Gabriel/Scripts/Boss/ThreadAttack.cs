using UnityEngine;

public class ThreadAttack : MonoBehaviour
{
    [Header("Stretch Settings")]
    [SerializeField] private float stretchDuration;
    [SerializeField] private float finalHeight;
    [SerializeField] private float activeTimeAfterStretch;

    [Header("Damage Settings")]
    [SerializeField] private float threadDamage;
    [SerializeField] private Vector2 knockbackForce;
    [SerializeField] private float knockbackDuration;
    private Vector3 initialScale;
    private Vector3 targetScale;
    private float timer;
    private bool stretching = true;

    private void Start()
    {
        initialScale = transform.localScale;
        targetScale = new Vector3(initialScale.x, finalHeight, initialScale.z);
        timer = 0f;
    }

    private void Update()
    {
        if (stretching)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / stretchDuration);
            transform.localScale = Vector3.Lerp(initialScale, targetScale, t);

            if (t >= 1f)
            {
                stretching = false;
                Invoke(nameof(DestroyThread), activeTimeAfterStretch);
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        KnockBackAbility knockback = collision.GetComponentInParent<KnockBackAbility>();
        knockback.StartKnockBack(knockbackDuration, knockbackForce, transform);
        collision.GetComponent<PlayerStats>().DamagePlayer(threadDamage);
    }

    private void DestroyThread()
    {
        Destroy(gameObject);
    }
}
