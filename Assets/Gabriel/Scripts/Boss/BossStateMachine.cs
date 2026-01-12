using UnityEngine;
using System.Collections;

public class BossStateMachine : MonoBehaviour
{
    public enum BossState
    {
        WaitingToStart,
        PhaseOne,
        PhaseTwo,
        Death
    }

    private BossState currentBossState;

    [Header("Referências")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D bossCollider;
    [SerializeField] private GameObject threadPrefab;
    [SerializeField] private Animator anim;
    [SerializeField] private BossStats bossStats;

    [Header("Animações")]
    [SerializeField] private string idleAnim = "Idle";
    [SerializeField] private string attackAnim = "Attack";
    [SerializeField] private string attackDiagonalAnim = "AttackDiagonal";
    [SerializeField] private string deathAnim = "Death";

    [Header("Escala Visual")]
    [SerializeField] private Vector3 backScale = Vector3.one * 0.5f;
    [SerializeField] private Vector3 frontScale = Vector3.one;
    [SerializeField] private float transitionDuration = 1.5f;

    [Header("Timers")]
    [SerializeField] private float phaseOneDuration = 10f;
    [SerializeField] private float phaseTwoDuration = 8f;
    [SerializeField] private float idleTime = 2f;
    [SerializeField] private float attackTime = 1.5f;

    [Header("Thread Spawn Area")]
    [SerializeField] private Transform threadSpawnArea;
    [SerializeField] private float threadSpawnWidth = 10f;

    [Header("Configuração de Ataques (Quantidade)")]
    [SerializeField] private int minVerticalThreads = 3;
    [SerializeField] private int maxVerticalThreads = 6;
    [Space(5)]
    [SerializeField] private int minDiagonalThreads = 4;
    [SerializeField] private int maxDiagonalThreads = 7;

    [Header("Configuração de Variantes (Fios Especiais - Fase 2)")]
    [Range(0, 100)][SerializeField] private float chanceExplosive = 20f;
    [Range(0, 100)][SerializeField] private float chanceSlow = 20f;

    private float phaseTimer;
    private float actionTimer;
    private bool isAttacking;
    private bool nextAttackIsDiagonal = false;

    private void Start()
    {
        if (bossStats == null) bossStats = GetComponent<BossStats>();

        ChangeBossState(BossState.WaitingToStart);
    }

    public void StartBossFight()
    {
        Debug.Log("Boss Fight Iniciada!");
        ChangeBossState(BossState.PhaseOne);
    }

    private void Update()
    {
        switch (currentBossState)
        {
            case BossState.PhaseOne:
                UpdatePhaseOne();
                break;
            case BossState.PhaseTwo:
                UpdatePhaseTwo();
                break;
        }
    }

    #region STATE MACHINE
    public void ChangeBossState(BossState newState)
    {
        if (newState == currentBossState) return;

        ExitBossState(currentBossState);
        currentBossState = newState;
        EnterBossState(currentBossState);
    }

    private void EnterBossState(BossState state)
    {
        switch (state)
        {
            case BossState.WaitingToStart: EnterWaiting(); break;
            case BossState.PhaseOne: EnterPhaseOne(); break;
            case BossState.PhaseTwo: EnterPhaseTwo(); break;
            case BossState.Death: EnterDeath(); break;
        }
    }

    private void ExitBossState(BossState state)
    {
    }
    #endregion

    #region WAITING STATE
    private void EnterWaiting()
    {
        transform.localScale = frontScale;
        if (anim) anim.Play(idleAnim);

        if (bossStats) bossStats.SetInvulnerable(true);
        if (bossCollider) bossCollider.enabled = false;
    }
    #endregion

    #region PHASE ONE
    private void EnterPhaseOne()
    {
        transform.localScale = backScale;
        if (bossCollider) bossCollider.enabled = false;
        if (bossStats) bossStats.SetInvulnerable(true);

        phaseTimer = phaseOneDuration;
        actionTimer = idleTime;
        isAttacking = false;
        nextAttackIsDiagonal = false;
        if (anim) anim.Play(idleAnim);
    }

    private void UpdatePhaseOne()
    {
        phaseTimer -= Time.deltaTime;
        actionTimer -= Time.deltaTime;

        if (actionTimer <= 0f)
        {
            if (isAttacking)
            {
                if (anim) anim.Play(idleAnim);
                actionTimer = idleTime;
                isAttacking = false;
            }
            else
            {
                actionTimer = attackTime;
                isAttacking = true;

                int count = 0;

                if (nextAttackIsDiagonal)
                {
                    if (anim) anim.Play(attackDiagonalAnim);
                    count = Random.Range(minDiagonalThreads, maxDiagonalThreads + 1);
                    SpawnDiagonalThreads(count);
                }
                else
                {
                    if (anim) anim.Play(attackAnim);
                    count = Random.Range(minVerticalThreads, maxVerticalThreads + 1);
                    SpawnVerticalThreads(count);
                }

                nextAttackIsDiagonal = !nextAttackIsDiagonal;
            }
        }

        if (phaseTimer <= 0f)
        {
            StartCoroutine(TransitionToPhaseTwo());
        }
    }
    #endregion

    #region TRANSITION
    private IEnumerator TransitionToPhaseTwo()
    {
        if (anim) anim.Play(idleAnim);

        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            transform.localScale = Vector3.Lerp(startScale, frontScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = frontScale;
        if (bossCollider) bossCollider.enabled = true;
        if (bossStats) bossStats.SetInvulnerable(false);

        ChangeBossState(BossState.PhaseTwo);
    }

    private IEnumerator TransitionToPhaseOne()
    {
        if (anim) anim.Play(idleAnim);
        if (bossStats) bossStats.SetInvulnerable(true);

        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            transform.localScale = Vector3.Lerp(startScale, backScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = backScale;
        if (bossCollider) bossCollider.enabled = false;

        ChangeBossState(BossState.PhaseOne);
    }
    #endregion

    #region PHASE TWO
    private void EnterPhaseTwo()
    {
        if (anim) anim.Play(idleAnim);
        if (bossStats) bossStats.SetInvulnerable(false);
        phaseTimer = phaseTwoDuration;
    }

    private void UpdatePhaseTwo()
    {
        phaseTimer -= Time.deltaTime;
        if (phaseTimer <= 0f)
        {
            StartCoroutine(TransitionToPhaseOne());
        }
    }
    #endregion

    #region DEATH
    private void EnterDeath()
    {
        if (anim) anim.Play(deathAnim);
        if (bossCollider) bossCollider.enabled = false;
        if (bossStats) bossStats.SetInvulnerable(true);

        if (rb)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
    #endregion

    #region SPECIAL ATTACKS
    private void SpawnVerticalThreads(int count)
    {
        if (CheckMissingRefs()) return;

        float areaX = threadSpawnArea.position.x;
        float areaY = threadSpawnArea.position.y;
        float halfWidth = threadSpawnWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            float randomX = Random.Range(areaX - halfWidth, areaX + halfWidth);
            Vector3 spawnPos = new Vector3(randomX, areaY, 0f);

            CreateThread(spawnPos, Quaternion.identity);
        }
    }

    private void SpawnDiagonalThreads(int count)
    {
        if (CheckMissingRefs()) return;

        float areaX = threadSpawnArea.position.x;
        float areaY = threadSpawnArea.position.y;
        float halfWidth = threadSpawnWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            float randomX = Random.Range(areaX - halfWidth, areaX + halfWidth);
            Vector3 spawnPos = new Vector3(randomX, areaY, 0f);
            bool leanRight = Random.value > 0.5f;
            float zAngle = leanRight ? -25f : 25f;
            Quaternion rotation = Quaternion.Euler(0f, 0f, zAngle);

            CreateThread(spawnPos, rotation);
        }
    }

    // LÓGICA DE CRIAÇÃO E DECISÃO DE TIPO
    private void CreateThread(Vector3 pos, Quaternion rot)
    {
        GameObject threadObj = Instantiate(threadPrefab, pos, rot);
        ThreadProjectile projectile = threadObj.GetComponent<ThreadProjectile>();

        if (projectile != null)
        {
            bool isEnraged = false;

            // Verifica se a vida está abaixo de 50%
            if (bossStats != null && bossStats.MaxHealth > 0)
            {
                if (bossStats.CurrentHealth <= bossStats.MaxHealth * 0.5f)
                {
                    isEnraged = true;
                }
            }

            if (!isEnraged)
            {
                // Fase 1: Sempre normal
                projectile.SetThreadType(ThreadProjectile.ThreadType.Normal);
            }
            else
            {
                // Fase 2: Aleatório
                float roll = Random.Range(0f, 100f);

                if (roll < chanceExplosive)
                {
                    projectile.SetThreadType(ThreadProjectile.ThreadType.Explosive);
                }
                else if (roll < chanceExplosive + chanceSlow)
                {
                    projectile.SetThreadType(ThreadProjectile.ThreadType.Slow);
                }
                else
                {
                    projectile.SetThreadType(ThreadProjectile.ThreadType.Normal);
                }
            }
        }
    }

    private bool CheckMissingRefs()
    {
        if (threadSpawnArea == null || threadPrefab == null)
        {
            Debug.LogError("Faltam referências do Spawn Area ou Prefab!");
            return true;
        }
        return false;
    }
    #endregion
}