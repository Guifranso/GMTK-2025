using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Image))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject itemPrefab;
    public int itemPrice;

    private Transform originalParent;
    private Vector3 startPosition;
    private Image image;
    private Canvas rootCanvas;
    private bool isDragging = false;

    void Awake()
    {
        image = GetComponent<Image>();
        rootCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // --- MODIFICAÇÃO PRINCIPAL ---
        // Verifica se o item pode ser arrastado (tem um prefab E os pontos de energia são maiores que 0)
        if (itemPrefab == null || EnergyyManager.points < itemPrice)
        {
            isDragging = false;
            // Cancela a operação de arrastar completamente.
            // Definir pointerDrag como nulo informa ao EventSystem para parar de processar este arraste.
            eventData.pointerDrag = null; 
            return;
        }
        // --- FIM DA MODIFICAÇÃO ---

        // Se todas as verificações passarem, configure o arraste
        isDragging = true;
        startPosition = transform.position;
        originalParent = transform.parent;
        
        // Move o item para o topo da hierarquia da UI para que ele apareça sobre outros elementos
        transform.SetParent(rootCanvas.transform, true);
        
        // Torna o item semitransparente e desativa o raycast para que o mouse possa detectar o que está abaixo dele
        image.raycastTarget = false;
        image.color = new Color(1f, 1f, 1f, 0.6f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Só move o item se o arraste foi iniciado com sucesso
        if (!isDragging) return;
        
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Só processa o final do arraste se ele foi iniciado com sucesso
        if (!isDragging) return;

        // Reseta o estado da flag e a aparência do item
        isDragging = false;
        ResetItemVisuals();
        
        // Converte a posição da tela para a posição do mundo do jogo
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(eventData.position);
        // Verifica se há um colisor na posição onde o item foi solto
        Collider2D hitCollider = Physics2D.OverlapPoint(worldPoint);

        // Verifica se o colisor é um TilemapCollider2D
        if (hitCollider != null && hitCollider.GetComponent<TilemapCollider2D>() != null)
        {
            Tilemap tilemap = hitCollider.GetComponent<Tilemap>();
            // Converte a posição do mundo para uma célula do tilemap
            Vector3Int cellPosition = tilemap.WorldToCell(worldPoint);
            // Pega a posição central da célula para posicionar o item perfeitamente
            Vector3 cellCenterPosition = tilemap.GetCellCenterWorld(cellPosition);

            // Instancia o item no tilemap e deduz os pontos
            Instantiate(itemPrefab, cellCenterPosition, Quaternion.identity);
            EnergyyManager.points -= itemPrice;
        }
    }

    private void ResetItemVisuals()
    {
        // Retorna o item para seu local e pai originais
        transform.SetParent(originalParent, true);
        transform.position = startPosition;
        
        // Restaura a cor e o raycast do item
        image.raycastTarget = true;
        image.color = Color.white;
    }
}
