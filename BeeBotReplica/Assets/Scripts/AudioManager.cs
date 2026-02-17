using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Sounds{
    Success,
    Failed
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private AudioClip[] _soundArray = new AudioClip[2];

    private Dictionary<Sounds, AudioClip> _soundDictionary;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _soundDictionary = new Dictionary<Sounds, AudioClip>
            {
                { Sounds.Success, _soundArray[0] },
                { Sounds.Failed, _soundArray[1] }
            };
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(Sounds soundToPlay)
    {
        _audioSource.PlayOneShot(_soundDictionary[soundToPlay]);
    }

    public void AdjustVolume(float volume)
    {
        _audioSource.volume = volume;
    }
}
