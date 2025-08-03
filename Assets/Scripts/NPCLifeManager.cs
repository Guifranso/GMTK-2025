using UnityEngine;

public class NPCLifeManager : MonoBehaviour
{
    public int life;
    public float lifeDecreaseTimer;

    private float timer;
    private int maxLife; // Para guardar a vida inicial
    private Renderer rend; // Para referenciar o componente de renderização
    private Color originalColor; // Para guardar a cor original

    void Start()
    {
        // Pega o componente Renderer do objeto
        rend = GetComponent<Renderer>();

        // Guarda os valores iniciais
        maxLife = life;
        if (rend != null) // Garante que o renderer existe
        {
            originalColor = rend.material.color;
        }

        timer = lifeDecreaseTimer;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            life -= 1;
            timer = lifeDecreaseTimer;
            Debug.Log("Vida atual: " + life);

            // Atualiza a cor sempre que a vida muda
            UpdateColor();
        }

        if (life <= 0)
        {
            Destroy(gameObject);
        }
    }

    void UpdateColor()
    {
        if (rend == null) return; 

        float lifePercentage = ((float)life / maxLife / 2) + 0.5f;


        rend.material.color = Color.Lerp(Color.black, originalColor, lifePercentage);
    }
}