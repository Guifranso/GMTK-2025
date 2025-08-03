using UnityEngine;
using System.Collections;

public class CursorManager : MonoBehaviour
{
    [Header("Sprites do Cursor")]
    [Tooltip("Arraste aqui a sprite padrão do cursor.")]
    public Texture2D cursorSpriteDefault;

    [Tooltip("Arraste aqui a sprite para quando o mouse estiver pressionado (em área vazia).")]
    public Texture2D cursorSpriteClicked;

    [Tooltip("Arraste aqui a sprite para quando o mouse estiver sobre um inimigo.")]
    public Texture2D cursorSpritePreAttack;

    // --- ALTERADO ---
    [Tooltip("A sequência de sprites que aparece ao atacar um inimigo.")]
    public Texture2D[] cursorAttackFrames; // Agora é um array para a sequência

    [Header("Configurações de Detecção")]
    [Tooltip("A tag dos objetos que serão considerados inimigos.")]
    public string enemyTag = "Enemy";

    [Header("Configurações do Cursor")]
    [Tooltip("O 'ponto quente' do cursor, onde o clique é registrado. (0,0) é o canto superior esquerdo.")]
    public Vector2 hotspot = Vector2.zero;
    
    // --- ALTERADO ---
    [Tooltip("A duração de cada frame na animação de ataque.")]
    public float attackFrameDuration = 0.1f; // Controla a velocidade da animação

    private Camera mainCamera;
    private bool isAttacking = false;

    void Start()
    {
        mainCamera = Camera.main;
        // Validação foi atualizada para checar o array
        if (cursorSpriteDefault == null || cursorSpriteClicked == null || cursorSpritePreAttack == null || cursorAttackFrames == null || cursorAttackFrames.Length == 0)
        {
            Debug.LogError("Uma ou mais sprites do cursor não foram atribuídas, ou o array de ataque está vazio!");
            return;
        }
        Cursor.SetCursor(cursorSpriteDefault, hotspot, CursorMode.Auto);
    }

    void Update()
    {
        if (isAttacking)
        {
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        bool isOverEnemy = hit.collider != null && hit.collider.CompareTag(enemyTag);

        if (isOverEnemy)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SnailMovement enemy = hit.collider.GetComponent<SnailMovement>();
                if (enemy != null)
                {
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

    // --- LÓGICA DA ANIMAÇÃO TOTALMENTE ATUALIZADA ---
    private IEnumerator AttackCursorSequence()
    {
        isAttacking = true;

        // Loop que passa por cada frame da animação de ataque
        foreach (var frame in cursorAttackFrames)
        {
            Cursor.SetCursor(frame, hotspot, CursorMode.Auto);
            yield return new WaitForSeconds(attackFrameDuration);
        }

        // Após a animação, a flag é desativada, e o Update() voltará
        // a definir o cursor correto (padrão ou pre-attack) no próximo quadro.
        isAttacking = false;
    }
}