using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    private AudioMixer audioMixer;

    private Dictionary<string, AudioClip> soundClips;
    private Dictionary<string, AudioClip> musicClips;
    private AudioSource sfxAudioSource;
    private AudioSource musicAudioSource;
    
    private const string AUDIO_PATH = "Audio/";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        audioMixer = Resources.Load<AudioMixer>("Audio/AudioMixer");
        AudioMixerGroup musicGroup = FindAudioMixerGroup("Music");
        AudioMixerGroup sfxGroup = FindAudioMixerGroup("SFX");

        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        sfxAudioSource.outputAudioMixerGroup = sfxGroup;
        musicAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource.outputAudioMixerGroup = musicGroup;
        musicAudioSource.loop = true;
        //Load AudioClips from Resources
        LoadAudioClips();
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void LoadAudioClips()
    {
        //To add a new sound effect, add the AudioClip to the Resources/Audio folder, then add a new entry to the dictionary below
        soundClips = new Dictionary<string, AudioClip>
        {
            { "Gunshot1", Resources.Load<AudioClip>(AUDIO_PATH + "Gunshot1") },
            { "Gunshot2", Resources.Load<AudioClip>(AUDIO_PATH + "Gunshot2") },
            { "Gunshot3", Resources.Load<AudioClip>(AUDIO_PATH + "Gunshot3") },
            { "EnemyShoot", Resources.Load<AudioClip>(AUDIO_PATH + "EnemyShoot") },
            { "Bite", Resources.Load<AudioClip>(AUDIO_PATH + "Bite3") },
            { "DuckHit", Resources.Load<AudioClip>(AUDIO_PATH + "DuckHit") },
            { "EnemyDie", Resources.Load<AudioClip>(AUDIO_PATH + "PointGet2") },
            { "GameOver", Resources.Load<AudioClip>(AUDIO_PATH + "GameOver") },
            { "Explosion", Resources.Load<AudioClip>(AUDIO_PATH + "Explosion2") },
            { "Button-Press", Resources.Load<AudioClip>(AUDIO_PATH + "Button-Press") },
            { "Button-Press2", Resources.Load<AudioClip>(AUDIO_PATH + "Button-Press2") },
            { "Button-Press3", Resources.Load<AudioClip>(AUDIO_PATH + "Button-Press3") },
            { "Button-Press4", Resources.Load<AudioClip>(AUDIO_PATH + "Button-Press4") },
            { "ItemPanelOpen", Resources.Load<AudioClip>(AUDIO_PATH + "BookClosing2") },
            { "ReadyWeapon", Resources.Load<AudioClip>(AUDIO_PATH + "ReloadRecon1") },
            { "EquipWeapon", Resources.Load<AudioClip>(AUDIO_PATH + "ReloadCyclone1") },
            { "Radiation", Resources.Load<AudioClip>(AUDIO_PATH + "Radiation2") },
            { "Deflect", Resources.Load<AudioClip>(AUDIO_PATH + "Deflect") },
            { "PlayerHurt", Resources.Load<AudioClip>(AUDIO_PATH + "PlayerHurt") },
        };

        musicClips = new Dictionary<string, AudioClip>
        {
            { "TitleMusic", Resources.Load<AudioClip>(AUDIO_PATH + "TitleMusic") },
            { "Wave1", Resources.Load<AudioClip>(AUDIO_PATH + "Wave1") },
            { "Wave2", Resources.Load<AudioClip>(AUDIO_PATH + "Wave2") },
            { "Wave3", Resources.Load<AudioClip>(AUDIO_PATH + "Wave3") },
            { "Wave4", Resources.Load<AudioClip>(AUDIO_PATH + "Wave4") },
            { "Wave5", Resources.Load<AudioClip>(AUDIO_PATH + "Wave5") },
            { "Wave6", Resources.Load<AudioClip>(AUDIO_PATH + "Wave6") },
            { "Wave7", Resources.Load<AudioClip>(AUDIO_PATH + "Wave7") },
            { "Wave8", Resources.Load<AudioClip>(AUDIO_PATH + "Wave8") },
            { "Wave9", Resources.Load<AudioClip>(AUDIO_PATH + "Wave9") }
        };
    }

    private AudioMixerGroup FindAudioMixerGroup(string groupName)
    {
        AudioMixerGroup[] groups = audioMixer.FindMatchingGroups(groupName);
        if (groups.Length > 0)
        {
            return groups[0]; // Return the first matching group
        }
        else
        {
            Debug.LogError($"AudioMixerGroup '{groupName}' not found!");
            return null;
        }
    }

    public void PlaySFX(string name)
    {
        if (soundClips.TryGetValue(name, out AudioClip clip))
        {
            sfxAudioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogError($"Clip '{name}' not found in dictionary!");
        }
    }

    public void PlayRandomSFX(string[] names)
    {
        int index = Random.Range(0, names.Length);
        if (soundClips.TryGetValue(names[index], out AudioClip clip))
            {
                sfxAudioSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogError($"Clip '{name}' not found in dictionary!");
            } 
    }

    public void PlayBackgroundMusic(string name)
    {
        if(name == "WaveMusic") //This converts WaveMusic to the name of a specific and random wave music
        {
            int index = Random.Range(1, 10);
            name = "Wave" + index;
        }
        if (musicClips.TryGetValue(name, out AudioClip clip))
        {
            // Check if the music is already playing and if it's the same clip
            if (musicAudioSource.clip == clip && musicAudioSource.isPlaying)
            {
                return; // Do nothing if the same music is already playing
            }
            musicAudioSource.clip = clip;
            musicAudioSource.Play();
        }
        else
        {
            Debug.LogError($"Clip '{name}' not found in dictionary!");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Titlescreen")
        {
            PlayBackgroundMusic("TitleMusic");
        }
        else if (scene.name == "MainScene")
        {
            PlayBackgroundMusic("WaveMusic");
        }
    }
}