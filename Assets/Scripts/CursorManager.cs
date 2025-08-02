using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [Header("Sprites Originais")]
    [Tooltip("Arraste aqui a sprite padrão do cursor.")]
    public Texture2D cursorSpriteDefault;

    [Tooltip("Arraste aqui a sprite para quando o mouse estiver pressionado.")]
    public Texture2D cursorSpriteClicked;

    [Tooltip("O 'ponto quente' do cursor, onde o clique é registrado. (0,0) é o canto superior esquerdo.")]
    public Vector2 hotspot = Vector2.zero;


    void Start()
    {
        if (cursorSpriteDefault == null || cursorSpriteClicked == null)
        {
            Debug.LogError("Uma ou ambas as sprites do cursor não foram atribuídas no Inspector!");
            return;
        }

        Cursor.SetCursor(cursorSpriteDefault, hotspot, CursorMode.Auto);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.SetCursor(cursorSpriteClicked, hotspot, CursorMode.Auto);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Cursor.SetCursor(cursorSpriteDefault, hotspot, CursorMode.Auto);
        }
    }

}