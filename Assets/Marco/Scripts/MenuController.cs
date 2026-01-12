using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void OpenCredits()
    {
        SceneManager.LoadScene("Credits"); // nome da cena de créditos
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // nome da cena principal
    }
}
