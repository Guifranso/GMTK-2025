using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Image))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject itemPrefab;
    
    private Transform originalParent;
    private Vector3 startPosition;
    private Image image;
    private Canvas rootCanvas;

    void Awake()
    {
        image = GetComponent<Image>();
        rootCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemPrefab == null) return;

        startPosition = transform.position;
        originalParent = transform.parent;
        
        transform.SetParent(rootCanvas.transform, true);
        
        image.raycastTarget = false;
        image.color = new Color(1f, 1f, 1f, 0.6f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (itemPrefab == null) return;
        
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (itemPrefab == null) return;

        ResetItemVisuals();
        
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(eventData.position);
        Collider2D hitCollider = Physics2D.OverlapPoint(worldPoint);
        
        if (hitCollider != null && hitCollider.GetComponent<TilemapCollider2D>() != null)
        {
            Tilemap tilemap = hitCollider.GetComponent<Tilemap>();
            Vector3Int cellPosition = tilemap.WorldToCell(worldPoint);
            Vector3 cellCenterPosition = tilemap.GetCellCenterWorld(cellPosition);
            
            Instantiate(itemPrefab, cellCenterPosition, Quaternion.identity);
        }
    }

    private void ResetItemVisuals()
    {
        transform.SetParent(originalParent, true);
        transform.position = startPosition;
        
        image.raycastTarget = true;
        image.color = Color.white;
    }
}