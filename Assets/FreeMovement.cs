using UnityEngine;
using System.Collections;

// This script moves the character controller forward
// and sideways based on the arrow keys.
// It also jumps when pressing space.
// Make sure to attach a character controller to the same game object.
// It is recommended that you make only one call to Move or SimpleMove per frame.

public class FreeMovement : MonoBehaviour
{
    CharacterController _characterController;

    public float speed = 20.0f;

    private Vector3 _moveDirection = Vector3.zero;

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        _moveDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("UpDown"), Input.GetAxis("Vertical"));
        _moveDirection *= speed;
        
        // Move the controller
        _characterController.Move(_moveDirection * Time.deltaTime);
    }
}