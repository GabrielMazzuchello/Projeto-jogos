using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float JumpForce = 7f;
    private Rigidbody2D rigd;
    public Animator anim;
    public bool isground;
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    public LayerMask groundLayer; // Layer que representa o chão


    // Para o ataque
    public Transform point;
    public float radius;
    public bool isattack;

    // Para vida
    public int maxHealth = 200; // Vida máxima
    public HealthBar healthBar; // Referência para a barra de vida

    public GameObject gameOverUI; // Referência para a tela de game over

    // Variável para controlar as animações
    private int TransitionPlayer = 0;
    private bool isDead = false; // Para verificar se o jogador morreu

    // Verificação de parede
    public Transform wallCheck;
    public Vector2 wallCheckSize = new Vector2(0.2f, 1f);
    private bool isTouchingWall;

    // Materiais de fricção
    public PhysicsMaterial2D noFrictionMaterial;
    public PhysicsMaterial2D normalFrictionMaterial;

    void Start()
    {
        if (PlayerData.Instance != null)
        {
            if (PlayerData.Instance.currentHealth == 0)
            {
                PlayerData.Instance.currentHealth = PlayerData.Instance.maxHealth;
            }
            healthBar.SetHealth(PlayerData.Instance.currentHealth);
        }
        rigd = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        gameOverUI = GameObject.Find("GameOver");
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    void Update()
    {
        if (isDead) return;

        CheckGround(); // Verifica se o jogador está no chão
        CheckWallCollision();
        AdjustFriction();
        Move();
        Jump();
        Attack();

        anim.SetInteger("TransitionPlayer", TransitionPlayer);
    }

    void CheckGround()
    {
        // Verifica se há colisão no retângulo do groundCheck
        isground = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
    }

    void CheckWallCollision()
    {
        Vector2 boxCenter = wallCheck.position; // Posição da verificação
        isTouchingWall = Physics2D.OverlapBox(boxCenter, wallCheckSize, 0f, groundLayer);
    }

    void AdjustFriction()
    {
        if (isTouchingWall && !isground)
        {
            rigd.velocity = new Vector2(0, rigd.velocity.y); // Impede movimento horizontal
        }
        else
        {
            rigd.sharedMaterial = normalFrictionMaterial; // Restaura fricção
        }
    }

    void Move()
    {
        if (isDead) return; // Impede que o jogador se mova após a morte

        float teclas = Input.GetAxis("Horizontal");

        // Verifica se o jogador está tocando a parede e tentando se mover na direção da parede
        if (isTouchingWall && teclas != 0 && Mathf.Sign(teclas) == Mathf.Sign(transform.right.x))
        {
            // Remove a velocidade ao colidir na parede, para não grudar
            rigd.velocity = new Vector2(teclas * speed * 0f, rigd.velocity.y);
        }
        else
        {
            rigd.velocity = new Vector2(teclas * speed, rigd.velocity.y);
        }

        // Atualiza a direção da sprite (flip) com base no movimento horizontal
        if (teclas > 0)
        {
            transform.eulerAngles = new Vector2(0, 0);
        }
        else if (teclas < 0)
        {
            transform.eulerAngles = new Vector2(0, 180);
        }

        // Atualiza a animação com base no estado (correndo, parado, pulando)
        if (isground) // No chão
        {
            if (teclas != 0 && !isattack)
            {
                TransitionPlayer = 1; // Correndo
            }
            else if (teclas == 0 && !isattack)
            {
                TransitionPlayer = 0; // Idle
            }
        }
        else // No ar
        {
            TransitionPlayer = 2; // Pulando
        }
    }



    void Jump()
    {
        if (isDead) return;

        if (Input.GetKeyDown(KeyCode.Space) && isground && !isattack)
        {
            rigd.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            TransitionPlayer = 2;
            isground = false;
        }
    }

    void Attack()
    {
        if (isDead) return;

        if (Input.GetButtonDown("Fire1") && !isattack)
        {
            isattack = true;
            TransitionPlayer = 3;

            Collider2D[] hits = Physics2D.OverlapCircleAll(point.position, radius, LayerMask.GetMask("Enemy"));
            foreach (Collider2D hit in hits)
            {
                Debug.Log($"Atingiu: {hit.name}");

                // Verifica se o objeto atingido tem o script Enemy
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamageEnemy(10);
                }

                // Verifica se o objeto atingido tem o script Enemybanana
                Enemybanana enemyBanana = hit.GetComponent<Enemybanana>();
                if (enemyBanana != null)
                {
                    enemyBanana.TakeDamageEnemy(10);
                }
            }

            StartCoroutine(OnAttack());
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(point.position, radius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);


        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
    }

    IEnumerator OnAttack()
    {
        yield return new WaitForSeconds(0.5f);
        isattack = false;

        if (isground) TransitionPlayer = 0;
    }

    void OnCollisionEnter2D(Collision2D colisao)
    {
        if (colisao.gameObject.layer == 6)
        {
            isground = true;
        }
        if (colisao.gameObject.layer == 7)
        {
            isground = true;
        }
    }

    public void TakeDamagePlayer(int damage)
    {
        PlayerData.Instance.currentHealth -= damage;

        healthBar.SetHealth(PlayerData.Instance.currentHealth);

        if (PlayerData.Instance.currentHealth <= 0 && !isDead)
        {
            isDead = true;
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        TransitionPlayer = 4;
        anim.SetInteger("TransitionPlayer", TransitionPlayer);

        yield return new WaitForSeconds(4.5f);

        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.currentHealth = PlayerData.Instance.maxHealth;
            healthBar.SetHealth(PlayerData.Instance.currentHealth);
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogError("Tela de Game Over não encontrada!");
        }
    }
}