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

    private Transform bigMushroomTarget;

    public enum EstadoNPC
    {
        Esperando,
        Movendo,
        Arrastado,
        IndoParaCogumelo
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

        GameObject mushroomObj = GameObject.FindWithTag("BigMushroom");
        if (mushroomObj != null)
        {
            bigMushroomTarget = mushroomObj.transform;
        }
        else
        {
            Debug.LogError("NPC não encontrou o GameObject com a tag 'BigMushroom' na cena!");
        }

        // Inicia no estado Esperando através da nova função
        MudarEstado(EstadoNPC.Esperando);
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

            case EstadoNPC.IndoParaCogumelo:
                if (bigMushroomTarget != null)
                {
                    movementDirection = (bigMushroomTarget.position - transform.position).normalized;
                }
                else
                {
                    Debug.LogWarning("O alvo BigMushroom foi destruído. O NPC voltará a esperar.");
                    PararEMudarParaEspera();
                }
                break;

            case EstadoNPC.Arrastado:
                // A lógica de seguir o mouse agora está no FixedUpdate
                break;
        }
    }

    void FixedUpdate()
    {
        if (estadoAtual == EstadoNPC.Movendo || estadoAtual == EstadoNPC.IndoParaCogumelo)
        {
            animator.SetBool("isWalking", true);
            rb.linearVelocity = movementDirection * movementSpeed;

            if (movementDirection.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (movementDirection.x > 0)
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
        else // Estado Esperando
        {
            animator.SetBool("isWalking", false);
            rb.linearVelocity = Vector2.zero;
        }
    }

    //-------------------------------------------------------------------//
    //                 INÍCIO DAS FUNÇÕES ADICIONADAS                    //
    //-------------------------------------------------------------------//

    /// <summary>
    /// Centraliza todas as mudanças de estado para garantir consistência.
    /// </summary>
    /// <param name="novoEstado">O novo estado para o qual o NPC deve transicionar.</param>
    private void MudarEstado(EstadoNPC novoEstado)
    {
        // Se o estado atual era Arrastado e o novo estado NÃO É Arrastado,
        // garantimos que a escala volte ao normal.
        if (estadoAtual == EstadoNPC.Arrastado && novoEstado != EstadoNPC.Arrastado)
        {
            transform.localScale = originalScale;
            Debug.Log("Saindo do estado 'Arrastado', retornando à escala original!");
        }

        estadoAtual = novoEstado;
    }

    public void GoToBigMushroom()
    {
        if (bigMushroomTarget == null)
        {
            Debug.LogError("Não é possível ir até o 'BigMushroom' porque ele não foi encontrado na cena.");
            return;
        }

        Debug.Log("Recebeu ordem para ir ao BigMushroom!");
        // Usa a nova função para mudar o estado
        MudarEstado(EstadoNPC.IndoParaCogumelo);
        rb.linearVelocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D  collision)
    {
        if (estadoAtual == EstadoNPC.IndoParaCogumelo && collision.gameObject.CompareTag("BigMushroom"))
        {
            Debug.Log("Chegou ao BigMushroom! Retornando ao comportamento normal.");
            PararEMudarParaEspera();
        }
    }

    //-------------------------------------------------------------------//
    //                  FIM DAS FUNÇÕES ADICIONADAS                      //
    //-------------------------------------------------------------------//

    public void IniciarArrasto()
    {
        // Agora, usamos MudarEstado
        MudarEstado(EstadoNPC.Arrastado);
        rb.linearVelocity = Vector2.zero;
        transform.localScale = originalScale * scaleMultiplier;
        Debug.Log("NPC selecionado, aplicando zoom!");
    }

    public void SoltarArrasto()
    {
        if (estadoAtual == EstadoNPC.Arrastado)
        {
            // A restauração da escala já é tratada por MudarEstado,
            // mas podemos deixar aqui por segurança ou remover.
            // transform.localScale = originalScale; 
            Debug.Log("NPC solto!");
            PararEMudarParaEspera();
        }
    }

    private void EscolherNovaDirecao()
    {
        movementDirection = Random.insideUnitCircle.normalized;
        currentMoveTime = Random.Range(minMoveTime, maxMoveTime);
        Debug.Log("Iniciando movimento na direção " + movementDirection + " por " + currentMoveTime.ToString("F2") + " segundos.");
        // Usa a nova função para mudar o estado
        MudarEstado(EstadoNPC.Movendo);
    }

    private void PararEMudarParaEspera()
    {
        // Usa a nova função para mudar o estado
        MudarEstado(EstadoNPC.Esperando);
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
            // Usa a nova função para mudar o estado
            MudarEstado(EstadoNPC.Esperando);
            currentCooldown = 0.5f;
        }
    }
}