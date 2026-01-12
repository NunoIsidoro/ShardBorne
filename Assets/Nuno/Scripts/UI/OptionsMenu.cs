using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Áudio Referências")]
    public AudioMixer mainMixer;
    public Slider masterSlider;
    public Slider sfxSlider;

    [Header("Vídeo Referências")]
    public Toggle fullscreenToggle; // NOVO: Arrasta o Toggle para aqui

    const string MASTER_PARAM = "MasterVolume";
    const string SFX_PARAM = "SFXVolume";

    void Start()
    {
        // --- Carregar Master ---
        float masterVol = PlayerPrefs.GetFloat(MASTER_PARAM, 0.75f);
        if (masterSlider)
        {
            masterSlider.value = masterVol;
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
        }
        SetMasterVolume(masterVol);

        // --- Carregar SFX ---
        float sfxVol = PlayerPrefs.GetFloat(SFX_PARAM, 0.75f);
        if (sfxSlider)
        {
            sfxSlider.value = sfxVol;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
        SetSFXVolume(sfxVol);

        // --- Carregar Fullscreen ---
        // Verifica o estado atual do ecrã ao iniciar
        if (fullscreenToggle)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }
    }

    // --- FUNÇÕES DE ÁUDIO ---

    public void SetMasterVolume(float volume)
    {
        float db = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        mainMixer.SetFloat(MASTER_PARAM, db);
        PlayerPrefs.SetFloat(MASTER_PARAM, volume);
    }

    public void SetSFXVolume(float volume)
    {
        float db = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        mainMixer.SetFloat(SFX_PARAM, db);
        PlayerPrefs.SetFloat(SFX_PARAM, volume);
    }

    // --- NOVA FUNÇÃO DE VÍDEO ---

    public void SetFullscreen(bool isFullscreen)
    {
        // Define o ecrã cheio com base no valor do Toggle (true ou false)
        Screen.fullScreen = isFullscreen;

        Debug.Log("Fullscreen alterado para: " + isFullscreen);
    }
}