using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(AudioSource))]
public class CrystalButtonSfx : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverClip, clickClip;
    AudioSource src;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        src.playOnAwake = false; src.spatialBlend = 0f; src.loop = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverClip) src.PlayOneShot(hoverClip, 0.35f);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickClip) src.PlayOneShot(clickClip, 0.5f);
    }
}
