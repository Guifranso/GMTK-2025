using System.Collections;
using UnityEngine;

public class EnergyyManager : MonoBehaviour
{
    // Variável estática para que possa ser acessada de qualquer lugar
    // sem precisar de uma referência direta. Ideal para pontuação.
    public static int points = 0;

    // Duração do fade em segundos
    [SerializeField]
    private float fadeDuration = 0.2f;

    // Variável para garantir que o clique não seja processado múltiplas vezes
    private bool isFading = false;

    // Componente SpriteRenderer do objeto
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        // Pega a referência do componente SpriteRenderer no início
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Este método é chamado automaticamente pelo Unity quando o mouse
    // clica em um objeto com Collider.
    private void OnMouseDown()
    {
        // Se o objeto já não estiver desaparecendo, comece o processo
        if (!isFading)
        {
            isFading = true;
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    // Coroutine para controlar o efeito de fade ao longo do tempo
    private IEnumerator FadeOutAndDestroy()
    {
        // Pega a cor original do sprite
        Color originalColor = spriteRenderer.color;

        // Calcula a cor final (totalmente transparente)
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        float elapsedTime = 0f;

        // Loop que executa a cada frame até que o tempo do fade tenha passado
        while (elapsedTime < fadeDuration)
        {
            // Interpola a cor do sprite do original para o transparente
            // ao longo do tempo especificado
            spriteRenderer.color = Color.Lerp(originalColor, targetColor, elapsedTime / fadeDuration);

            // Adiciona o tempo que passou desde o último frame
            elapsedTime += Time.deltaTime;

            // Pausa a execução da Coroutine até o próximo frame
            yield return null;
        }

        // Garante que a cor final seja totalmente transparente
        spriteRenderer.color = targetColor;

        // Incrementa a pontuação
        points++;
        Debug.Log("Points: " + points); // Opcional: mostra a pontuação no console

        // Destrói o GameObject
        Destroy(gameObject);
    }
}