using UnityEngine;

public class OpenGUI : MonoBehaviour, IInteractable
{
    //public GameObject Player;
    public GameObject playerCam;
    public GameObject GUICam;
    public GameObject GUI;

    public void Interact()
    {
        Debug.Log("Opened GUI");

        // TODO: Disable player movement

        // Change the active camera to the GUI camera
        GUICam.SetActive(true);
        playerCam.SetActive(false);

        // Activate the GUI elements
        GUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
