using UnityEngine;
using System.Collections;

public class ThreadProjectile : MonoBehaviour
{
    public enum ThreadType { Normal, Explosive, Slow }
    private ThreadType myType = ThreadType.Normal;

    [Header("Configurações de Movimento")]
    [SerializeField] private float fallSpeed = 20f;
    [SerializeField] private float telegraphTime = 0.5f;
    [SerializeField] private float stickDuration = 3f;

    [Header("Ajustes de Impacto")]
    [SerializeField] private float surfaceOffset = 0f;
    [SerializeField] private Vector2 manualHitOffset = Vector2.zero;

    [Header("Configurações de Deteção")]
    [SerializeField] private float gracePeriod = 0.1f;

    [Header("Configurações de Dano e Knockback")]
    [SerializeField] private float damage = 1f;
    [SerializeField] private Vector2 knockbackForce = new Vector2(5f, 5f);
    [SerializeField] private float knockbackDuration = 0.2f;

    [Header("Configurações de Variantes")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color explosiveColor = Color.red;
    [SerializeField] private Color slowColor = Color.cyan;

    [Space(5)]
    [Header("Explosão")]
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float explosionDelay = 2f;
    [SerializeField] private float explosionDamage = 2f;
    [SerializeField] private GameObject explosionVFX;

    [Space(5)]
    [Header("Slow")]
    [SerializeField] private float slowMultiplier = 0.4f;
    [SerializeField] private float slowDuration = 2f;
    [SerializeField] private GameObject slowVFX; // <--- NOVO CAMPO ADICIONADO

    [Header("Referências Visuais")]
    [SerializeField] private SpriteRenderer threadSprite;
    [SerializeField] private LineRenderer telegraphLine;
    [SerializeField] private Collider2D threadCollider;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private LayerMask groundLayer;

    private bool isMoving = false;
    private bool isStuck = false;
    private bool hasDamagedPlayer = false;
    private float moveTimer = 0f;
    private bool exploded = false;

    public void SetThreadType(ThreadType type)
    {
        myType = type;
    }

    private void Start()
    {
        if (stickDuration <= 0.1f) stickDuration = 3f;
        if (groundLayer == 0) Debug.LogError("ERRO: 'Ground Layer' não definida!");

        if (threadCollider) threadCollider.enabled = false;

        if (threadSprite)
        {
            threadSprite.enabled = true;
            Color c = threadSprite.color;
            c.a = 0f;
            threadSprite.color = c;
            threadSprite.sortingOrder = 50;
        }

        if (telegraphLine)
        {
            telegraphLine.enabled = true;
            SetupTelegraphLine();
        }

        StartCoroutine(AttackSequence());
    }

    private void Update()
    {
        if (isStuck) return;

        if (isMoving)
        {
            moveTimer += Time.deltaTime;
            MoveAndCheckCollision();
        }
    }

    // --- LÓGICA DE DETEÇÃO DE JOGADOR ---
    private bool IsPlayerTarget(Collider2D col)
    {
        if (col.CompareTag("Player")) return true;

        if (col.GetComponent<PlayerStats>() != null) return true;
        if (col.GetComponentInParent<PlayerStats>() != null) return true;

        return false;
    }

    private void MoveAndCheckCollision()
    {
        float moveDistance = fallSpeed * Time.deltaTime;
        Vector3 localDown = -transform.up;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, localDown, moveDistance);
        foreach (RaycastHit2D hit in hits)
        {
            if (IsPlayerTarget(hit.collider))
            {
                HandlePlayerCollision(hit.collider);
            }
        }

        if (moveTimer < gracePeriod)
        {
            transform.Translate(Vector3.down * moveDistance);
            return;
        }

        RaycastHit2D hitGround = Physics2D.Raycast(transform.position, localDown, moveDistance, groundLayer);

        if (hitGround.collider != null)
        {
            StickToGround(hitGround.point);
        }
        else
        {
            transform.Translate(Vector3.down * moveDistance);
        }
    }

    private void SetupTelegraphLine()
    {
        if (telegraphLine == null) return;
        telegraphLine.positionCount = 2;
        telegraphLine.SetPosition(0, transform.position);

        Vector3 direction = -transform.up;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 50f, groundLayer);

