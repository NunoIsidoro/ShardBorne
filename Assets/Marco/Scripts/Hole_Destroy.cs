using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap), typeof(TilemapCollider2D))]
public class ClearWholeTilemapOnTouchGeneric : MonoBehaviour
{
    [Header("Configurações")]
    public GameObject destroyEffectPrefab; // opcional: partículas/explosão
    public bool useTrigger = false;        // usa trigger em vez de colisão

    private Tilemap tilemap;
    private Collider2D tilemapCollider;
    private bool destroyed = false;

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        tilemapCollider = GetComponent<Collider2D>();

        if (tilemapCollider != null)
            tilemapCollider.isTrigger = useTrigger;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (useTrigger) return;
        TryDestroy(collision.gameObject, collision.contacts[0].point);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!useTrigger) return;
        TryDestroy(other.gameObject, other.ClosestPoint(transform.position));
    }

    private void TryDestroy(GameObject other, Vector2 hitPoint)
    {
        if (destroyed) return;
        if (!other.CompareTag("Player")) return;

        destroyed = true;

        if (destroyEffectPrefab != null)
            Instantiate(destroyEffectPrefab, hitPoint, Quaternion.identity);

        // Apaga todos os tiles desta Tilemap
        tilemap.ClearAllTiles();

        // Desativa o collider
        if (tilemapCollider != null)
            tilemapCollider.enabled = false;
    }
}
