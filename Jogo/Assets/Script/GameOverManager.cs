using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{

    public Button restartButton; // Bot�o de Reiniciar


    void Start()
    {
        // Associa o bot�o Restart ao m�todo RestartGame
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    public void ShowGameOver()
    {
        Debug.Log("Exibindo tela de Game Over...");
        gameObject.SetActive(true); // Exibe o painel Game Over
        Time.timeScale = 0f;        // Pausa o jogo
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Retoma o tempo normal
        SceneManager.LoadScene("lvl_1"); // Carrega a primeira cena pelo nome
    }
}
