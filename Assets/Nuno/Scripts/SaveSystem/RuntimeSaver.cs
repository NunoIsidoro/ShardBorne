using UnityEngine;
using UnityEngine.SceneManagement;

public class RuntimeSaver : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += OnSceneChanged;
    }
    void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
    void OnSceneChanged(Scene previous, Scene next)
    {
        SaveSystem.SaveAtScene(next.name);
    }
}
