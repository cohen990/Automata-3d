using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rigidBody;

    public float speed = 20.0f;
    public float jumpSpeed = 20.0f;
    public Transform terrainTransform;
    
    private Vector3 _moveDirection = Vector3.zero;
    private Transform _camera;
    private Collider _collider;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        _camera = transform.Find("Camera");
        _collider = GetComponent<CapsuleCollider>();
        var spawn = terrainTransform.gameObject.GetComponent<Terrain.Terrain>().Spawn;
        _rigidBody.MovePosition(spawn);
    }

    private void FixedUpdate()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            _rigidBody.AddForce(new Vector3(0, jumpSpeed, 0));
        }
    }

    void Update()
    {
        _moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        _moveDirection *= speed;
        _moveDirection = Quaternion.Euler(new Vector3(0, _camera.rotation.eulerAngles.y, 0)) * _moveDirection;
        
        _rigidBody.velocity =_moveDirection * Time.deltaTime;
    }

    private bool IsGrounded()
    {
        var bounds = _collider.bounds;
        return Physics.CheckCapsule(
            bounds.center, 
            new Vector3(bounds.center.x, bounds.min.y - 0.2f, bounds.center.z),
            0.18f);
    }
}