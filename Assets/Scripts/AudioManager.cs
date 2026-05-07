using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Components")]
    public GameObject AudioSourcePrefab;
    public AudioMixer ManagedAudioMixer;
    public TextMeshProUGUI SoundStatusText;
    public Toggle SoundToggle;

    [Header("System")]
    public List<AudioSource> AudioSources;
    public static AudioManager Instance { get; private set; }

    private bool IsMuted = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void PlaySfx(AudioClip clip, float volume)
    {
        AudioSource audioSource = Instantiate(AudioSourcePrefab).GetComponent<AudioSource>();
        UpdateAudioSourcesList();

        audioSource.clip = clip;

        if(!IsMuted)
            audioSource.volume = volume;
        else
            audioSource.volume = 0;

        audioSource.Play();
        Destroy(audioSource.gameObject, audioSource.clip.length);
        Invoke(nameof(UpdateAudioSourcesList), audioSource.clip.length);
    }

    public void StopAllSound()
    {
        UpdateAudioSourcesList();

        foreach (AudioSource audioSource in AudioSources)
        {
            audioSource.Stop();
            Destroy(audioSource.gameObject);
        }

        UpdateAudioSourcesList();
    }

    void UpdateAudioSourcesList()
    {
        AudioSources.Clear();

        foreach(AudioSource audioSource in GameObject.FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
        {
           AudioSources.Add(audioSource);
        }
    }
    
    public void OnSoundToggle()
    {
        IsMuted = SoundToggle.isOn;

        if(IsMuted)
        {
            SoundStatusText.text = "Sound Off";
        }
        else
        {
            SoundStatusText.text = "Sound On";
        }
    }
}
