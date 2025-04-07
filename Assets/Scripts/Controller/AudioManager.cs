using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Controller 
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [Header("Background Music Settings")]
        public AudioSource musicSource;
        public float musicVolume = 0.5f;
        
        [Header("Sound Effects Settings")]
        public AudioSource sfxSource;
        public float sfxVolume = 1.0f;
        
        [Header("Audio Clips")]
        public List<AudioClip> backgroundMusic;
        public List<AudioClip> soundEffects;
        
        // Dictionary for finding sound effects by name
        [Header("Sound Dictionaries")]
        private Dictionary<string, AudioClip> bgmDictionary = new Dictionary<string, AudioClip>();
        private Dictionary<string, AudioClip> soundEffectDictionary = new Dictionary<string, AudioClip>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudio();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeAudio()
        {
            // Initialize background music source
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.volume = musicVolume;
            }
            
            // Initialize sound effects source
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.volume = sfxVolume;
            }

            // Add background music clips to dictionary
            foreach (AudioClip clip in backgroundMusic)
            {
                if (clip != null)
                {
                    bgmDictionary[clip.name] = clip;
                }
            }
            
            // Add sound effects clips to dictionary
            foreach (AudioClip clip in soundEffects)
            {
                if (clip != null)
                {
                    soundEffectDictionary[clip.name] = clip;
                }
            }
            
            // // Automatically play background music
            // if (backgroundMusic != null)
            // {
            //     PlayMusic(bgmDictionary["Ethereal_Echo_Part_1_menu"]);
            // }
        }
        
        public void PlayMusic(string musicName)
        {
            if (bgmDictionary.ContainsKey(musicName))
            {
                musicSource.clip = bgmDictionary[musicName];
                musicSource.Play();
            }
            else
            {
                Debug.LogWarning("Music not found: " + musicName);
            }
        }
        public void PlayMusic(AudioClip music)
        {
            musicSource.clip = music;
            musicSource.Play();
        }
        
        public void PlaySound(string soundName)
        {
            if (soundEffectDictionary.ContainsKey(soundName))
            {
                sfxSource.PlayOneShot(soundEffectDictionary[soundName]);
            }
            else
            {
                Debug.LogWarning("Sound not found: " + soundName);
            }
        }
        
        public void StopMusic()
        {
            musicSource.Stop();
        }
        
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            musicSource.volume = musicVolume;
        }
        
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            sfxSource.volume = sfxVolume;
        }
    }
}