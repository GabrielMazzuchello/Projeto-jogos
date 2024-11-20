using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;  // Inst�ncia �nica para acesso global

    public int currentHealth;  // Vida atual
    public int maxHealth = 100; // Vida m�xima

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

        // Se currentHealth n�o foi atribu�do (primeira vez que o jogo inicia),
        // inicializa com maxHealth
        if (currentHealth == 0)
        {
            currentHealth = maxHealth;
        }
    }

    // Reseta a vida para o valor m�ximo no in�cio de cada cena, se necess�rio
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}
