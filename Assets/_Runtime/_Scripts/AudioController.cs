using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ------------------------------------------------------------------------------ 
// Quiz  
// Written by: Alex Fourneaux 40022711
// For COMP 376 – Fall 2021 
// -----------------------------------------------------------------------------
public class AudioController : MonoBehaviour
{
    // Allow access from any object
    public static AudioController instance;

    [SerializeField] private List<KeyedAudioClip> AudioClipsFromEditor;
    private Dictionary<string, AudioClip> Audio;                // All audio clips neatly organised in a dictionary
    private List<AudioSource> PlayingSounds;                    // Track all sounds currently playing
    private Dictionary<int, AudioSource> LoopingSounds;         // Track all songs that are looping by index so we can later stop them individually
    private int Index = 0;                                      // Autoincrementing index for LoopingSounds dictionary
    
    void OnEnable()
    {
        instance = this;

        Audio = new Dictionary<string, AudioClip>();
        PlayingSounds = new List<AudioSource>();
        LoopingSounds = new Dictionary<int, AudioSource>();
        foreach (KeyedAudioClip clip in AudioClipsFromEditor) {
            Audio.Add(clip.name, clip.clip);
        }
    }

    void Update() {
        foreach (AudioSource sound in PlayingSounds) {
            // Find all sound effects that have finished and destroy them
            if (sound.isPlaying == false) {
                Destroy(sound);
            }
        }
    }

    // Play a one-time sound effect
    public void PlaySound(string sound) {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = Audio[sound];
        audioSource.Play();
    }

    // Play a looping sound effect and return its index
    public int PlayLoopingSound(string sound) {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = Audio[sound];
        audioSource.loop = true;
        int thisIndex = Index;
        Index++;
        LoopingSounds.Add(thisIndex, audioSource);
        audioSource.Play();
        return thisIndex;
    }

    // End a looping sound effect
    public void StopLoopingSound(int index) {
        if (LoopingSounds.ContainsKey(index)) {
            Destroy(LoopingSounds[index]);
            LoopingSounds.Remove(index);
        }
    }

    // Turn all sounds down to 20% or restore them to 100%
    public void SetQuietSounds(bool quiet = true) {
        foreach (AudioSource sound in PlayingSounds) {
            sound.volume = quiet ? 0.2f : 1.0f;
        }
        foreach (int index in LoopingSounds.Keys) {
            LoopingSounds[index].volume = quiet ? 0.2f : 1.0f;
        }
    }
}
