using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Dog script

public class Dog : MonoBehaviour, IAnimal, IInteractable {
    public AudioSource dogSounds;
    public List<AudioClip> audioClips;
    public AudioClip petAudioClip;
    public Player player;
    
    [SerializeField]
    string objectInteractMessage;

    public string InteractMessage => objectInteractMessage;

    void Start() {
        player = GameObject.Find("FirstPersonController").GetComponent<Player>();
    }

    public void Interact() {
        Pet();
    }

    public void Pet() {
        dogSounds.Stop();
        dogSounds.clip = petAudioClip;
        dogSounds.Play();
        player.nbTimesPetted++;
    }

    public Transform GetDestination() {
        return player.transform;
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