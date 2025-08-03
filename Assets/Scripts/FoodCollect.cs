using UnityEngine;

public class FoodCollect : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("Arraste o AudioSource do GameObject para este campo")]
    public AudioSource audioSource;
    
    [Tooltip("Arraste o arquivo de áudio para tocar quando comer (ex: Gulp.mp4)")]
    public AudioClip eatSound;
    
    [Tooltip("Se marcado, usa o AudioManager global. Se não, usa o AudioSource local")]
    public bool useGlobalAudioManager = true;

    // Variável para verificar se já está segurando um objeto
    private bool isAlreadyHolding = false;

    // Referência para o objeto que está sendo segurado
    private GameObject heldItem = null;
    private GameObject parent;


    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        // Se ainda não tem AudioSource, adiciona um
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    
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

            PlayEatSound();

            isAlreadyHolding = false;
            heldItem = null;
        }
    }
    
    private void PlayEatSound()
    {
        if (useGlobalAudioManager && AudioManager.Instance != null)
        {
            // Usa o AudioManager global
            AudioManager.Instance.PlayEatSound();
        }
        else if (audioSource != null && eatSound != null)
        {
            // Usa o AudioSource local
            audioSource.PlayOneShot(eatSound);
        }
    }
}