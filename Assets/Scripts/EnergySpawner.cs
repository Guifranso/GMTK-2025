using UnityEngine;

public class EnergySpawner : MonoBehaviour
{

    public GameObject itemPrefab;

    public GameObject spawnArea;

    Vector3 randomPosition;

    float timer;

    void Start()
    {
        timer = 5;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            float randomX = Random.Range(-1 * spawnArea.transform.localScale.x / 2, spawnArea.transform.localScale.x / 2);
            float randomY = Random.Range(-1 * spawnArea.transform.localScale.y / 2, spawnArea.transform.localScale.y / 2);
            Vector3 randomPosition = new Vector3(randomX, randomY, 0f);
            Debug.Log("Posicao aleatoria: " + randomPosition);
            Instantiate(itemPrefab, spawnArea.transform.position + randomPosition, Quaternion.identity);
            timer = 5;
        }
    }
}
