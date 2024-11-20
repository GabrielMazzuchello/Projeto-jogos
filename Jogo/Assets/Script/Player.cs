using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        // Procura pela barra de vida na cena atual (caso seja necessário ajustar referência)
        if (PlayerData.Instance != null)
        {
            healthBar.SetMaxHealth(PlayerData.Instance.maxHealth);
            healthBar.SetHealth(PlayerData.Instance.currentHealth);
        }
        rigd = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Jump();
        Attack();
    }

    void Move()
    {
        float teclas = Input.GetAxis("Horizontal");
        rigd.velocity = new Vector2(teclas * speed, rigd.velocity.y);

        if (teclas > 0 && isground == true)
        {
            transform.eulerAngles = new Vector2(0, 0);
            anim.SetInteger("transitions", 1);
        }

        if (teclas < 0 && isground == true)
        {
            transform.eulerAngles = new Vector2(0, 180);
            anim.SetInteger("transitions", 1);
        }

        if (teclas == 0 && isground == true && isattack == false)
        {
            anim.SetInteger("transitions", 0);
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isground == true)
        {
            rigd.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            anim.SetInteger("transitions", 2);
            isground = false;
        }
    }

    void Attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            isattack = true;
            anim.SetInteger("transitions", 3);

            Collider2D hit = Physics2D.OverlapCircle(point.position, radius);

            if (hit != null)
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
        yield return new WaitForSeconds(0.33f);
        isattack = false;
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

        if (PlayerData.Instance.currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Player morreu!");
        Destroy(gameObject);
        Time.timeScale = 0f; // Pausa o jogo, simular derrota
    }

}
