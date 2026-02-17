using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public enum Sounds{
    Success,
    Failed
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip[] soundArray = new AudioClip[2];

    private Dictionary<Sounds, AudioClip> soundDictionary;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            soundDictionary = new Dictionary<Sounds, AudioClip>
            {
                { Sounds.Success, soundArray[0] },
                { Sounds.Failed, soundArray[1] }
            };
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(Sounds soundToPlay)
    {
        switch (soundToPlay) {
            case Sounds.Success:
                Debug.Log("Playing success sound");
                audioSource.PlayOneShot(soundDictionary[Sounds.Success]);
                break;
            case Sounds.Failed:
                Debug.Log("Playing failed sound");
                audioSource.PlayOneShot(soundDictionary[Sounds.Failed]);
                break;
        }
    }
}
