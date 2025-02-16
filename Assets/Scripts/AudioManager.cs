using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    [Tooltip("The main mixer to use for the audio in the game.")]
    public AudioMixer mainMixer;

    [SerializeField] private AudioSource sfxPrefab;

    [Tooltip("Array of most sounds to be played asynchronously.")]
    public Sound[] sounds;

    private void Awake() {

        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(this);

        // Loops through each sound in the array
        foreach (Sound s in sounds) {

            // If the sound does not have a specific location, but needs to be played upon starting—
            if (s.location == null && s.playOnAwake) {

                // Create new audio source component on the AudioManager
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();

                // Assign sound
                audioSource.clip = s.clips[0];

                // Assign variables
                SetSoundStats(audioSource, s);

                // Play sound
                PlaySoundByName(s.name, transform);
            }
        }
    }

    public void SetSoundStats(AudioSource source, Sound sound) {
        
        // Volume
        source.volume = sound.volume;

        // Mixer group
        source.outputAudioMixerGroup = sound.mixerGroup;

        // Pitch
        source.pitch = sound.pitch;

        // Spatial blend
        source.spatialBlend = sound.spatialBlend;

        // Stereo pan
        source.panStereo = sound.stereoPan;

        // Loop
        source.loop = sound.loop;

        // Play on Awake
        source.playOnAwake = sound.playOnAwake;

    }

    // Plays a sound from a specific audio file
    public void PlaySoundByFile(AudioClip clip, Transform spawnLocation) {

        // Finds the file in question
        Sound s = Array.Find(sounds, sound => sound.clips.Contains(clip));
        
        // If file exists—
        if (s != null) {
            Debug.Log("Playing sound: " + s.name + "- " + clip.name);

            AudioSource audioSource;

            if (spawnLocation) {
                audioSource = Instantiate(sfxPrefab, spawnLocation.position, Quaternion.identity);
            } else {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            // Sets the audio file to be played
            audioSource.clip = s.clips[0];

            // Sets variables
            SetSoundStats(audioSource, s);

            // Plays sound
            audioSource.Play();

            // Destroy sound object after it is done playing
            Destroy(audioSource.gameObject, audioSource.clip.length);

        } else {
            Debug.LogWarning("Could not find sound that contains file: " + clip.name);
        }
    }

    // Plays a random version of a sound by name
    public void PlaySoundByName(string name, Transform spawnLocation) {

        // Finds the sound type in question
        Sound s = Array.Find(sounds, sound => sound.name == name);
        
        // If sound exists—
        if (s != null) {

            AudioSource audioSource;

            // If given location exists, play sound at location
            if (spawnLocation) {
                audioSource = Instantiate(sfxPrefab, spawnLocation.position, Quaternion.identity);
            } 
            // Otherwise, play it from this AudioManager
            else {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            // --------------------------------RANDOM SOUND---------------------------------

            // Copies all variations to local list
            List<AudioClip> soundArray = s.clips.ToList();

            // Integer value for index of selected sound variation
            int selected;

            // If multiple variations for this sound exist—
            if (soundArray.Count > 1) {

                // Select a random one
                selected = UnityEngine.Random.Range(0, soundArray.Count);

                // If the previous sound played was selected, select a new one
                if (selected == s.lastSoundPlayed) {
                    soundArray.Remove(soundArray[selected]);
                    selected = UnityEngine.Random.Range(0, soundArray.Count);
                }
                Debug.Log("Playing random sound: " + s.name + " [" + selected + "]");

            } 
            // If only one sound variation exists, select that one
            else {
                selected = 0;
                Debug.Log("Playing single sound: " + s.name);
            }

            // Set sound clip to selected clip
            audioSource.clip = soundArray[selected];

            // Set last played sound
            s.lastSoundPlayed = selected;

            // -----------------------------------------------------------------

            // Sets variables
            SetSoundStats(audioSource, s);

            // Plays sound
            audioSource.Play();

            // Destroy sound object after it is done playing
            Destroy(audioSource.gameObject, audioSource.clip.length);
        } else {
            //Debug.LogWarning("Could not find sound: " + name);
        }
    }

    // Sets master volume (used in settings sliders)
    public void SetMasterVolume(float level) {
        mainMixer.SetFloat("masterVolume", level);
    }

    // Sets music volume (used in settings sliders)
    public void SetMusicVolume(float level) {
        mainMixer.SetFloat("musicVolume", level);
    }

    // Sets overall SFX volume (used in settings sliders)
    public void SetSFXVolume(float level) {
        mainMixer.SetFloat("sfxVolume", level);
    }

    // Sets weapons SFX volume (used in settings sliders)
    public void SetWeaponsVolume(float level) {
        mainMixer.SetFloat("weaponsVolume", level);
    }

    // Sets enemies SFX volume (used in settings sliders)
    public void SetEnemyVolume(float level) {
        mainMixer.SetFloat("enemyVolume", level);
    }
}