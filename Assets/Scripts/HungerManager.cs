using UnityEngine;
using UnityEngine.SceneManagement; // --- ALTERAÇÃO 1: Necessário para gerenciar cenas
using System.Collections;

public class HungerManager : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Arraste o GameObject de preenchimento ('HungerBarFill') para este campo.")]
    public Transform hungerBarFill;

    [Header("Configurações da Fome")]
    [Tooltip("O valor máximo que a fome pode atingir.")]
    public float maxHunger = 100f;

    [Tooltip("Arraste este slider no modo Play para testar a barra.")]
    [Range(0f, 100f)]
    [SerializeField]
    public static float currentHunger;

    [Header("Diminuição da Fome")]
    [Tooltip("A cada quantos segundos a fome deve diminuir.")]
    public float hungerDecreaseRate = 1f; // Diminui a cada 1 segundo

    [Tooltip("Quanto de fome o personagem perde a cada intervalo.")]
    public float hungerDecreaseAmount = 2f; // Perde 2 de fome

    private float timer;

    // Constantes com os seus valores
    private const float MIN_FILL_SCALE_X = 1f;
    private const float MIN_FILL_POSITION_X = -3.1f;
    private const float MAX_FILL_SCALE_X = 90f;
    private const float MAX_FILL_POSITION_X = -0.3f;

    void Start()
    {
        // Define o valor inicial quando o jogo começa
        currentHunger = maxHunger;
        SetHunger(currentHunger);
    }

    private void Update()
    {
        // Incrementa o timer com o tempo que passou desde o último frame
        timer += Time.deltaTime;

        // Verifica se o tempo decorrido é maior ou igual à taxa de diminuição
        if (timer >= hungerDecreaseRate)
        {
            // Diminui a fome
            currentHunger -= hungerDecreaseAmount;

            // Garante que a fome não seja menor que zero
            if (currentHunger < 0)
            {
                currentHunger = 0;
            }

            // Reinicia o timer
            timer = 0f;
        }

        // Chama a função de atualização visual a cada frame para refletir as mudanças
        SetHunger(currentHunger);

        // --- ALTERAÇÃO 2: Verifica se a fome chegou a zero para mudar de cena ---
        if (currentHunger <= 0)
        {
            // Carrega a cena de GameOver.
            // O nome da cena deve ser exatamente "GameOver" (sensível a maiúsculas/minúsculas).
            SceneManager.LoadScene("GameOver");
        }
    }

    public void SetHunger(float hungerValue)
    {
        // Garante que o valor que estamos usando está dentro do limite
        currentHunger = Mathf.Clamp(hungerValue, 0f, maxHunger);

        float hungerPercentage = 0f;
        if (maxHunger > 0)
        {
            hungerPercentage = currentHunger / maxHunger;
        }

        float targetScaleX = Mathf.Lerp(MIN_FILL_SCALE_X, MAX_FILL_SCALE_X, hungerPercentage);
        float targetPositionX = Mathf.Lerp(MIN_FILL_POSITION_X, MAX_FILL_POSITION_X, hungerPercentage);

        if (hungerBarFill != null)
        {
            hungerBarFill.localScale = new Vector3(targetScaleX, hungerBarFill.localScale.y, hungerBarFill.localScale.z);
            hungerBarFill.localPosition = new Vector3(targetPositionX, hungerBarFill.localPosition.y, hungerBarFill.localPosition.z);
        }
    }
}