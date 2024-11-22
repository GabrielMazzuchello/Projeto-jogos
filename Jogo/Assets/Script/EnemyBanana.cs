using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEnemy : MonoBehaviour
{
    public float speed = 2f;
    public bool grounded = true;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public bool facingRight = true;

    public Transform player;
    public float detectionRangeX = 5f;
    public float detectionRangeY = 2f;
    public float attackRangeX = 1f;
    public float attackRangeY = 1f;
    private bool isChasing = false;

    public int health = 20;
    private bool isDead = false;
    private bool isAttacking = false;

    public float attackInterval = 1f;
    private float lastAttackTime = 0f;

    // Referência ao Animator
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        // Localiza automaticamente o jogador pela tag "Player"
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player não encontrado! Verifique se o jogador tem a tag 'Player'.");
        }
    }

    void Update()
    {
        if (isDead) return;

        DetectPlayer();

        if (isAttacking)
        {
            anim.SetInteger("TransicaoBanana", 2); // Animação de ataque
        }
        else if (isChasing)
        {
            anim.SetInteger("TransicaoBanana", 1); // Animação de perseguição
            MoveTowardsPlayer();
        }
        else
        {
            anim.SetInteger("TransicaoBanana", 0); // Animação de patrulha
            Patrol();
        }
    }

    void Patrol()
    {
        if (isAttacking) return;

        transform.Translate(Vector2.right * speed * Time.deltaTime);

        grounded = Physics2D.Linecast(groundCheck.position, transform.position + Vector3.down * 0.1f, groundLayer);

        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        RaycastHit2D obstacle = Physics2D.Raycast(transform.position, direction, 2f, groundLayer);

        if (!grounded || obstacle.collider != null)
        {
            Flip();
        }
    }

    void Flip()
    {
        if (isAttacking) return;

        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void DetectPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapBox(transform.position, new Vector2(detectionRangeX, detectionRangeY), 0f, LayerMask.GetMask("Player"));

        if (playerCollider != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerCollider.transform.position);

            if (distanceToPlayer <= attackRangeX)
            {
                if (!isAttacking)
                {
                    StartCoroutine(AttackPlayer());
                }
                isChasing = false;
            }
            else if (distanceToPlayer <= detectionRangeX)
            {
                isChasing = true;
                if (isAttacking)
                {
                    StopCoroutine(AttackPlayer());
                    isAttacking = false;
                }
            }
            else
            {
                isChasing = false;
            }
        }
        else
        {
            isChasing = false;
        }
    }

    void MoveTowardsPlayer()
    {
        if (isAttacking) return;

        grounded = Physics2D.Linecast(groundCheck.position, transform.position + Vector3.down * 0.1f, groundLayer);

        if (!grounded)
        {
            isChasing = false;
            anim.SetInteger("TransicaoBanana", 0);
            return;
        }

        if (player != null)
        {
            // Move o inimigo em direção ao jogador
            Vector2 direction = new Vector2(player.position.x - transform.position.x, 0).normalized;
            transform.Translate(direction * Mathf.Abs(speed) * Time.deltaTime);

            // Verifica se o inimigo precisa inverter a direção (flip) com base no jogador
            if ((player.position.x > transform.position.x && !facingRight) ||
                (player.position.x < transform.position.x && facingRight))
            {
                Flip();
            }
        }
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;

        while (isAttacking && !isDead)
        {
            if (Time.time >= lastAttackTime + attackInterval)
            {
                Debug.Log("Inimigo atacando o jogador!");
                Player playerScript = player.GetComponent<Player>();
                if (playerScript != null)
                {
                    playerScript.TakeDamagePlayer(5);
                }
                lastAttackTime = Time.time;
            }
            yield return null;
        }
        isAttacking = false;
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        anim.SetInteger("TransicaoBanana", 3); // Animação de morte
        Debug.Log("Inimigo morreu!");

        StopAllCoroutines();

        Destroy(gameObject, 1f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(detectionRangeX, detectionRangeY, 0f));
        Gizmos.DrawWireCube(transform.position, new Vector3(attackRangeX, attackRangeY, 0f));
    }
}
