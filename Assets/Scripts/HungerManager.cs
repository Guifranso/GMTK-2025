using UnityEngine;

public class HungerManager : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Arraste o GameObject de preenchimento ('HungerBarFill') para este campo.")]
    public Transform hungerBarFill;

    [Header("Configurações da Fome")]
    [Tooltip("O valor máximo que a fome pode atingir.")]
    public float maxHunger = 100f;

    // MODIFICAÇÃO 1: Adicione esta linha para criar um slider no Inspector
    [Tooltip("Arraste este slider no modo Play para testar a barra.")]
    [Range(0f, 100f)]
    [SerializeField]
    private float currentHunger;

    // Constantes com os seus valores
    private const float MIN_FILL_SCALE_X = 1f;
    private const float MIN_FILL_POSITION_X = -3.1f;
    private const float MAX_FILL_SCALE_X = 90f;
    private const float MAX_FILL_POSITION_X = -0.3f;

    // MODIFICAÇÃO 2: Adicione o método Update
    // Este método é chamado a cada frame, então ele vai verificar constantemente
    // o valor de 'currentHunger' e atualizar a barra.
    private void Update()
    {
        // Chama a função de atualização visual a cada frame
        SetHunger(currentHunger);
    }
    
    public void SetHunger(float hungerValue)
    {
        // Garante que o valor que estamos usando está dentro do limite do slider
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
    
    void Start()
    {
        // Define o valor inicial quando o jogo começa
        currentHunger = maxHunger;
        SetHunger(currentHunger); 
    }
}