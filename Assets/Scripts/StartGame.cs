using UnityEngine;

public class StartGame : MonoBehaviour, IInteractable
{

    public void Interact()
    {
        // Start the game!
        Debug.Log("Starting Game!");

        // Disable the start button
        gameObject.SetActive(false);
    }
}
