using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DetectEndofClip : MonoBehaviour
{
    AudioSource _audioSource;

    public AudioClip intro;
    public AudioClip major;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = intro;
        _audioSource.Play();        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_audioSource.isPlaying)
        {
            if (SceneManager.GetActiveScene().buildIndex == 4)
            {
                _audioSource.clip = major;
            }
            else
            {
                _audioSource.clip = intro;
            }
            
            _audioSource.Play();
            Debug.Log("Play the audio");
        }
    }
}
