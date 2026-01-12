using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderLink : MonoBehaviour
{
    [Header("Tipo de Volume")]
    [Tooltip("Marque se este slider for para SFX. Desmarque se for para Música Master.")]
    public bool isSFXSlider = false;

    private Slider slider;
    private MusicManager musicManager;
    private OptionsMenu optionsMenu;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void OnEnable()
    {
        // Procura os gestores persistentes
        musicManager = FindFirstObjectByType<MusicManager>();
        optionsMenu = FindFirstObjectByType<OptionsMenu>();

        if (musicManager != null && slider != null)
        {
            // 1. Sincronização Inicial
            float volumeGuardado;

            if (isSFXSlider)
            {
                // Busca o volume de SFX guardado no PlayerPrefs (padrão 0.75f como no seu OptionsMenu)
                volumeGuardado = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
            }
            else
            {
                // Busca o volume Master do MusicManager
                volumeGuardado = musicManager.volume;
            }

            slider.value = volumeGuardado;

            // 2. Adiciona os Listeners para atualizar o sistema quando movermos o slider
            slider.onValueChanged.RemoveAllListeners(); // Limpa para evitar duplicados

            if (isSFXSlider && optionsMenu != null)
            {
                slider.onValueChanged.AddListener(optionsMenu.SetSFXVolume);
            }
            else
            {
                slider.onValueChanged.AddListener(musicManager.SetVolume);
            }
        }
    }

    void Update()
    {
        // 3. Sincronização em Tempo Real (Caso altere num painel, o outro move-se sozinho)
        if (slider == null || musicManager == null) return;

        if (isSFXSlider)
        {
            float currentSFX = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
            if (!Mathf.Approximately(slider.value, currentSFX))
            {
                slider.value = currentSFX;
            }
        }
        else
        {
            if (!Mathf.Approximately(slider.value, musicManager.volume))
            {
                slider.value = musicManager.volume;
            }
        }
    }
}