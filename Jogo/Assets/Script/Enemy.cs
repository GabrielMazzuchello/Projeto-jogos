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

    public Transform player;
    public float detectionRange;
    public float minDistanceToPlayer = 1.5f;
    private bool isChasing = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DetectPlayer();

        if (isChasing)
        {
            MoveTowardsPlayer();
        }
        else
        {
            patrol();
        }        
    }

    void patrol()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        grond = Physics2D.Linecast(groundCheck.position, transform.position + Vector3.down * 0.1f, grondLayer);
        Debug.Log(grond);

        if (grond == false)
        {
            speed *= -1;
            flip();
        }

    }

    void flip ()
    {
        facingRight = !facingRight;
        Vector3 Scale = transform.localScale;
        Scale.x *= -1;
        transform.localScale = Scale;
    }

    void DetectPlayer()
    {
        isChasing = Physics2D.OverlapCircle(transform.position, detectionRange, LayerMask.GetMask("Player"));

        if (!isChasing && speed < 0 && facingRight || speed > 0 && !facingRight)
        {
            flip();
        }
        
    }

    void MoveTowardsPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > minDistanceToPlayer)
        {
            Vector2 direction = new Vector2(player.position.x - transform.position.x, 0).normalized;
            
            transform.Translate(direction * Mathf.Abs(speed) * Time.deltaTime);
            
            if ((player.position.x > transform.position.x && !facingRight) ||
                (player.position.x < transform.position.x && facingRight))
            {
                flip();
            }
        }
    }
}

