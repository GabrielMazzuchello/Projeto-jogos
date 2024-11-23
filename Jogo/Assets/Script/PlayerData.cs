using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;  // Inst�ncia �nica para acesso global

    public int currentHealth;  // Vida atual
    public int maxHealth = 200; // Vida m�xima

    void Awake()
    {
        // Verifica se j� existe uma inst�ncia. Se n�o, cria a inst�ncia.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // N�o destr�i o objeto ao trocar de cena
        }
        else
        {
            Destroy(gameObject); // Se j� existe, destr�i este objeto
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
