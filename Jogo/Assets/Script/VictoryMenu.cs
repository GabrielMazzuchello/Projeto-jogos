using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryMenu : MonoBehaviour
{
    public void RestartGame()
    {
        Time.timeScale = 1f; // Retoma o tempo normal
        SceneManager.LoadScene("lvl_1"); // Troque pelo nome da cena inicial
    }
}
