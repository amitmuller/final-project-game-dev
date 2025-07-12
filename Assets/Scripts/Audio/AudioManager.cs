using System.Collections;
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
    [Header("Timing for Player Screech")]
    [SerializeField] private float minScreechInterval = 15f;
    [SerializeField] private float maxScreechInterval = 45f;

    
    [Header("Timing for Occasional Ambiance")]
    [SerializeField] private float minAmbianceInterval = 10f;
    [SerializeField] private float maxAmbianceInterval = 30f;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambianceSource;
    [SerializeField] private AudioSource ambiance2Source;
    
    [Header("Player Sounds")]
    [SerializeField] private Sound playerWalk;
    [SerializeField] private Sound playerScreech;
    [SerializeField] private Sound playerInHide;
    
    [Header("Enemy Sounds")]
    [SerializeField] private Sound enemyWalk;
    [SerializeField] private Sound enemyGasp;
    [SerializeField] private Sound enemyConfused;
    [SerializeField] private Sound enemyWhistle;
    
    [Header("General Sounds")]
    [SerializeField] private Sound backgroundMusicGame;
    [SerializeField] private Sound backgroundMusicTutorial;
    [SerializeField] private Sound backgroundMusicOpenScene;
    [SerializeField] private Sound trainSound;
    [SerializeField] private Sound breakSound1;
    [SerializeField] private Sound breakSound2;
    [SerializeField] private Sound breakSound3;
    

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

        // Initialize the sound dictionary with all your Sound fields
        sounds = new Dictionary<string, Sound>
        {
            // Player
            { "playerWalk",        playerWalk },
            { "playerScreech",     playerScreech },
            { "playerInHide",      playerInHide },

            // Enemy
            { "enemyWalk",         enemyWalk },
            { "enemyGasp",         enemyGasp },
            { "enemyConfused",     enemyConfused },
            { "enemyWhistle",      enemyWhistle },

            // Music
            { "backgroundMusicGame",      backgroundMusicGame },
            { "backgroundMusicTutorial",  backgroundMusicTutorial },
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
        
        if (sounds.TryGetValue("trainSound", out Sound train) && train.clip != null)
        {
            ambianceSource.clip  = train.clip;
            ambianceSource.volume = train.volume;
            ambianceSource.loop   = true;
            ambianceSource.Play();
        }

        // 2) Begin randomly playing chochoTrian or lightsMove
        StartCoroutine(PlayOccasionalAmbiance());
        StartCoroutine(PlayOccasionalPlayerScreech());
    }
    
    private IEnumerator PlayOccasionalPlayerScreech()
    {
        while (true)
        {
            float wait = Random.Range(minScreechInterval, maxScreechInterval);
            yield return new WaitForSeconds(wait);
            // safe-play the screech effect
            PlayEffect("playerScreech");
        }
    }

    private IEnumerator PlayOccasionalAmbiance()
    {
        // Make sure we have both sounds
        Sound chocho = sounds.ContainsKey("chochoTrian") ? sounds["chochoTrian"] : null;
        Sound lights = sounds.ContainsKey("lightsMove") ? sounds["lightsMove"] : null;

        while (true)
        {
            // wait a random time before the next “pop”
            float wait = Random.Range(minAmbianceInterval, maxAmbianceInterval);
            yield return new WaitForSeconds(wait);

            // pick one of the two (skip if it’s missing)
            Sound toPlay = (Random.value < 0.5f) ? chocho : lights;
            if (toPlay?.clip != null)
            {
                ambiance2Source.PlayOneShot(toPlay.clip, toPlay.volume);
            }
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
    
    public void PlayRandomBreak(MaterialType material)
    {
        string key1 = "", key2 = "";

        switch (material)
        {
            case MaterialType.Glass:
                key1 = "glassBreak1";
                key2 = "glassBreak2";
                break;
            case MaterialType.Wood:
                key1 = "woodBreak1";
                key2 = "woodBreak2";
                break;
            case MaterialType.Stone:
                key1 = "stoneBreak1";
                key2 = "stoneBreak2";
                break;
        }

        // flip a coin
        string chosenKey = (Random.value < 0.5f) ? key1 : key2;
        PlayEffect(chosenKey);
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