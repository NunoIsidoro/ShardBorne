using UnityEngine;

public class Damage_Spikes : MonoBehaviour
{
    [SerializeField] private float damageAmount = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se o objeto que entrou é o Player
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Detetou Player");

            // Usa o Singleton para garantir que encontra PlayerStats mesmo entre cenas
            PlayerStats playerStats = PlayerStats.Instance;

            if (playerStats != null)
            {
                Debug.Log("Player entrou em contato com espinhos. Causando dano.");
                playerStats.DamagePlayer(damageAmount);
            }
            else
            {
                Debug.LogError("ERRO: PlayerStats.Instance é NULL — o PlayerStats não foi inicializado!");
            }
        }
        }
    
}
