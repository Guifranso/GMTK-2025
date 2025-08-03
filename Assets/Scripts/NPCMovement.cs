using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class NPCMovement : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float movementSpeed = 2f;
    public float collisionCheckDistance = 0.5f;
    public LayerMask collisionLayer;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Controle de Tempo (Aleatório)")]
    public float minWaitTime = 4f;
    public float maxWaitTime = 7f;
    public float minMoveTime = 1f;
    public float maxMoveTime = 3f;

    [Header("Configurações de Arrastar")]
    [Tooltip("O quanto o sprite aumenta ao ser selecionado. 1.2 = 20% maior.")]
    public float scaleMultiplier = 1.2f;

    private Vector2 movementDirection;
    private float currentCooldown;
    private float currentMoveTime;
    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector3 originalScale;

    public enum EstadoNPC
    {
        Esperando,
        Movendo,
        Arrastado
    }
    public EstadoNPC estadoAtual;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0;
        mainCamera = Camera.main;
        originalScale = transform.localScale;
        estadoAtual = EstadoNPC.Esperando;
        currentCooldown = Random.Range(minWaitTime, maxWaitTime);
    }

    void Update()
    {
        switch (estadoAtual)
        {
            case EstadoNPC.Esperando:
                currentCooldown -= Time.deltaTime;
                if (currentCooldown <= 0)
                {
                    EscolherNovaDirecao();
                }
                break;

            case EstadoNPC.Movendo:
                VerificarColisaoECancelar();
                if (estadoAtual == EstadoNPC.Movendo)
                {
                    ContarTempoDeMovimento();
                }
                break;

            case EstadoNPC.Arrastado:
                // A lógica de seguir o mouse agora está no FixedUpdate
                break;
        }
    }

    void FixedUpdate()
    {
        if (estadoAtual == EstadoNPC.Movendo)
        {
            animator.SetBool("isWalking", true);
            rb.linearVelocity = movementDirection * movementSpeed;
            if (movementDirection.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
        else if (estadoAtual == EstadoNPC.Arrastado)
        {
            animator.SetBool("isWalking", false);
            Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            rb.MovePosition(mouseWorldPosition);
        }
        else
        {
            animator.SetBool("isWalking", false);
            rb.linearVelocity = Vector2.zero;
        }
    }

    // NOVA FUNÇÃO PÚBLICA: Chamada pelo DragController para iniciar o arrasto
    public void IniciarArrasto()
    {
        estadoAtual = EstadoNPC.Arrastado;
        rb.linearVelocity = Vector2.zero;
        transform.localScale = originalScale * scaleMultiplier;
        Debug.Log("NPC selecionado, aplicando zoom!");
    }

    // NOVA FUNÇÃO PÚBLICA: Chamada pelo DragController para soltar o NPC
    public void SoltarArrasto()
    {
        if (estadoAtual == EstadoNPC.Arrastado)
        {
            transform.localScale = originalScale;
            Debug.Log("NPC solto, retornando à escala original!");
            PararEMudarParaEspera();
        }
    }

    private void EscolherNovaDirecao()
    {
        movementDirection = Random.insideUnitCircle.normalized;
        currentMoveTime = Random.Range(minMoveTime, maxMoveTime);
        Debug.Log("Iniciando movimento na direção " + movementDirection + " por " + currentMoveTime.ToString("F2") + " segundos.");
        estadoAtual = EstadoNPC.Movendo;
    }

    private void PararEMudarParaEspera()
    {
        estadoAtual = EstadoNPC.Esperando;
        currentCooldown = Random.Range(minWaitTime, maxWaitTime);
        Debug.Log("Movimento finalizado. Esperando por " + currentCooldown.ToString("F2") + " segundos.");
    }

    private void ContarTempoDeMovimento()
    {
        currentMoveTime -= Time.deltaTime;
        if (currentMoveTime <= 0)
        {
            PararEMudarParaEspera();
        }
    }

    private void VerificarColisaoECancelar()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, movementDirection, collisionCheckDistance, collisionLayer);
        if (hit.collider != null)
        {
            Debug.Log("Colisão iminente detectada com: " + hit.collider.name + ". Cancelando movimento.");
            estadoAtual = EstadoNPC.Esperando;
            currentCooldown = 0.5f;
        }
    }
}