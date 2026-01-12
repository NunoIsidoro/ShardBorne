using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("Painéis UI")]
    [Tooltip("Arrasta o Painel de Pausa Principal aqui")]
    [SerializeField] public GameObject pausePanel;

    [Tooltip("Arrasta o Painel de Opções aqui")]
    [SerializeField] public GameObject optionsPanel;

    private bool isPaused = false;

    void Start()
    {
        // Garante que ambos começam fechados
        if (pausePanel) pausePanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    void Update()
    {
        // Deteta o ESC
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // Se o painel de opções estiver aberto, o ESC volta para o menu de pausa
            if (optionsPanel != null && optionsPanel.activeSelf)
            {
                CloseOptions();
            }
            // Se não, faz o comportamento normal de Pausar/Despausar
            else if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        optionsPanel.SetActive(false); // Garante que opções não abre por cima
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // --- FUNÇÕES DE NAVEGAÇÃO ---

    public void OpenOptions()
    {
        // Esconde a Pausa, Mostra as Opções
        pausePanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        // Esconde as Opções, Volta à Pausa
        optionsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Saiu do jogo");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}