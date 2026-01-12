using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MonsterRoar : MonoBehaviour
{
    [Header("Configurações de Áudio")]
    public AudioClip roarClip;
    [Range(0, 1)] public float volume = 0.7f;

    [Header("Configurações de Distância")]
    public float detectionRadius = 5f; // Distância para ativar o rugido
    public float cooldown = 10f;       // Tempo até poder rugir de novo

    private AudioSource audioSource;
    private Transform playerTransform;
    private float lastRoarTime = -999f;
    private bool hasRoaredInThisRange = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // Configuração automática para som 2D/3D misto
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 100% 3D para o som vir do monstro

        // Encontra o jogador pela Tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    void Update()
    {
        if (playerTransform == null || roarClip == null) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // Se o jogador entrar no raio e o cooldown já passou
        if (distance <= detectionRadius && !hasRoaredInThisRange && Time.time > lastRoarTime + cooldown)
        {
            PlayRoar();
        }
        // Resetar o estado quando o jogador se afasta (para rugir ao voltar)
        else if (distance > detectionRadius + 2f)
        {
            hasRoaredInThisRange = false;
        }
    }

    void PlayRoar()
    {
        audioSource.PlayOneShot(roarClip, volume);
        lastRoarTime = Time.time;
        hasRoaredInThisRange = true;
        Debug.Log($"{gameObject.name} rugiu!");
    }

    // Desenha o círculo de deteção no Editor para facilitar o ajuste
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}