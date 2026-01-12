using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneVarSaver : MonoBehaviour
{
    public DialogueVariables vars;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += (_, __) => SaveSystem.SaveAll(vars);
    }
    void OnDestroy() => SceneManager.activeSceneChanged -= (_, __) => SaveSystem.SaveAll(vars);
}
