using UnityEngine;

public class DropOnDeath : MonoBehaviour
{
    [System.Serializable] public class Drop { public GameObject prefab; [Range(0, 1)] public float chance = 1f; public int min = 1, max = 1; }

    public Drop[] drops;
    public Transform dropPoint;

    public bool alsoOnDestroy = true;
    static bool quitting;

    void OnApplicationQuit() { quitting = true; }

    public void OnDeath()
    {
        DoDrop();
        Destroy(gameObject);
    }



    void OnDestroy()
    {
        if (!alsoOnDestroy) return;
        if (quitting) return;                         // não dropar ao fechar o jogo
        if (!gameObject.scene.isLoaded) return;       // nem ao descarregar a cena
        DoDrop();
    }

    void DoDrop()
    {
        if (drops == null) return;
        Vector3 pos = dropPoint ? dropPoint.position : transform.position;
        foreach (var d in drops)
        {
            if (!d.prefab) continue;
            if (Random.value > d.chance) continue;
            int n = Random.Range(d.min, d.max + 1);
            for (int i = 0; i < n; i++) Instantiate(d.prefab, pos, Quaternion.identity);
        }
    }
}
