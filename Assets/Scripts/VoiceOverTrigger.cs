using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceOverTrigger : MonoBehaviour {
    public AudioSource audioSource;
    public AudioClip audioClip;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.name == "FirstPersonController") {
            audioSource.Stop();
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}