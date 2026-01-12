using UnityEngine;

/// <summary>
/// Objeto que dá Eco ao jogador quando apanhado.
/// Inclui levitação e efeito de íman.
/// </summary>
public class EchoCollectible : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float ecoAmount = 20f;
    [SerializeField] private GameObject collectVFX;

    [Header("Visuals (Levitação)")]
    [SerializeField] private float floatSpeed = 2f;      // Quão rápido oscila
    [SerializeField] private float floatAmplitude = 0.25f; // Quão alto/baixo vai
    private Vector3 startPos;

    [Header("Magnetism (Atração)")]
    [SerializeField] private float detectionRadius = 4f; // Distância para começar a seguir
    [SerializeField] private float magnetSpeed = 12f;    // Velocidade ao ir para o player
    [Tooltip("A Layer que atrai o Eco (deve ser a mesma da colisão, ex: PlayerStats)")]
    [SerializeField] private LayerMask attractionLayer;

    private Transform target;
    private bool isMagnetized = false;

    private void Start()
    {
        startPos = transform.position;

        // Configuração automática da layer se esqueceres no Inspector
        if (attractionLayer == 0)
        {
            int layerIndex = LayerMask.NameToLayer("PlayerStats");
            if (layerIndex != -1)
                attractionLayer = 1 << layerIndex;
        }
    }

    private void Update()
    {
        if (!isMagnetized)
        {
            // 1. LEVITAÇÃO: Oscila em torno da posição inicial
            float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            transform.position = new Vector3(startPos.x, newY, startPos.z);

            // 2. DETEÇÃO: Procura o player nas redondezas
            CheckForPlayer();
        }
        else
        {
            // 3. ATRAÇÃO: Voa para o player se tiver um alvo
            if (target != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, magnetSpeed * Time.deltaTime);
            }
            else
            {
                // Se perdeu o alvo (ex: player morreu), volta a flutuar no sítio onde parou
                isMagnetized = false;
                startPos = transform.position;
            }
        }
    }

    private void CheckForPlayer()
    {
        // Procura um colisor na layer definida dentro do raio
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, attractionLayer);

        if (hit != null)
        {
            // Encontrou! Define como alvo e ativa o modo íman
            target = hit.transform;
            isMagnetized = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Lógica original de coleta
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerStats"))
        {
            PlayerStats stats = collision.GetComponent<PlayerStats>();
            if (stats == null) stats = collision.GetComponentInParent<PlayerStats>();

            if (stats != null)
            {
                stats.AddEco(ecoAmount);

                if (collectVFX != null)
                {
                    Instantiate(collectVFX, transform.position, Quaternion.identity);
                }

                Destroy(gameObject);
            }
        }
    }

    // Desenha o raio de atração no Editor para facilitar o ajuste
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}