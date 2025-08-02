using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class NPCMovement : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float movementSpeed = 2f;
    public float collisionCheckDistance = 0.5f;
    public LayerMask collisionLayer;

    [Header("Controle de Tempo (Aleatório)")]
    public float minWaitTime = 4f;
    public float maxWaitTime = 7f;
    public float minMoveTime = 1f;
    public float maxMoveTime = 3f;

    [Header("Configurações de Arrastar")]
    [Tooltip("O quanto o sprite aumenta ao ser selecionado. 1.2 = 20% maior.")]
    public float scaleMultiplier = 1.2f; // Multiplicador para o efeito de "zoom"

    // Variáveis privadas
    private Vector2 movementDirection;
    private float currentCooldown;
    private float currentMoveTime;
    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector3 originalScale; // Para guardar a escala original do objeto

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
        rb.gravityScale = 0;
        mainCamera = Camera.main;
        
        // Guarda a escala inicial do objeto para poder restaurá-la depois
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
                // Nenhuma lógica necessária aqui, tudo é controlado por outros métodos
                break;
        }
    }

    void FixedUpdate()
    {
        if (estadoAtual == EstadoNPC.Movendo)
        {
            rb.linearVelocity = movementDirection * movementSpeed;
        }
        else if (estadoAtual == EstadoNPC.Arrastado)
        {
            Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            rb.MovePosition(mouseWorldPosition);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // --- LÓGICA DE SELEÇÃO ATUALIZADA ---
    void OnMouseDown()
    {
        estadoAtual = EstadoNPC.Arrastado;
        rb.linearVelocity = Vector2.zero;

        // Aumenta a escala do objeto para dar o feedback visual
        transform.localScale = originalScale * scaleMultiplier;

        Debug.Log("NPC selecionado, aplicando zoom!");
    }

    // --- LÓGICA PARA SOLTAR ATUALIZADA ---
    void OnMouseUp()
    {
        if (estadoAtual == EstadoNPC.Arrastado)
        {
            // Restaura a escala original do objeto
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

    void OnDrawGizmos()
    {
        if (estadoAtual == EstadoNPC.Movendo)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + movementDirection * collisionCheckDistance);
        }
    }
}