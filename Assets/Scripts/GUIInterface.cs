using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class GUIInterface : MonoBehaviour
{
    public GameObject GUICamera;
    public GameObject PlayerCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            Ray ray = GUICamera.GetComponent<Camera>().ScreenPointToRay(mouseScreenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.name == "Status_Red")
                {
                    Debug.Log("Hit OFF on " + hit.transform.parent.gameObject);
                    // Close Breaker
                }
                else if (hit.collider.gameObject.name == "Status_Green")
                {
                    Debug.Log("Hit ON on " + hit.transform.parent.gameObject);
                    // Open Breaker
                }
            }
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // Close the GUI
            Debug.Log("Closed GUI.");

            // Change the active camera to the Player camera, disable GUI
            GUICamera.SetActive(false);
            PlayerCamera.SetActive(true);

            // Disable the cursor
            gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
