using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float JumpForce = 6f;
    private Rigidbody2D rigd;
    public Animator anim;
    public bool isground;

    // Para o ataque
    public Transform point;
    public float radius;
    public bool isattack;

    // Para vida
    public int maxHealth = 100; // Vida máxima
    public HealthBar healthBar; // Referência para a barra de vida

    public GameObject gameOverUI; // Referência para a tela de game over

    // Variável para controlar as animações
    private int TransitionPlayer = 0;
    private bool isDead = false; // Flag para verificar se o jogador morreu

    void Start()
    {
        // Procura pela barra de vida na cena atual (caso seja necessário ajustar referência)
        if (PlayerData.Instance != null)
        {
            Debug.Log("Vida inicial: " + PlayerData.Instance.currentHealth);  // Verifique o valor da vida

            if (PlayerData.Instance.currentHealth == 0)
            {
                PlayerData.Instance.currentHealth = PlayerData.Instance.maxHealth; // Restaura a vida se estiver 0
            }

            healthBar.SetHealth(PlayerData.Instance.currentHealth); // Atualiza a barra de vida
        }
        rigd = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        gameOverUI = GameObject.Find("GameOver");
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false); // Desativa o painel na inicialização
        }
    }

    void Update()
    {
        if (isDead) return; // Se o jogador estiver morto, não processa mais o movimento nem outras animações.

        Move();
        Jump();
        Attack();

        // Atualiza a animação baseada na transição
        anim.SetInteger("TransitionPlayer", TransitionPlayer);
    }

    void Move()
    {
        if (isDead) return; // Impede que o jogador se mova após a morte

        float teclas = Input.GetAxis("Horizontal");
        rigd.velocity = new Vector2(teclas * speed, rigd.velocity.y);

        if (teclas > 0 && isground == true && !isattack)
        {
            transform.eulerAngles = new Vector2(0, 0);
            TransitionPlayer = 1; // Correndo
        }
        else if (teclas < 0 && isground == true && !isattack)
        {
            transform.eulerAngles = new Vector2(0, 180);
            TransitionPlayer = 1; // Correndo
        }
        else if (teclas == 0 && isground == true && !isattack)
        {
            TransitionPlayer = 0; // Idle
        }
    }

    void Jump()
    {
        if (isDead) return; // Impede que o jogador pule após a morte

        if (Input.GetKeyDown(KeyCode.Space) && isground == true && !isattack)
        {
            rigd.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            TransitionPlayer = 2; // Pular
            isground = false;
        }
    }

    void Attack()
    {
        if (isDead) return; // Impede que o jogador ataque após a morte

        if (Input.GetButtonDown("Fire1") && !isattack)
        {
            isattack = true;
            TransitionPlayer = 3; // Ataque

            Collider2D[] hits = Physics2D.OverlapCircleAll(point.position, radius, LayerMask.GetMask("Enemy"));
            foreach (Collider2D hit in hits)
            {
                Debug.Log($"Atingiu: {hit.name}");

                // Aplica dano se o objeto atingido for um inimigo
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamageEnemy(7); // Dano causado pelo jogador
                    Debug.Log("Dano aplicado ao inimigo!");
                }
            }

            StartCoroutine(OnAttack());
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(point.position, radius);
    }

    IEnumerator OnAttack()
    {
        // Espera o tempo necessário para completar a animação de ataque
        yield return new WaitForSeconds(1f); // Ajuste o tempo de espera para o ataque

        isattack = false;

        if (isground) TransitionPlayer = 0; // Retorna para Idle após o ataque
    }

    void OnCollisionEnter2D(Collision2D colisao)
    {
        if (colisao.gameObject.layer == 6)
        {
            isground = true;
            Debug.Log("Estou no chão");
        }
        if (colisao.gameObject.layer == 7)
        {
            isground = true;
            Debug.Log("Tocou na caixa");
        }
    }

    // Sistema de vida
    public void TakeDamagePlayer(int damage)
    {
        PlayerData.Instance.currentHealth -= damage;

        healthBar.SetHealth(PlayerData.Instance.currentHealth);

        if (PlayerData.Instance.currentHealth <= 0 && !isDead)
        {
            isDead = true; // Marca como morto
            StartCoroutine(Die()); // Aguarda um tempo antes de mostrar a tela de game over
        }
    }

    IEnumerator Die()
    {
        TransitionPlayer = 4; // Morte
        anim.SetInteger("TransitionPlayer", TransitionPlayer); // Atualiza imediatamente a animação para morte

        // Espera o tempo necessário para a animação de morte (ajuste o valor conforme necessário)
        yield return new WaitForSeconds(4.5f); // Ajuste o tempo de espera para a animação de morte

        // Aqui você deve restaurar a vida do jogador
        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.currentHealth = PlayerData.Instance.maxHealth; // Restaura a vida do jogador
            healthBar.SetHealth(PlayerData.Instance.currentHealth); // Atualiza a barra de vida
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true); // Ativa a tela de Game Over
            Time.timeScale = 0f;       // Pausa o jogo
        }
        else
        {
            Debug.LogError("Tela de Game Over não encontrada!");
        }
    }

}
