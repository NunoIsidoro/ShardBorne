using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    // Arrasta aqui o botao do tipo UI Button
    public Button quitButton;

    void Start()
    {
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
        else
        {
            Debug.LogWarning("Nenhum botao foi atribuido ao script QuitButton!");
        }
    }

    void QuitGame()
    {
        Debug.Log("Fechar jogo...");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
