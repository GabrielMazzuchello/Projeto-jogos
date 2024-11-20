using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    private static HealthBarManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Remove duplicatas
        }
    }
}
