using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class NextLevelPoint : MonoBehaviour
{
    public string lvlName;

    void OnCollisionEnter2D(Collision2D colisao)
    {
        if (colisao.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(lvlName);

        }
        
    }
}
