using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;  // Instância única para acesso global

    public int currentHealth;  // Vida atual
    public int maxHealth = 200; // Vida máxima

    void Awake()
    {
        // Verifica se já existe uma instância. Se não, cria a instância.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Não destrói o objeto ao trocar de cena
        }
        else
        {
            Destroy(gameObject); // Se já existe, destrói este objeto
        }

        
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
    public void RestartGame()
    {
        Time.timeScale = 1f; // Restaura o tempo
        ResetHealth(); // Restaura a vida do jogador
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reinicia a cena
    }
}
