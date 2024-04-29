using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;
    public float distance = 5f;
    public float height = 2f;
    public float rotationSpeed = 3f;
    public float sensitivityX = 1f;
    public float sensitivityY = 1f;
    public float minYAngle = -80f; // Minimum angle to look down
    public float maxYAngle = 80f; // Maximum angle to look up
    public float minDistance = 1f; // Minimum distance from the player
    public float maxDistance = 10f; // Maximum distance from the player

    private float currentHorizontalAngle = 0f;
    private float currentVerticalAngle = 0f;
    private float originalDistance;
    public bool isJumping = false;

    public PlayerController playerCont;

    void Start()
    {
        playerCont = GameObject.Find("Player").GetComponent<PlayerController>();

        if (playerTransform == null)
        {
            Debug.LogError("Player Transform not assigned to CameraController.");
            return;
        }

        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalDistance = distance;
    }

    void LateUpdate()
    {
        if (playerCont.isRunning)
        {
            if (playerTransform == null)
            {
                return;
            }

            // Update the current horizontal angle based on mouse input
            float horizontalInput = Input.GetAxis("Mouse X") * sensitivityX;
            currentHorizontalAngle += horizontalInput * rotationSpeed * Time.deltaTime;

            // Update the current vertical angle based on mouse input
            float verticalInput = Input.GetAxis("Mouse Y") * sensitivityY;
            currentVerticalAngle -= verticalInput * rotationSpeed * Time.deltaTime;
            currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minYAngle, maxYAngle);

            // Calculate the desired position around the player
            Vector3 offset = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0f) * new Vector3(0f, 0f, -distance);
            Vector3 desiredPosition = playerTransform.position + offset;

            // Check for obstacles between the camera and the player
            RaycastHit hit;
            if (Physics.Raycast(playerTransform.position, desiredPosition - playerTransform.position, out hit, maxDistance))
            {
                if (hit.distance < distance)
                {
                    distance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
                    offset = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0f) * new Vector3(0f, 0f, -distance);
                    desiredPosition = playerTransform.position + offset;
                }
            }
            else
            {
                // Reset distance to original distance if no obstacles are detected
                distance = originalDistance;
            }

            // Set the camera's position to the desired position
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 5f);

            // Look at the player
            transform.LookAt(playerTransform.position);
        }
    }
}
