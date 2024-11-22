using UnityEngine;

public class VictoryZone : MonoBehaviour
{
    public GameObject victoryUI; // Referência para a tela de vitória

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Verifica se o objeto que entrou é o jogador
        {
            Time.timeScale = 0f; // Pausa o jogo
            victoryUI.SetActive(true); // Mostra a tela de vitória
        }
    }
}
