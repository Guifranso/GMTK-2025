using UnityEngine;

public class FoodCollect : MonoBehaviour
{
    // Variável para verificar se já está segurando um objeto
    private bool isAlreadyHolding = false;

    // Referência para o objeto que está sendo segurado
    private GameObject heldItem = null;
    private GameObject parent;

    void Start()
    {
        parent = transform.parent.gameObject;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food") && !isAlreadyHolding)
        {
            isAlreadyHolding = true;

            heldItem = other.gameObject;

            heldItem.transform.SetParent(transform);

            heldItem.transform.localPosition = Vector3.zero;

            parent.GetComponent<NPCMovement>().GoToBigMushroom();
        }
        else if (other.CompareTag("BigMushroom") && isAlreadyHolding)
        {
            Destroy(heldItem);
            HungerManager.currentHunger += 10;

            isAlreadyHolding = false;
            heldItem = null;
        }
    }
}