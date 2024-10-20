using UnityEngine;

public class CameraSway : MonoBehaviour
{
    [Header("Sway Settings")]
    public float swayStrength = 0.5f;  // Controls the sway intensity
    public float smoothSpeed = 2.0f;   // Controls how smooth the sway is

    private Vector3 initialRotation;

    void Start()
    {
        // Store the initial rotation of the camera
        initialRotation = transform.localRotation.eulerAngles;
    }

    void Update()
    {
        // Get the mouse position as a percentage of the screen dimensions
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2; // Normalized from -1 to 1
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2;

        // Calculate the desired rotation based on the mouse position and sway strength
        Vector3 targetRotation = new Vector3(-mouseY * swayStrength, mouseX * swayStrength, 0) + initialRotation;

        // Smoothly interpolate towards the target rotation
        Vector3 smoothedRotation = Vector3.Lerp(transform.localRotation.eulerAngles, targetRotation, Time.deltaTime * smoothSpeed);

        // Apply the smoothed rotation to the camera
        transform.localRotation = Quaternion.Euler(smoothedRotation);
    }
}