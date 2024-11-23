using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemybanana : MonoBehaviour
{
    public float speed;
    public bool grond = true;
    public Transform groundCheck;
    public LayerMask grondLayer;
    public bool facingRight = true;

    public Transform player;
    public float detectionRangeX = 5f; // Largura da �rea de detec��o
    public float detectionRangeY = 2f; // Altura da �rea de detec��o
    public float attackRangeX = 1f;  // Largura da �rea de ataque
    public float attackRangeY = 1f;  // Altura da �rea de ataque
    private bool isChasing = false;

    private int patrolDirection = 1;  // 1 para direita, -1 para esquerda

    public int health = 20; // Vida do inimigo
    private bool isDead = false; // Verifica se j� morreu

    private bool isAttacking = false;  // Flag para verificar se est� atacando
    public float attackInterval = 1f;  // Intervalo de tempo entre ataques
    private float lastAttackTime = 0f;  // Tempo do �ltimo ataque

    // Refer�ncia ao Animator
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        // Localiza automaticamente o jogador pelo Tag "Player"
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player n�o encontrado! Verifique se o jogador tem a tag 'Player'.");
        }

    }

    void Update()
    {
        if (isDead) return; // Interrompe as a��es se o inimigo estiver morto

        DetectPlayer();

        if (isAttacking)
        {
            anim.SetInteger("TransitionBanana", 2); // Estado de ataque
        }
        else if (isChasing)
        {
            anim.SetInteger("TransitionBanana", 1); // Estado de persegui��o
            MoveTowardsPlayer();
        }
        else
        {
            anim.SetInteger("TransitionBanana", 1); // Estado de patrulha
            patrol();
        }
    }

    void patrol()
    {
        if (isAttacking) return;

        // Move o inimigo na dire��o atual
        transform.Translate(Vector2.right * patrolDirection * speed * Time.deltaTime);

        // Verifica se h� ch�o � frente
        grond = Physics2D.Linecast(groundCheck.position, transform.position + Vector3.down * 0.1f, grondLayer);

        // Verifica se h� obst�culos horizontais � frente
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        RaycastHit2D obstacle = Physics2D.Raycast(transform.position, direction, 2f, grondLayer);

        // Se n�o houver ch�o ou houver obst�culo, inverter a dire��o
        if (!grond || obstacle.collider != null)
        {
            flip();
        }
    }

    void flip()
    {
        if (isAttacking) return;

        facingRight = !facingRight;
        patrolDirection *= -1;

        Vector3 Scale = transform.localScale;
        Scale.x *= -1;
        transform.localScale = Scale;
    }

    void DetectPlayer()
    {
        // Verifica a detec��o usando OverlapBox
        Collider2D playerCollider = Physics2D.OverlapBox(transform.position, new Vector2(detectionRangeX, detectionRangeY), 0f, LayerMask.GetMask("Player"));

        if (playerCollider != null)
        {
            // Calcula a dist�ncia at� o jogador
            float distanceToPlayer = Vector2.Distance(transform.position, playerCollider.transform.position);

            // Se o jogador est� dentro da �rea de ataque
            if (distanceToPlayer <= attackRangeX)
            {
                if (!isAttacking)
                {
                    StartCoroutine(AttackPlayer()); // Inicia o ataque
                }
                isChasing = false; // Para a persegui��o, pois estamos atacando
            }
            // Se o jogador est� dentro da �rea de detec��o, mas fora da �rea de ataque
            else if (distanceToPlayer <= detectionRangeX)
            {
                isChasing = true;
                if (isAttacking)
                {
                    StopCoroutine(AttackPlayer()); // Para a coroutine de ataque se o inimigo estava atacando
                    isAttacking = false;
                }
            }
            else
            {
                isChasing = false; // Para a persegui��o se o jogador sair da �rea de detec��o
            }
        }
        else
        {
            isChasing = false; // Para a persegui��o se o jogador n�o for detectado
        }
    }

    void MoveTowardsPlayer()
    {
        if (isAttacking) return; // N�o se move durante o ataque

        // Verifica se h� ch�o � frente antes de se mover
        grond = Physics2D.Linecast(groundCheck.position, transform.position + Vector3.down * 0.1f, grondLayer);
        if (!grond)
        {
            isChasing = false; // Para a persegui��o se n�o houver ch�o
            anim.SetInteger("TransitionBanana", 0);
            return;
        }

        Collider2D playerCollider = Physics2D.OverlapBox(transform.position, new Vector2(detectionRangeX, detectionRangeY), 0f, LayerMask.GetMask("Player"));

        if (playerCollider != null)
        {
            // Move o inimigo em dire��o ao jogador
            Vector2 direction = new Vector2(playerCollider.transform.position.x - transform.position.x, 0).normalized;
            transform.Translate(direction * Mathf.Abs(speed) * Time.deltaTime);

            // Verifica se o inimigo precisa virar para o outro lado
            if ((playerCollider.transform.position.x > transform.position.x && !facingRight) ||
                (playerCollider.transform.position.x < transform.position.x && facingRight))
            {
                flip(); // Inverte a dire��o do inimigo
            }
        }
    }


    IEnumerator AttackPlayer()
    {
        isAttacking = true;

        while (isAttacking && !isDead)
        {
            if (Time.time >= lastAttackTime + attackInterval) // Verifica o intervalo entre ataques
            {
                Debug.Log("Inimigo atacando o jogador!");
                Player playerScript = player.GetComponent<Player>();
                if (playerScript != null)
                {
                    playerScript.TakeDamagePlayer(5); // Aplica dano ao jogador
                }
                lastAttackTime = Time.time;
            }
            yield return null;
        }
        isAttacking = false;
    }

    IEnumerator FlashDamage()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            // Altera a cor para vermelho
            spriteRenderer.color = Color.red;

            // Espera por 0.2 segundos
            yield return new WaitForSeconds(0.2f);

            // Retorna � cor original (geralmente branca)
            spriteRenderer.color = Color.white;
        }
    }

    public void TakeDamageEnemy(int damage)
    {
        if (isDead) return;

        StartCoroutine(FlashDamage());
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return; // Evita m�ltiplas execu��es

        isDead = true; // Marca o inimigo como morto
        anim.SetInteger("TransitionBanana", 5); // Estado de morte
        Debug.Log("Inimigo morreu!");

        // Restaura a cor padr�o
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white; // Define a cor padr�o (geralmente branca)
        }

        // Para todas as coroutines ativas
        StopAllCoroutines();

        // Remove o inimigo ap�s 1 segundo
        Destroy(gameObject, 1.6f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Desenha a caixa de detec��o
        Gizmos.DrawWireCube(transform.position, new Vector3(detectionRangeX, detectionRangeY, 0f));
        // Desenha a caixa de ataque
        Gizmos.DrawWireCube(transform.position, new Vector3(attackRangeX, attackRangeY, 0f));
    }
}