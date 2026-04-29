using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    public Camera playerCamera;
    
    public float InteractionDistance = 5f;

    IInteractable currentTargetedInteractable;

    // Update is called once per frame
    void Update()
    {
        UpdateCurrentInteractable();

        CheckForInteractionInput();
    }

    void UpdateCurrentInteractable()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));

        Physics.Raycast(ray, out var hit, InteractionDistance);

        currentTargetedInteractable = hit.collider?.GetComponent<IInteractable>();
    }

    void CheckForInteractionInput()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && currentTargetedInteractable != null)
        {
            currentTargetedInteractable.Interact();
        }
    }
}
