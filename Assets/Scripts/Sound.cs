using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{

    [Tooltip("Name of this sound.")]
    public string name;

    [Tooltip("The location at which to play this sound.")]
    public GameObject location;

    [Tooltip("The last variation of this sound played.")]
    public int lastSoundPlayed;

    [Tooltip("Array of audio file versions for this sound.")]
    public AudioClip[] clips;

    [Tooltip("Reference to the audio mixer group for this sound to be played in.")]
    public AudioMixerGroup mixerGroup;

    [Tooltip("This sound's preferred volume.")]
    [Range(0f, 1f)]
    public float volume;

    [Tooltip("This sound's preferred pitch.")]
    [Range(-3f, 3f)]
    public float pitch = 1;

    [Tooltip("This sound's preferred stereo pan value.")]
    [Range(-1f, 1f)]
    public float stereoPan = 0;

    [Tooltip("This sound's preferred spatial blend value, 0 = 2D, 1 = 3D")]
    [Range(0f, 1f)]
    public float spatialBlend = 0;

    [Tooltip("Boolean flag; Whether this sound plays as soon as it is spawned in or not.")]
    public bool playOnAwake;

    [Tooltip("Boolean flag; Whether this sound loops or not.")]
    public bool loop;
}