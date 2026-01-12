using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    public Button btnNewGame;
    public Button btnContinue;
    public Button btnOptions;
    public Button btnCredits;
    public Button btnQuit;

    [Header("Panels")]
    public GameObject panelOptions;
    public GameObject panelCredits;

    [Header("Fade")]
    public ScreenFader fader; 
    public float fadeOutTime = 0.35f; 

    [Header("Config")]
    [Tooltip("Nome da primeira cena jogável (exacto como no Build Settings).")]
    public string firstGameScene = "A_Vila_Abertura";
    [Tooltip("Se não existir save, o botão Continuar fica oculto.")]
    public bool hideContinueIfNoSave = true;

    [Header("State")]
    public DialogueVariables dialogueVars;

    void Start()
    {

        if (Player.Instance != null)
        {
            Destroy(Player.Instance.gameObject);
        }

        if (PlayerStats.Instance != null)
        {
            Destroy(PlayerStats.Instance.gameObject);
        }

        SaveSystem.DeleteSave();
        SaveSystem.DeleteVars();
        

        Time.timeScale = 1f;

        if (btnNewGame) btnNewGame.onClick.AddListener(OnNewGame);
        if (btnContinue) btnContinue.onClick.AddListener(OnContinue);
        if (btnOptions) btnOptions.onClick.AddListener(() => ShowPanel(panelOptions, true));
        if (btnCredits) btnCredits.onClick.AddListener(() => ShowPanel(panelCredits, true));
        if (btnQuit) btnQuit.onClick.AddListener(OnQuit);

        // Estado do Continuar
        bool hasSave = SaveSystem.HasSave() && SceneExists(SaveSystem.GetLastScene());
        if (btnContinue)
        {
            if (hideContinueIfNoSave) btnContinue.gameObject.SetActive(hasSave);
            else btnContinue.interactable = hasSave;
        }

        ShowPanel(panelOptions, false);
        ShowPanel(panelCredits, false);
    }

    void ShowPanel(GameObject panel, bool show)
    {
        if (panel) panel.SetActive(show);
    }

    void SetButtonsInteractable(bool v)
    {
        if (btnNewGame) btnNewGame.interactable = v;
        if (btnContinue) btnContinue.interactable = v;
        if (btnOptions) btnOptions.interactable = v;
        if (btnCredits) btnCredits.interactable = v;
        if (btnQuit) btnQuit.interactable = v;
    }

    public void OnNewGame()
    {
        SaveSystem.DeleteSave();
        SaveSystem.HardResetVars(dialogueVars, setNewGameDefaults: true);
        SaveSystem.EnsureDefaultVars(dialogueVars);

        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.ResetHealth();
        }

        LoadWithFade(firstGameScene);
    }

    public void OnContinue()
    {
        string scene = SaveSystem.GetLastScene();
        if (string.IsNullOrEmpty(scene) || !SceneExists(scene))
        {
            // fallback
            OnNewGame();
            return;
        }
        LoadWithFade(scene);
    }

    void LoadWithFade(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[MainMenu] Cena de destino vazia.");
            return;
        }
        SetButtonsInteractable(false);

        SceneManager.LoadScene(sceneName);
        
    }

    bool SceneExists(string sceneName)
    {
        // verifica se a cena está no Build Settings
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName) return true;
        }
        return false;
    }

    public void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Botões "Voltar"
    public void CloseOptions() => ShowPanel(panelOptions, false);
    public void CloseCredits() => ShowPanel(panelCredits, false);
}
