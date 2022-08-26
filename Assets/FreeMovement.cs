using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

// This script moves the character controller forward
// and sideways based on the arrow keys.
// It also jumps when pressing space.
// Make sure to attach a character controller to the same game object.
// It is recommended that you make only one call to Move or SimpleMove per frame.

public class FreeMovement : MonoBehaviour
{
    private CharacterController _characterController;

    public float speed = 20.0f;
    public float sensitivity = 1f;

    private Vector3 _moveDirection = Vector3.zero;

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        
        var rotateHorizontal = Input.GetAxis ("Mouse X");
        var rotateVertical = Input.GetAxis ("Mouse Y");
        transform.Rotate(transform.up * (rotateHorizontal * sensitivity)); //use transform.Rotate(-transform.up * rotateHorizontal * sensitivity) instead if you dont want the camera to rotate around the player
        // transform.Rotate(-transform.right * (rotateVertical * sensitivity)); // again, use transform.Rotate(transform.right * rotateVertical * sensitivity) if you don't want the camera to rotate around the player
    }

    void Update()
    {
        _moveDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("UpDown"), Input.GetAxis("Vertical"));
        _moveDirection *= speed;
        
        // Move the controller
        _characterController.Move(_moveDirection * Time.deltaTime);

        if (Input.GetButtonDown("Escape"))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}