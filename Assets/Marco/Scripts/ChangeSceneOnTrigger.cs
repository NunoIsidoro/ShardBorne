using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneOnTrigger : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "Winning_Scene";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player entrou no trigger. A mudar para a cena: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
