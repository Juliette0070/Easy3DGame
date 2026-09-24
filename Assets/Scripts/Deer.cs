using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Deer script

public class Deer : MonoBehaviour, IAnimal {
    public AudioSource deerSounds;
    public List<AudioClip> audioClips;
    public List<Transform> pathPoints;
    public int pathIndex;

    public Transform GetDestination() {
        return pathPoints[pathIndex];
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.name == "FirstPersonController") {
            deerSounds.Stop();
            System.Random random = new();
            deerSounds.clip = audioClips[random.Next(audioClips.Count)];
            deerSounds.Play();
        }
        else if (pathPoints[pathIndex] == other.transform) {
            //print("pathPoint: " + pathPoints[pathIndex]);
            pathIndex++;
            if (pathIndex>=pathPoints.Count) {
                pathIndex=0;
            }
        }
    }
}