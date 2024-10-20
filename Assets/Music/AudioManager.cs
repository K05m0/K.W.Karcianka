using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip tavernAmbient;
    [SerializeField] private AudioClip gameAmbient;
    private AudioSource audioSource;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = tavernAmbient;
        audioSource.Play();
    }

    // Update is called once per frame
    public void StartGame()
    {
        audioSource.clip = gameAmbient;
        audioSource.volume = 0.2f;
        audioSource.Play();
    }
}
