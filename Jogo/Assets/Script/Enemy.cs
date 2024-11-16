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
    public float attackRange = 1f; // Distância de ataque
    private bool isChasing = false;

    private int patrolDirection = 1;  // 1 para direita, -1 para esquerda

    public int health = 20; // Vida do inimigo

    // Área de detecção do ataque (objeto com trigger)
    public Collider2D attackRangeCollider;

    private bool isAttacking = false;  // Flag para verificar se está atacando
    public float attackInterval = 1f;  // Intervalo de tempo entre ataques
    private float lastAttackTime = 0f;  // Tempo do último ataque


    void Update()
    {
        DetectPlayer();

        if (isChasing && !isAttacking)
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
        if (isAttacking) return;  // Impede que o inimigo patrulhe enquanto ataca.

        transform.Translate(Vector2.right * patrolDirection * speed * Time.deltaTime);

        grond = Physics2D.Linecast(groundCheck.position, transform.position + Vector3.down * 0.1f, grondLayer);
        if (grond == false)
        {
            // speed *= -1;
            flip();
        }
    }

    void flip()
    {
        // Impede flip durante o ataque
        if (isAttacking) return;

        facingRight = !facingRight;
        patrolDirection *= -1;

        Vector3 Scale = transform.localScale;
        Scale.x *= -1;
        transform.localScale = Scale;
    }

    void DetectPlayer()
    {
        // Verifica a distância entre o inimigo e o jogador
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Se o jogador estiver dentro do alcance de ataque, iniciar o ataque
        if (distanceToPlayer <= attackRange)
        {
            if (!isAttacking)  // Inicia o ataque se não estiver atacando
            {
                StartCoroutine(AttackPlayer());
            }
            isChasing = false;  // Para de perseguir quando dentro do alcance de ataque
        }
        // Se o jogador estiver dentro do alcance de detecção (fora do alcance de ataque), começa a perseguição
        else if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;  // Começa a perseguir
            if (isAttacking)
            {
                StopCoroutine(AttackPlayer());  // Para o ataque se o jogador sair do alcance de ataque
                isAttacking = false;
            }
        }
        else
        {
            isChasing = false;  // O inimigo não persegue quando o jogador está fora do alcance
        }
    }

    void MoveTowardsPlayer()
    {
        // Se o inimigo está atacando, ele não deve se mover
        if (isAttacking) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Só se move se estiver fora do alcance de ataque
        if (distanceToPlayer > attackRange)
        {
            Vector2 direction = new Vector2(player.position.x - transform.position.x, 0).normalized;

            transform.Translate(direction * Mathf.Abs(speed) * Time.deltaTime);

            // Flip para que o inimigo sempre olhe para o jogador
            if ((player.position.x > transform.position.x && !facingRight) ||
                (player.position.x < transform.position.x && facingRight))
            {
                flip();
            }
        }
    }

    // Coroutine para atacar o jogador
    IEnumerator AttackPlayer()
    {
        isAttacking = true;

        // Atacar enquanto o inimigo estiver na área de ataque
        while (isAttacking)
        {
            if (Time.time >= lastAttackTime + attackInterval)  // Checa se o intervalo foi respeitado
            {
                Debug.Log("Inimigo atacando o jogador!");

                // Aplica dano ao jogador
                Player playerScript = player.GetComponent<Player>();
                if (playerScript != null)
                {
                    playerScript.TakeDamagePlayer(5);  // Valor do dano
                }

                lastAttackTime = Time.time;  // Atualiza o tempo do último ataque
            }

            yield return null;  // Espera um frame antes de continuar
        }
    }


    public void TakeDamageEnemy(int damage)
    {
        health -= damage;
        Debug.Log($"Inimigo recebeu {damage} de dano! Vida restante: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Inimigo morreu!");
        Destroy(gameObject); // Remove o inimigo da cena
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
