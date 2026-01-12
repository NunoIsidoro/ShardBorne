using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;
#if UNITY_6000_0_OR_NEWER
using Unity.Cinemachine;
#else
using Cinemachine;
#endif

public class PlayerSpawnManager2D : MonoBehaviour
{
    //[Header("Configuração da Cena")]
    //[Tooltip("Nome que aparece no ecrã ao entrar (ex: Vila Inicial)")]
    //public string sceneDisplayName = "";

    public Transform playerRoot;
    public string defaultSpawnId = "Start";

    private ScreenFader fader;

    void Awake()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //fader = FindFirstObjectByType<ScreenFader>();

        /*
        // 1. CONFIGURAÇÃO IMEDIATA (Antes de veres a cena)
        if (fader != null)
        {
            // O Awake do Fader já pôs tudo preto, mas garantimos aqui o texto
            fader.SetSceneName(sceneDisplayName);
        }
        */
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        // --- 1. ENCONTRAR PLAYER ---
        if (playerRoot == null || playerRoot.gameObject == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go) playerRoot = go.transform;
        }

        // Se falhar, abre o ecrã para não bloquear o jogo
        if (playerRoot == null)
        {
            if (fader) fader.FadeIn();
            yield break;
        }

        // --- 2. CONGELAR VISUALMENTE (Tudo escondido pelo Fader) ---
        var rb = playerRoot.GetComponent<Rigidbody2D>();
        var renderers = playerRoot.GetComponentsInChildren<Renderer>();
        if (rb) { rb.simulated = false; rb.linearVelocity = Vector2.zero; }
        foreach (var r in renderers) r.enabled = false;

        yield return null; // Espera 1 frame para estabilizar

        // --- 3. POSICIONAR ---
        string id = string.IsNullOrEmpty(SceneTransit.nextSpawnId) ? defaultSpawnId : SceneTransit.nextSpawnId;
        var spawns = FindObjectsByType<SpawnPoint2D>(FindObjectsSortMode.None);
        var target = spawns.FirstOrDefault(s => s.spawnId == id);
        if (target == null && spawns.Length > 0) target = spawns[0];

        if (target != null)
        {
            Vector3 finalPos = target.transform.position + (Vector3)target.spawnOffset;

            // Snap ao chão
            if (target.snapToGround)
            {
                var col = playerRoot.GetComponent<Collider2D>();
                float halfHeight = (col ? col.bounds.extents.y : 0.5f);
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(finalPos.x, finalPos.y + 2f), Vector2.down, 10f, target.groundMask);
                if (hit.collider != null) finalPos.y = hit.point.y + halfHeight;
            }

            playerRoot.position = finalPos;
            if (rb) rb.position = finalPos;

            // Orientação
            if (target.face != FaceDir.Unchanged)
            {
                var s = playerRoot.localScale;
                float xParams = Mathf.Abs(s.x);
                if (target.face == FaceDir.Left) xParams *= -1;
                playerRoot.localScale = new Vector3(xParams, s.y, s.z);

                var pScript = playerRoot.GetComponent<Player>();
                if (pScript) pScript.facingRight = (xParams > 0);
            }

            // Cinemachine Warp (Evita flicker da câmara)
#if UNITY_6000_0_OR_NEWER
            var cam = FindFirstObjectByType<CinemachineCamera>();
            if (cam) cam.OnTargetObjectWarped(playerRoot, finalPos - playerRoot.position);
#else
            var cam = FindFirstObjectByType<CinemachineVirtualCamera>();
            if (cam) cam.OnTargetObjectWarped(playerRoot, finalPos - playerRoot.position);
#endif
        }

        Physics2D.SyncTransforms();

        // --- 4. DESCONGELAR ---
        if (rb) { rb.simulated = true; rb.linearVelocity = Vector2.zero; }
        foreach (var r in renderers) r.enabled = true;

        SceneTransit.nextSpawnId = null;
        SceneTransit.ArmCooldown(0.5f);

        // --- 5. FADE IN (0.6s) ---
        // Agora que está tudo pronto, revelamos o jogo
        if (fader == null) fader = FindFirstObjectByType<ScreenFader>();
        if (fader != null)
        {
            fader.FadeIn();
        }

        SaveSystem.SaveAtScene(SceneManager.GetActiveScene().name);
    }
}