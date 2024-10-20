using UnityEngine;
using System.Collections;

public class CameraSway : MonoBehaviour
{
    [Header("Sway Settings")]
    public float swayStrength = 0.5f;  // Controls the sway intensity
    public float smoothSpeed = 2.0f;   // Controls how smooth the sway is

    [Header("Shake Settings")]
    public float shakeDuration = 0.5f; // How long the camera shakes
    public float shakeMagnitude = 0.3f; // The amount of shake movement

    private Vector3 initialPosition;
    private Vector3 initialRotation;

    private Coroutine shakeCoroutine;

    void Start()
    {
        // Store the initial position and rotation of the camera
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation.eulerAngles;
    }

    void Update()
    {
        HandleCameraSway();
    }

    // Method to handle the camera sway effect
    void HandleCameraSway()
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

    // Method to trigger the camera shake
    public void TriggerCameraShake()
    {
        // Stop any ongoing shake before starting a new one
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        // Start the shake coroutine
        shakeCoroutine = StartCoroutine(CameraShake());
    }

    // Coroutine to handle the camera shake effect
    IEnumerator CameraShake()
    {
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            // Generate a random offset for the shake effect
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;

            // Apply the random offset to the camera's position
            transform.localPosition = initialPosition + randomOffset;

            // Wait for the next frame and increment the elapsed time
            elapsed += Time.deltaTime;
            yield return null;
        }

        // After shaking, reset the camera to its original position
        transform.localPosition = initialPosition;
    }
}