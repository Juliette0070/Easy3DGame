using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Dog script

public class Dog : MonoBehaviour, IAnimal {
    public AudioSource dogSounds;
    public List<AudioClip> audioClips;
    public Transform player;
    
    public Transform getDestination() {
        return player;
    }
    

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.name == "FirstPersonController") {
            dogSounds.Stop();
            System.Random random = new();
            dogSounds.clip = audioClips[random.Next(audioClips.Count)];
            dogSounds.Play();
        }
    }
}