        Vector3 target = hit.collider != null ? hit.point : transform.position + (direction * 20f);
        target.z = transform.position.z;
        telegraphLine.SetPosition(1, target);
    }

    private IEnumerator AttackSequence()
    {
        yield return new WaitForSeconds(telegraphTime);

        if (telegraphLine) telegraphLine.enabled = false;

        if (threadCollider)
        {
            threadCollider.enabled = true;
        }

        if (trail) trail.emitting = true;

        isMoving = true;
    }

    // --- COLISÕES E GATILHOS ---
    private void OnTriggerStay2D(Collider2D other)
    {
        if (isStuck && IsPlayerTarget(other))
        {
            TryApplySlow(other);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsPlayerTarget(other))
        {
            if (isStuck)
            {
                TryApplySlow(other);
            }
            else if (moveTimer > gracePeriod)
            {
                HandlePlayerCollision(other);
            }
        }
        else if (!isStuck && ((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            StickToGround(transform.position);
        }
    }

    private MoveAbility GetMoveAbility(Collider2D col)
    {
        MoveAbility move = col.GetComponent<MoveAbility>();
        if (move != null) return move;

        move = col.GetComponentInParent<MoveAbility>();
        if (move != null) return move;

        move = col.GetComponentInChildren<MoveAbility>();
        return move;
    }

    private void TryApplySlow(Collider2D other)
    {
        if (myType == ThreadType.Slow)
        {
            MoveAbility moveScript = GetMoveAbility(other);
            if (moveScript != null)
            {
                moveScript.ApplySlow(slowMultiplier, slowDuration);
            }
        }
    }

    private void HandlePlayerCollision(Collider2D playerCollider)
    {
        if (hasDamagedPlayer) return;

        if (myType == ThreadType.Slow)
        {
            TryApplySlow(playerCollider);
        }

        KnockBackAbility knockback = playerCollider.GetComponentInParent<KnockBackAbility>();
        if (knockback == null) knockback = playerCollider.GetComponentInChildren<KnockBackAbility>();
        if (knockback != null) knockback.StartKnockBack(knockbackDuration, knockbackForce, transform);

        PlayerStats stats = playerCollider.GetComponentInParent<PlayerStats>();
        if (stats == null) stats = playerCollider.GetComponent<PlayerStats>();
        if (stats == null) stats = playerCollider.GetComponentInChildren<PlayerStats>();

        if (stats != null)
        {
            stats.DamagePlayer(damage);
            hasDamagedPlayer = true;
        }
    }

    private void StickToGround(Vector3 hitPoint)
    {
        if (isStuck) return;
        isStuck = true;
        isMoving = false;

        Vector3 finalPos = hitPoint + (transform.up * surfaceOffset);
        finalPos += (Vector3)manualHitOffset;
        finalPos.z = transform.position.z;

        transform.position = finalPos;

        if (trail) trail.emitting = false;

        if (threadSprite)
        {
            threadSprite.enabled = true;
            threadSprite.sortingOrder = 300;

            switch (myType)
            {
                case ThreadType.Explosive:
                    threadSprite.color = explosiveColor;
                    break;

                case ThreadType.Slow:
                    threadSprite.color = slowColor;

                    // --- Instancia o VFX de Slow ---
                    if (slowVFX != null)
                    {
                        // Instancia como filho (parent = transform) para desaparecer junto com o fio
                        Instantiate(slowVFX, transform.position, Quaternion.identity, transform);
                    }
                    // -----------------------------------------------
                    break;

                default:
                    threadSprite.color = normalColor;
                    break;
            }
        }

        StartCoroutine(StickRoutine());
    }

    private IEnumerator StickRoutine()
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0f;
        while (elapsed < 0.15f)
        {
            Vector3 shake = (Vector3)Random.insideUnitCircle * 0.05f;
            transform.position = originalPos + new Vector3(shake.x, shake.y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPos;

        if (myType == ThreadType.Explosive)
        {
            float warningTime = explosionDelay;
            float blinkSpeed = 10f;
            float blinkTimer = 0f;

            while (warningTime > 0)
            {
                warningTime -= Time.deltaTime;
                blinkTimer += Time.deltaTime * blinkSpeed;
                float t = Mathf.PingPong(blinkTimer, 1f);
                threadSprite.color = Color.Lerp(explosiveColor, Color.white, t);
                yield return null;
            }

            Explode();
            yield break;
        }
        else
        {
            yield return new WaitForSeconds(stickDuration);
        }

        float fadeTime = 0.5f;
        float fadeElapsed = 0f;
        Color startColor = threadSprite != null ? threadSprite.color : Color.white;

        while (fadeElapsed < fadeTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, fadeElapsed / fadeTime);
            if (threadSprite) threadSprite.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            fadeElapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    private void Explode()
    {
        if (exploded) return;
        exploded = true;

        if (threadSprite) threadSprite.enabled = false;
        if (threadCollider) threadCollider.enabled = false;

        Collider2D[] objectsHit = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D obj in objectsHit)
        {
            if (IsPlayerTarget(obj))
            {
                PlayerStats stats = obj.GetComponentInParent<PlayerStats>();
                if (stats == null) stats = obj.GetComponent<PlayerStats>();

                if (stats != null)
                {
                    stats.DamagePlayer(explosionDamage);
                    KnockBackAbility knockback = obj.GetComponentInParent<KnockBackAbility>();
                    if (knockback != null) knockback.StartKnockBack(0.3f, knockbackForce * 1.5f, transform);
                }
            }
        }

        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
            Destroy(gameObject, 0.1f);
        }
        else
        {
            // Explosão Procedural (círculo vermelho caso o VFX esteja vazio)
            GameObject tempExplosion = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(tempExplosion.GetComponent<Collider>());

            tempExplosion.transform.position = transform.position;
            tempExplosion.transform.localScale = Vector3.zero;

            Renderer r = tempExplosion.GetComponent<Renderer>();
            if (r) r.material.color = new Color(1f, 0f, 0f, 0.5f);

            StartCoroutine(AnimateProceduralExplosion(tempExplosion));
        }
    }

    private IEnumerator AnimateProceduralExplosion(GameObject explosionObj)
    {
        float duration = 0.3f;
        float t = 0f;
        Vector3 maxScale = Vector3.one * explosionRadius * 2f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            if (explosionObj != null)
            {
                explosionObj.transform.localScale = Vector3.Lerp(Vector3.zero, maxScale, t);

                Renderer r = explosionObj.GetComponent<Renderer>();
                if (r)
                {
                    Color c = r.material.color;
                    c.a = Mathf.Lerp(0.8f, 0f, t);
                    r.material.color = c;
                }
            }
            yield return null;
        }

        if (explosionObj != null) Destroy(explosionObj);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}