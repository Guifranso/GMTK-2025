using UnityEngine;
using System.Collections.Generic; // Necessário para usar List<string>

public class EnergySpawner : MonoBehaviour
{
    [Header("Configurações do Item")]
    public GameObject itemPrefab; // O prefab do item a ser spawnado
    public float itemRadius = 0.5f; // O raio do item, para a verificação de colisão

    [Header("Configurações do Spawner")]
    public GameObject spawnArea; // A área onde os itens podem spawnar
    public float spawnCooldown = 5f; // Tempo entre cada tentativa de spawn
    public int maxSpawnAttempts = 10; // Número máximo de tentativas para achar um local livre

    [Header("Filtro de Tags")]
    [Tooltip("O item NÃO será spawnado em cima de objetos com estas tags.")]
    public List<string> forbiddenTags = new List<string>();

    private float timer;

    void Start()
    {
        // Garante que o itemPrefab tenha um collider para a verificação funcionar
        if (itemPrefab.GetComponent<Collider2D>() == null)
        {
            Debug.LogError("O itemPrefab precisa de um componente Collider2D para a verificação de spawn funcionar!");
        }

        timer = spawnCooldown;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            AttemptToSpawn();
            timer = spawnCooldown; // Reseta o timer para a próxima contagem
        }
    }

    /// <summary>
    /// Tenta encontrar uma posição válida e instanciar o item.
    /// </summary>
    void AttemptToSpawn()
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            // Calcula uma posição aleatória dentro da spawnArea
            float randomX = Random.Range(-1 * spawnArea.transform.localScale.x / 2, spawnArea.transform.localScale.x / 2);
            float randomY = Random.Range(-1 * spawnArea.transform.localScale.y / 2, spawnArea.transform.localScale.y / 2);
            Vector3 potentialPosition = spawnArea.transform.position + new Vector3(randomX, randomY, 0f);

            // Verifica se a posição é bloqueada por alguma tag proibida
            if (!IsPositionBlocked(potentialPosition))
            {
                // Posição válida! Instancia o item e sai do loop.
                Instantiate(itemPrefab, potentialPosition, Quaternion.identity);
                Debug.Log($"Item instanciado em: {potentialPosition}");
                return; // Sai da função pois o spawn foi bem-sucedido
            }
        }

        // Se o loop terminar sem encontrar um local, exibe um aviso
        Debug.LogWarning($"Não foi possível encontrar um local livre para spawnar após {maxSpawnAttempts} tentativas.");
    }

    /// <summary>
    /// Verifica se uma determinada posição está bloqueada por um objeto com uma tag proibida.
    /// </summary>
    /// <param name="position">A posição a ser verificada.</param>
    /// <returns>Retorna 'true' se a posição estiver bloqueada, 'false' caso contrário.</returns>
    private bool IsPositionBlocked(Vector2 position)
    {
        // Pega TODOS os colliders dentro do círculo de verificação
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, itemRadius);

        // Itera sobre cada collider encontrado
        foreach (var hitCollider in hitColliders)
        {
            // Verifica se a tag do collider encontrado está na nossa lista de tags proibidas
            if (forbiddenTags.Contains(hitCollider.tag))
            {
                // Se encontrarmos UMA tag proibida, a posição já é considerada bloqueada.
                return true;
            }
        }

        // Se o loop terminar, significa que nenhum dos colliders encontrados tinha uma tag proibida.
        // Portanto, a posição está livre.
        return false;
    }
}