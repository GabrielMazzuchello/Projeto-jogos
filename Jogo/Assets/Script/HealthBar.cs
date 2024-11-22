using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider; // Referência ao Slider

    public void SetMaxHealth(int health)
    {
        slider.maxValue = 100 ; // Define o valor máximo da barra de vida
        slider.value = health; // Define o valor atual da barra de vida
    }

    public void SetHealth(int health)
    {
        slider.value = health; // Atualiza o valor da barra de vida conforme o dano
    }
}
