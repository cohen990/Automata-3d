using UnityEngine;

namespace Player
{
    public class CameraMovement : MonoBehaviour
    {
        public float sensitivity = 1f;

        private void FixedUpdate()
        {
        
            var rotateHorizontal = Input.GetAxis ("Mouse X");
            var rotateVertical = -Input.GetAxis ("Mouse Y");
            transform.RotateAround(transform.position, Vector3.up, rotateHorizontal * sensitivity);
            transform.Rotate(new Vector3(rotateVertical, 0, 0) * sensitivity);
            var rotationEuler = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(rotationEuler.x, rotationEuler.y, 0);
            
            if (!Input.GetButtonDown("Escape")) return;
        
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