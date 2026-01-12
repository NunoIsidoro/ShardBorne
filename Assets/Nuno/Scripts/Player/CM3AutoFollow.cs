using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CM3AutoFollow : MonoBehaviour
{
    public string playerTag = "Player";

    void OnEnable() { SceneManager.sceneLoaded += OnLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnLoaded; }

    void Start() => Assign();
    void OnLoaded(Scene s, LoadSceneMode m) => Assign();

    void Assign()
    {
        var cam = GetComponent<CinemachineCamera>();
        if (!cam) return;

        var player = GameObject.FindGameObjectWithTag(playerTag);
        if (player)
        {
            // em CM3 é isto:
            cam.Target.TrackingTarget = player.transform;

            // se o nome do campo for diferente na tua versão (ex.: TrackedTarget),
            // abre o Inspector do CinemachineCamera e escolhe o transform do Player manualmente.
        }
    }
}
