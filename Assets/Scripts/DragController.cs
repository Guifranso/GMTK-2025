using UnityEngine;

public class DragController : MonoBehaviour
{
    private Camera mainCamera;
    private NPCMovement draggedNPC; // Guarda a referência do NPC que está sendo arrastado

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Quando o botão esquerdo do mouse é pressionado
        if (Input.GetMouseButtonDown(0))
        {
            // Converte a posição do mouse na tela para uma posição no mundo do jogo
            Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            // Lança um raio que detecta TODOS os objetos na posição do mouse
            RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorldPos, Vector2.zero);

            // Percorre todos os objetos que foram atingidos pelo raio
            foreach (RaycastHit2D hit in hits)
            {
                // Tenta pegar o componente NPCMovement do objeto atingido
                NPCMovement npc = hit.collider.GetComponent<NPCMovement>();

                // Se o objeto tiver o componente, encontramos nosso NPC!
                if (npc != null)
                {
                    draggedNPC = npc; // Guarda a referência dele
                    draggedNPC.IniciarArrasto(); // Avisa o NPC para começar a ser arrastado
                    break; // Para o loop, pois já encontramos o que queríamos
                }
            }
        }

        // Quando o botão esquerdo do mouse é solto
        if (Input.GetMouseButtonUp(0))
        {
            // Se estávamos arrastando um NPC
            if (draggedNPC != null)
            {
                draggedNPC.SoltarArrasto(); // Avisa o NPC que ele foi solto
                draggedNPC = null; // Limpa a referência
            }
        }
    }
}