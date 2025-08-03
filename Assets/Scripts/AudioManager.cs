using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Clips")]
    [Tooltip("Som para quando o personagem come")]
    public AudioClip eatSound;
    
    [Tooltip("Som para quando o personagem pega comida")]
    public AudioClip pickupSound;
    
    [Header("Audio Settings")]
    [Range(0f, 1f)]
    [Tooltip("Volume do som de comer")]
    public float eatSoundVolume = 0.7f;
    
    [Range(0f, 1f)]
    [Tooltip("Volume do som de pegar comida")]
    public float pickupSoundVolume = 0.5f;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayEatSound()
    {
        if (audioSource != null && eatSound != null)
        {
            audioSource.PlayOneShot(eatSound, eatSoundVolume);
        }
    }

    public void PlayPickupSound()
    {
        if (audioSource != null && pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound, pickupSoundVolume);
        }
    }

    // Método estático para facilitar o acesso de outros scripts
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern para garantir que só existe uma instância
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
} 