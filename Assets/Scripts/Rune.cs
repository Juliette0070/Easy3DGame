using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Rune script

public class Rune : MonoBehaviour, IInteractable {
    public Player player;
    public int requiredOrbs = 0;
    public int requiredPet = 5;
    [SerializeField]
    string objectInteractMessage;
    [SerializeField]
    TextMeshProUGUI infoText;
    int timeBeforeDisappear;

    public string InteractMessage => objectInteractMessage;

    // Start is called before the first frame update
    void Start() {
        player = GameObject.Find("FirstPersonController").GetComponent<Player>();
        infoText.text = string.Empty;
    }

    public void Interact() {
        BeatLevel();
    }

    void Update() {
        if (timeBeforeDisappear>0) {
            timeBeforeDisappear--;
            if (timeBeforeDisappear==0) {
                infoText.text = string.Empty;
            }
        }
    }

    public void BeatLevel() {
        if (player.orbs < requiredOrbs) {
            infoText.text = "not enough orbs: " + player.orbs + "/" + requiredOrbs;
            timeBeforeDisappear = 50;
            return;
        }
        if (player.nbTimesPetted < 5) {
            infoText.text = "not enough animals petted: " + player.nbTimesPetted + "/" + requiredPet;
            timeBeforeDisappear = 50;
            return;
        }
        player.BeatLevel();
    }
}