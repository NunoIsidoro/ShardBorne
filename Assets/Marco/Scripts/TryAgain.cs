using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TryAgain : MonoBehaviour
{
    // Arrasta aqui o botão de UI
    public Button loadSceneButton;

    // Nome da cena para onde deve voltar
    public string sceneToLoad;

    void Start()
    {
        if (loadSceneButton != null)
        {
            loadSceneButton.onClick.AddListener(LoadScene);
        }
        else
        {
            Debug.LogWarning("Nenhum botão foi atribuído ao script LoadSceneButton!");
        }

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning("Nenhuma cena foi definida no script LoadSceneButton!");
        }
    }

    void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log("A carregar cena: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("sceneToLoad está vazio! Define o nome da cena no inspetor.");
        }
    }
}
