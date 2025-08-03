using UnityEngine;
using System.Collections; // Necessário para usar Coroutines

// Garante que o GameObject tenha os componentes necessários
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Renderer))] // Garante que há um componente para alterar a cor
public class SnailMovement : MonoBehaviour
{
    [Header("Configurações de Vida e Dano")]
    [Tooltip("A vida inicial do caracol.")]
    public int life = 5;
    [Tooltip("Tempo em segundos que o caracol fica invencível após levar dano.")]
    public float invincibilityTime = 0.5f;
    [Tooltip("A cor para a qual o caracol vai tender ao levar dano.")]
    public Color damageColor = Color.red;

    private int maxLife;
    private Renderer rend;
    private Color originalColor;
    private bool canBeDamaged = true;

    [Header("Configurações de Perseguição")]
    [Tooltip("A velocidade do caracol ao perseguir um alvo.")]
    public float chaseSpeed = 3f;
    [Tooltip("O raio no qual o caracol detectará outros GameObjects.")]
    public float detectionRadius = 5f;
    [Tooltip("A Tag do objeto que o caracol deve perseguir.")]
    public string targetTag = "MiniMushroom";

    [Header("Configurações de Movimento Aleatório")]
    [Tooltip("A velocidade do caracol ao se mover aleatoriamente.")]
    public float wanderSpeed = 1f;
    public float collisionCheckDistance = 0.5f;
    public LayerMask collisionLayer;

    [Header("Controle de Tempo (Aleatório)")]
    public float minWaitTime = 4f;
    public float maxWaitTime = 7f;
    public float minMoveTime = 1f;
    public float maxMoveTime = 3f;

    // Variáveis privadas
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 wanderDirection;
    private float currentWaitTime;
    private float currentMoveTime;
    private Transform currentTarget;

    private enum SnailState
    {
        Waiting,
        Wandering,
        Chasing
    }
    private SnailState currentState;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rend = GetComponent<Renderer>(); // Pega o componente Renderer

        // Configuração inicial da vida e cor
        maxLife = life;
        if (rend != null)
        {
            originalColor = rend.material.color; // Guarda a cor original
        }
        UpdateColor(); // Garante que a cor inicial está correta

        rb.gravityScale = 0;
        currentState = SnailState.Waiting;
        currentWaitTime = Random.Range(minWaitTime, maxWaitTime);
    }

    void Update()
    {
        DetectTargetByTag();

        switch (currentState)
        {
            case SnailState.Waiting:
                if (currentTarget != null) { currentState = SnailState.Chasing; break; }
                currentWaitTime -= Time.deltaTime;
                if (currentWaitTime <= 0) { ChooseNewWanderDirection(); }
                break;

            case SnailState.Wandering:
                if (currentTarget != null) { currentState = SnailState.Chasing; break; }
                CheckWanderCollision();
                if (currentState == SnailState.Wandering) { CountMoveTime(); }
                break;

            case SnailState.Chasing:
                if (currentTarget == null) { StopAndEnterWaitState(); }
                break;
        }
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case SnailState.Chasing: MoveTowardsTarget(); break;
            case SnailState.Wandering: Wander(); break;
            case SnailState.Waiting: StopMovement(); break;
        }
    }

    // --- NOVOS MÉTODOS DE VIDA E DANO ---

    /// <summary>
    /// Função pública para aplicar dano ao caracol.
    /// </summary>
    /// <param name="damageAmount">A quantidade de vida a ser removida.</param>
    public void TakeDamage(int damageAmount)
    {
        if (!canBeDamaged)
        {
            return; // Sai da função se estiver no período de invencibilidade
        }

        life -= damageAmount;
        Debug.Log(gameObject.name + " tomou dano! Vida restante: " + life);

        UpdateColor(); // Atualiza a cor para refletir a nova vida

        if (life <= 0)
        {
            Destroy(gameObject); // Destrói o objeto se a vida chegar a zero
        }
        else
        {
            // Inicia o cooldown de invencibilidade
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    /// <summary>
    /// Atualiza a cor do material com base na porcentagem de vida restante.
    /// </summary>
    private void UpdateColor()
    {
        if (rend == null) return;

        // Interpola entre a cor de dano (vermelho) e a cor original.
        // Quanto menor a vida, mais próximo da cor de dano.
        float lifePercentage = (float)life / maxLife;
        rend.material.color = Color.Lerp(damageColor, originalColor, lifePercentage);
    }

    /// <summary>
    /// Coroutine que cria um período de invencibilidade temporário.
    /// </summary>
    private IEnumerator InvincibilityCoroutine()
    {
        canBeDamaged = false;
        yield return new WaitForSeconds(invincibilityTime);
        canBeDamaged = true;
    }


    // --- MÉTODOS DE MOVIMENTO E DETECÇÃO (sem alterações funcionais) ---
    private void DetectTargetByTag()
    {
        Collider2D[] collidersInRadius = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        Transform closestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        foreach (Collider2D col in collidersInRadius)
        {
            if (col.CompareTag(targetTag))
            {
                float dSqrToTarget = (col.transform.position - transform.position).sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    closestTarget = col.transform;
                }
            }
        }
        currentTarget = closestTarget;
    }

    private void MoveTowardsTarget()
    {
        if (currentTarget == null) return;
        animator.SetBool("isWalking", true);
        Vector2 directionToTarget = (currentTarget.position - transform.position).normalized;
        rb.linearVelocity = directionToTarget * chaseSpeed;
        FlipSprite(directionToTarget.x);
    }

    private void Wander()
    {
        animator.SetBool("isWalking", true);
        rb.linearVelocity = wanderDirection * wanderSpeed;
        FlipSprite(wanderDirection.x);
    }

    private void StopMovement()
    {
        animator.SetBool("isWalking", false);
        rb.linearVelocity = Vector2.zero;
    }

    private void ChooseNewWanderDirection()
    {
        wanderDirection = Random.insideUnitCircle.normalized;
        currentMoveTime = Random.Range(minMoveTime, maxMoveTime);
        currentState = SnailState.Wandering;
    }

    private void CountMoveTime()
    {
        currentMoveTime -= Time.deltaTime;
        if (currentMoveTime <= 0)
        {
            StopAndEnterWaitState();
        }
    }

    private void StopAndEnterWaitState()
    {
        currentState = SnailState.Waiting;
        currentWaitTime = Random.Range(minWaitTime, maxWaitTime);
    }

    private void CheckWanderCollision()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, wanderDirection, collisionCheckDistance, collisionLayer);
        if (hit.collider != null)
        {
            StopAndEnterWaitState();
        }
    }

    private void FlipSprite(float directionX)
    {
        if (directionX < 0) { spriteRenderer.flipX = true; }
        else if (directionX > 0) { spriteRenderer.flipX = false; }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}