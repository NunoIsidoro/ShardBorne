using UnityEngine;

using UnityEngine.SceneManagement;



public class MusicManager : MonoBehaviour

{

    public AudioSource audioSource;

    public AudioClip musicaMenu;

    public AudioClip musicaUnderground;



    [Range(0f, 1f)]

    public float volume = 1f; // Volume inicial padrão



    private string[] scenesSemMusica = { "Finish_TryAgain_Scene" };

    private string[] scenesUnderground = { "Underground1", "Underground2", "Underground3" };



    void Awake()

    {

        // Evita duplicações caso a primeira cena seja recarregada

        MusicManager[] managers = FindObjectsOfType<MusicManager>();

        if (managers.Length > 1)

        {

            Destroy(gameObject);

            return;

        }



        DontDestroyOnLoad(gameObject);



        // Carregar volume salvo, se existir

        if (PlayerPrefs.HasKey("Volume"))

        {

            volume = PlayerPrefs.GetFloat("Volume");

        }



        audioSource.volume = volume;

    }



    void Start()

    {

        SceneManager.sceneLoaded += OnSceneLoaded;

    }



    void OnSceneLoaded(Scene scene, LoadSceneMode mode)

    {

        string currentScene = scene.name;



        // Verifica se a cena não deve ter música

        foreach (string nome in scenesSemMusica)

        {

            if (currentScene == nome)

            {

                audioSource.Stop();

                return;

            }

        }



        // Verifica se a cena é Underground

        foreach (string nome in scenesUnderground)

        {

            if (currentScene == nome)

            {

                TrocarMusica(musicaUnderground);

                return;

            }

        }



        // Qualquer outra cena toca música do menu

        TrocarMusica(musicaMenu);

    }



    void TrocarMusica(AudioClip novaMusica)

    {

        if (audioSource.clip == novaMusica && audioSource.isPlaying)

            return;



        audioSource.clip = novaMusica;

        audioSource.loop = true;

        audioSource.Play();

        audioSource.volume = volume; // garante que o volume atual seja aplicado

    }



    // Método público para o Slider

    public void SetVolume(float novoVolume)

    {

        volume = novoVolume;

        audioSource.volume = volume;



        // Salva o volume para futuras sessões

        PlayerPrefs.SetFloat("Volume", volume);

    }

}