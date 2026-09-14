using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Voice Over Trigger script

public class VoiceOverTrigger : MonoBehaviour {
    public AudioSource audioSource;
    public List<AudioClip> audioClips;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.name == "FirstPersonController") {
            audioSource.Stop();
            System.Random random = new();
            audioSource.clip = audioClips[random.Next(audioClips.Count)];
            audioSource.Play();
        }
    }
}