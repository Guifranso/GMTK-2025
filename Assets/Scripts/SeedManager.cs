using UnityEngine;
using System.Collections; // Necessário para usar Coroutines

public class SeedManager : MonoBehaviour
{
    // Variável pública para definir o tempo do timer no Inspector
    public float timer = 5f; 

    // Variável pública para definir o objeto a ser instanciado no Inspector
    public GameObject spawnObject;

    void Start()
    {
        // Inicia a rotina que vai esperar e depois executar uma ação
        StartCoroutine(SpawnAfterTime());
    }

    IEnumerator SpawnAfterTime()
    {
        // 1. Espera por 'n' segundos definidos pela variável 'timer'
        yield return new WaitForSeconds(timer);

        // 2. Verifica se o 'spawnObject' foi atribuído para evitar erros
        if (spawnObject != null)
        {
            // Instancia o 'spawnObject' na mesma posição e rotação deste objeto
            Instantiate(spawnObject, transform.position, transform.rotation);
        }
        
        // 3. Destrói o próprio GameObject que contém este script
        Destroy(gameObject);
    }
}