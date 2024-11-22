using UnityEngine;

public class VictoryZone : MonoBehaviour
{
    public GameObject victoryUI; // Refer�ncia para a tela de vit�ria

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Verifica se o objeto que entrou � o jogador
        {
            Time.timeScale = 0f; // Pausa o jogo
            victoryUI.SetActive(true); // Mostra a tela de vit�ria
        }
    }
}
