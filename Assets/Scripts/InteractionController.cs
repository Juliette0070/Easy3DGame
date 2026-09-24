using UnityEngine;
using TMPro;

public class InteractionController : MonoBehaviour {
    [SerializeField]
    Camera playerCamera;
    
    [SerializeField]
    TextMeshProUGUI interactionText;
    
    [SerializeField]
    float interactionDistance = 2f;

    IInteractable currentTargetedInteractable;

    public void Update() {
        UpdateCurrentInteractable();
        UpdateInteractionText();
        CheckForInterctionInput();
    }

    void UpdateCurrentInteractable() {
        var ray = playerCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));
        Physics.Raycast(ray, out var hit, interactionDistance);
        currentTargetedInteractable = hit.collider?.GetComponent<IInteractable>();
    }

    void UpdateInteractionText() {
        if (currentTargetedInteractable==null) {
            interactionText.text = string.Empty;
            return;
        }
        interactionText.text = currentTargetedInteractable.InteractMessage;
    }

    void CheckForInterctionInput() {
        if (Input.GetKeyDown(KeyCode.E) && currentTargetedInteractable!=null) {
            currentTargetedInteractable.Interact();
        }
    }
}