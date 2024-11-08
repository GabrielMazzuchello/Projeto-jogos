using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float speed;
    public bool grond = true;
    public Transform groundCheck;
    public LayerMask grondLayer;
    public bool facingRight = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
        grond = Physics2D.Linecast(groundCheck.position, transform.position, grondLayer);
        Debug.Log(grond);

        if (grond == false)
        {
            speed *= -1;
        }

        if (speed > 0 && !facingRight)
        {
            flip();
        }
        else if (speed < 0 && facingRight)
        {
            flip();
        }
        
    }

    void flip ()
    {
        facingRight = !false;
        Vector3 Scale = transform.localScale;
        Scale.x *= -1;
        transform.localScale = Scale;
    }
}
