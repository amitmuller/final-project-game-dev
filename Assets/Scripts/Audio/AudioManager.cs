using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f; // Volume slider in the inspector
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambianceSource;
    [SerializeField] private AudioSource ambiance2Source;
    
    [Header("Player Sounds")]
    [SerializeField] private Sound playerWalk;
    
    [Header("General Sounds")]
    [SerializeField] private Sound backgroundMusicGame;
    [SerializeField] private Sound backgroundMusicTutorial;
    [SerializeField] private Sound backgroundMusicOpenScene;


    private Dictionary<string, Sound> sounds;
    private List<AudioSource> effectSources; // Pool of AudioSources for effects
    private int currentEffectIndex = 0;
    private int effectPoolSize = 10;

    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize the sound dictionary
        sounds = new Dictionary<string, Sound>
        {
            // Player
            { "playerWalk", playerWalk },
            
            // General
            { "backgroundMusicGame", backgroundMusicGame },
            { "backgroundMusicOpenScene", backgroundMusicOpenScene },
            { "backgroundMusicTutorial", backgroundMusicTutorial },
        };

        // Initialize the effect source pool
        effectSources = new List<AudioSource>();
        for (int i = 0; i < effectPoolSize; i++)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            effectSources.Add(source);
        }
    }

    public Sound GetSound(string soundName)
    {
        if (sounds.ContainsKey(soundName))
        {
            return sounds[soundName];
        }

        return null;
    }
    
    /// <summary>
    /// Play a sound effect by name.
    /// </summary>
    /// <param name="soundName">Name of the sound in the dictionary.</param>
    public void PlayEffect(string soundName)
    {
        if (sounds.ContainsKey(soundName) && sounds[soundName].clip != null)
        {
            AudioSource source = effectSources[currentEffectIndex];
            Sound sound = sounds[soundName];
            source.volume = sound.volume;
            source.clip = sound.clip;
            source.PlayOneShot(sound.clip);
            currentEffectIndex = (currentEffectIndex + 1) % effectPoolSize; // Move to the next source in the pool
        }
    }

    /// <summary>
    /// Play background music by name.
    /// </summary>
    /// <param name="soundName">Name of the music in the dictionary.</param>
    public void PlayMusic(string soundName)
    {
        if (sounds.ContainsKey(soundName) && sounds[soundName].clip != null)
        {
            Sound sound = sounds[soundName];
            
            if (musicSource.isPlaying && musicSource.clip == sound.clip)
            {
                return; // Do nothing if the same music is already playing
            }
            
            musicSource.clip = sound.clip;
            musicSource.volume = sound.volume;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
    
    public void StopEffect(string soundName)
    {
        if (sounds.ContainsKey(soundName) && sounds[soundName].clip != null)
        {
            foreach (AudioSource source in effectSources)
            {
                if (source.isPlaying && source.clip == sounds[soundName].clip)
                {
                    source.Stop();
                    return; // Stop once we find the first matching source
                }
            }
        }
    }
    
    public void PlayAmbiance()
    {
        if (sounds.ContainsKey("ambianceSound") && sounds["ambianceSound"].clip != null)
        {
            Sound sound = sounds["ambianceSound"];
            ambianceSource.clip = sound.clip;
            ambianceSource.volume = sound.volume;
            ambianceSource.loop = true;
            ambianceSource.Play();
        }
        
        if (sounds.ContainsKey("ambiance2Sound") && sounds["ambiance2Sound"].clip != null)
        {
            Sound sound = sounds["ambianceSound"];
            ambiance2Source.loop = true;
            ambiance2Source.Play();
        }
    }

    /// <summary>
    /// Stop the currently playing music.
    /// </summary>
    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }
    
    public Sound GetClip(string soundName)
    {
        if (sounds.ContainsKey(soundName))
        {
            return sounds[soundName];
        }

        return null;
    }

    public void StopAmbiance()
    {
        if (ambianceSource.isPlaying)
        {
            ambianceSource.Stop();
            ambianceSource.Stop();
        }
    }
} 