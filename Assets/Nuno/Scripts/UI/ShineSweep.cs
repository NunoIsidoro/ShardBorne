using UnityEngine;

public class ShineSweep : MonoBehaviour
{
    public float speed = 120f;     // px/seg
    public float pause = 1.5f;     // pausa entre varrimentos
    public RectTransform area;     // alvo (o Glass)
    RectTransform rt;
    float timer; bool sweeping;

    void Awake() { rt = GetComponent<RectTransform>(); }
    void OnEnable() { timer = Random.Range(0f, pause); sweeping = false; }

    void Update()
    {
        if (!area) return;
        if (!sweeping)
        {
            timer -= Time.unscaledDeltaTime;
            if (timer <= 0f) { StartSweep(); }
            return;
        }
        rt.anchoredPosition += Vector2.right * speed * Time.unscaledDeltaTime;
        if (rt.anchoredPosition.x > area.rect.width * 1.1f)
        {
            sweeping = false;
            timer = pause + Random.Range(0f, 1f);
        }
    }

    void StartSweep()
    {
        var startX = -area.rect.width * 1.1f;
        rt.anchoredPosition = new Vector2(startX, 0f);
        sweeping = true;
    }
}
