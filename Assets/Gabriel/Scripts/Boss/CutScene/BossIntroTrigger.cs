using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossIntroTrigger : MonoBehaviour
{
    [Header("Configurações Gerais")]
    [SerializeField] private BossStateMachine boss;
    [SerializeField] private float cutsceneDuration = 4f; // Tempo total que a câmara fica no Boss
    [SerializeField] private float textDisplayDuration = 3f;

    [Header("Configurações de Cinemachine")]
    [Tooltip("Arrasta aqui o GameObject da VCam que criaste para focar no Boss")]
    [SerializeField] private GameObject bossVirtualCamera;

    [Header("Configurações de UI")]
    [SerializeField] private CanvasGroup titleTextCanvasGroup;

    [Header("Referências de Scripts")]
    [SerializeField] private MonoBehaviour playerMovementScript;

    // NOTA: Com Cinemachine não precisamos de desligar o script da câmara, 
    // a própria Cinemachine gere as prioridades.

    private bool hasTriggered = false;

    private void Start()
    {
        if (titleTextCanvasGroup)
        {
            titleTextCanvasGroup.alpha = 0f;
        }

        // Garante que a câmara do boss começa desligada
        if (bossVirtualCamera)
        {
            bossVirtualCamera.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;

            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            Animator playerAnim = other.GetComponent<Animator>();

            StartCoroutine(PlayIntroSequence(playerRb, playerAnim));
        }
    }

    private IEnumerator PlayIntroSequence(Rigidbody2D playerRb, Animator playerAnim)
    {
        // 1. FORÇAR PARAGEM TOTAL (Correção da Animação e Física)
        if (playerRb)
        {
            // Trava a física imediatamente
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
            playerRb.isKinematic = true; // Impede que forças externas (gravidade/empurrões) afetem
        }

        // 2. DESLIGAR CONTROLO
        if (playerMovementScript) playerMovementScript.enabled = false;

        // 3. RESET DE ANIMAÇÃO (Abordagem "Nuclear")
        if (playerAnim)
        {
            // Rebind() reinicia o Animator para o estado de entrada (Entry State), que é normalmente o "Idle".
            // Também coloca todos os parâmetros (Speed, IsRunning, etc.) a zero/false automaticamente.
            playerAnim.Rebind();
            playerAnim.Update(0f); // Força o Unity a atualizar o boneco para a pose de Idle neste exato frame
        }

        // 4. TROCA DE CÂMARA (CINEMACHINE)
        // Ao ativar a câmara do boss, a Cinemachine faz o blend suave automaticamente
        if (bossVirtualCamera)
        {
            bossVirtualCamera.SetActive(true);
        }

        // 5. MOSTRAR TÍTULO
        if (titleTextCanvasGroup)
        {
            float elapsed = 0f;
            // Fade In
            while (elapsed < 1f)
            {
                titleTextCanvasGroup.alpha = elapsed;
                elapsed += Time.deltaTime;
                yield return null;
            }
            titleTextCanvasGroup.alpha = 1f;

            // Espera enquanto mostra o texto e a câmara foca no boss
            yield return new WaitForSeconds(textDisplayDuration);

            // Fade Out
            elapsed = 0f;
            while (elapsed < 1f)
            {
                titleTextCanvasGroup.alpha = 1f - elapsed;
                elapsed += Time.deltaTime;
                yield return null;
            }
            titleTextCanvasGroup.alpha = 0f;
        }
        else
        {
            // Se não houver texto, espera apenas o tempo da cutscene
            yield return new WaitForSeconds(cutsceneDuration);
        }

        // VOLTAR AO PLAYER
        // Desativa a câmara do boss faz a Cinemachine voltar para a câmara do player (que tem menor prioridade mas é a única que sobra)
        if (bossVirtualCamera)
        {
            bossVirtualCamera.SetActive(false);
        }

        // Dá um pequeno tempo para a câmara voltar ao player antes de devolver o controlo
        yield return new WaitForSeconds(1.5f);

        // REATIVAR CONTROLO E FÍSICA
        if (playerRb)
        {
            playerRb.isKinematic = false;
        }

        if (playerMovementScript) playerMovementScript.enabled = true;

        // INICIAR BOSS
        if (boss) boss.StartBossFight();

        Destroy(gameObject);
    }
}