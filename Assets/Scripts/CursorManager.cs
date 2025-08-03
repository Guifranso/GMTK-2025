using UnityEngine;
using System.Collections; // Necessário para usar Coroutines

public class CursorManager : MonoBehaviour
{
    [Header("Sprites do Cursor")]
    [Tooltip("Arraste aqui a sprite padrão do cursor.")]
    public Texture2D cursorSpriteDefault;

    [Tooltip("Arraste aqui a sprite para quando o mouse estiver pressionado (em área vazia).")]
    public Texture2D cursorSpriteClicked;

    [Tooltip("Arraste aqui a sprite para quando o mouse estiver sobre um inimigo.")]
    public Texture2D cursorSpritePreAttack;

    [Tooltip("A sprite que aparece rapidamente ao atacar um inimigo.")]
    public Texture2D cursorAttack; // Nova sprite para o momento do ataque

    [Header("Configurações de Detecção")]
    [Tooltip("A tag dos objetos que serão considerados inimigos.")]
    public string enemyTag = "Enemy";

    [Header("Configurações do Cursor")]
    [Tooltip("O 'ponto quente' do cursor, onde o clique é registrado. (0,0) é o canto superior esquerdo.")]
    public Vector2 hotspot = Vector2.zero;
    public float attackCursorDuration = 0.2f; // Duração da sprite de ataque

    private Camera mainCamera;
    private bool isAttacking = false; // Flag para controlar o estado de ataque do cursor

    void Start()
    {
        mainCamera = Camera.main;
        if (cursorSpriteDefault == null || cursorSpriteClicked == null || cursorSpritePreAttack == null || cursorAttack == null)
        {
            Debug.LogError("Uma ou mais sprites do cursor não foram atribuídas no Inspector!");
            return;
        }
        Cursor.SetCursor(cursorSpriteDefault, hotspot, CursorMode.Auto);
    }

    void Update()
    {
        // Se a animação de ataque do cursor estiver ativa, não faz mais nada.
        if (isAttacking)
        {
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        bool isOverEnemy = hit.collider != null && hit.collider.CompareTag(enemyTag);

        // Se o mouse estiver sobre um inimigo
        if (isOverEnemy)
        {
            // E o jogador clicar...
            if (Input.GetMouseButtonDown(0))
            {
                // Tenta pegar o componente SnailMovement do inimigo
                SnailMovement enemy = hit.collider.GetComponent<SnailMovement>();
                if (enemy != null)
                {
                    // Chama a função de dano no inimigo
                    enemy.TakeDamage(1);
                }

                StartCoroutine(AttackCursorSequence());
            }
            else
            {
                Cursor.SetCursor(cursorSpritePreAttack, hotspot, CursorMode.Auto);
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                Cursor.SetCursor(cursorSpriteClicked, hotspot, CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(cursorSpriteDefault, hotspot, CursorMode.Auto);
            }
        }
    }

    private IEnumerator AttackCursorSequence()
    {
        isAttacking = true;
        Cursor.SetCursor(cursorAttack, hotspot, CursorMode.Auto);
        yield return new WaitForSeconds(attackCursorDuration);
        isAttacking = false;
    }
}