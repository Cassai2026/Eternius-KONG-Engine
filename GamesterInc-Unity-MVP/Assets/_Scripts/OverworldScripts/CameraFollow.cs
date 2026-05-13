using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Reference to the object the camera will follow
    public Transform target;

    // Offset from the target's position where the camera will be positioned
    public Vector3 offset;

    // How quickly the camera will catch up to its target
    public float damping;

    // Velocity used for smoothing camera movement
    private Vector3 velocity = Vector3.zero;

    // This method is called every fixed framerate frame
    void FixedUpdate()
    {
        // Calculate the desired position for the camera to move to,
        // which is the target's position plus the offset
        Vector3 movePosition = target.position + offset;

        // Smoothly move the camera from its current position to the calculated movePosition
        // using Vector3.SmoothDamp, which gradually changes a vector towards a desired target over time
        transform.position = Vector3.SmoothDamp(transform.position, movePosition, ref velocity, damping);
    }
}